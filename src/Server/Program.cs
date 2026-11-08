using System.Net;
using System.Net.Sockets;
using Shared;
using UdpGame.Shared;

const int Port = 9050;

var users = new Dictionary<string, string>
{
    ["yuriy"] = "123",
    ["vasya"]   = "qwerty"
};

var players = new Dictionary<IPEndPoint, PlayerState>();

var udp = new UdpClient(Port);
var sender = new IPEndPoint(IPAddress.Any, 0);
var rng = new Random();

Log($"Сервер запущен на порту {Port}");

while (true)
{
    byte[] raw = udp.Receive(ref sender);
    
    if (!PacketCodec.TryDecode(raw, out var type, out var seq, out var payload))
    {
        Log($"[{sender}] ПОВРЕЖДЁННЫЙ пакет ({raw.Length} байт) — отброшен");
        continue;
    }

    Log($"[{sender}] <- {type} seq={seq} payload={payload.Length}б");

    switch (type)
    {
        case PacketType.Auth:
            if (AuthPayload.TryParse(payload, out var auth)
                && users.TryGetValue(auth.Login, out var pass) && pass == auth.Password)
            {
                players[sender] = new PlayerState(auth.Login);
                Log($"[{sender}] Вход выполнен: {auth.Login}");
                Reply(PacketType.AuthResult, seq, new byte[] { 1 });
            }
            else
            {
                Log($"[{sender}] Неверный логин/пароль");
                Reply(PacketType.AuthResult, seq, new byte[] { 0 });
            }
            break;

        case PacketType.Movement:
            if (!players.TryGetValue(sender, out var p1)) { Reply(PacketType.Error, seq, Array.Empty<byte>()); break; }
            if (MovementPayload.TryParse(payload, out var move))
            {
                p1.Position = move;
                Log($"[{sender}] {p1.Login} переместился в {move.X}, {move.Y}, {move.Z}");
                Reply(PacketType.PositionUpdate, seq, move.ToBytes());
            }
            break;

        case PacketType.Shoot:
            if (!players.TryGetValue(sender, out var p2)) { Reply(PacketType.Error, seq, Array.Empty<byte>()); break; }
            if (ShootPayload.TryParse(payload, out var shoot))
            {
                byte hit = (byte)(rng.Next(2));
                Log($"[{sender}] {p2.Login} выстрелил из оружия #{shoot.WeaponId}, попадание={hit}");
                Reply(PacketType.ShootResult, seq, new[] { shoot.WeaponId, hit });
            }
            break;

        default:
            Log($"[{sender}] Неизвестный тип пакета: {type}");
            Reply(PacketType.Error, seq, Array.Empty<byte>());
            break;
    }
}

void Reply(PacketType t, ushort s, byte[] data)
{
    byte[] bytes = PacketCodec.Encode(t, s, data);
    udp.Send(bytes, bytes.Length, sender);
}

void Log(string msg)
{
    string line = $"{DateTime.Now:HH:mm:ss.fff} {msg}";
    Console.WriteLine(line);
    File.AppendAllText("server.log", line + Environment.NewLine);
}

class PlayerState(string login)
{
    public string Login { get; } = login;
    public MovementPayload Position { get; set; }
}