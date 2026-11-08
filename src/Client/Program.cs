using System.Net;
using System.Net.Sockets;
using UdpGame.Shared;
using Shared;

var udp = new UdpClient();
udp.Connect("127.0.0.1", 9050);
udp.Client.ReceiveTimeout = 2000;

ushort seq = 0;
var from = new IPEndPoint(IPAddress.Any, 0);

SendAndPrint(PacketType.Auth, new AuthPayload("yuriy", "123").ToBytes());

float x = 0;
for (int i = 0; ; i++)
{
    x += 1.5f;
    SendAndPrint(PacketType.Movement, new MovementPayload(x, 0f, 5f).ToBytes());

    if (i % 3 == 2)
        SendAndPrint(PacketType.Shoot, new ShootPayload(1).ToBytes());

    Thread.Sleep(1000);
}

void SendAndPrint(PacketType type, byte[] payload)
{
    ushort s = ++seq;
    byte[] bytes = PacketCodec.Encode(type, s, payload);
    udp.Send(bytes, bytes.Length);
    Console.WriteLine($"-> {type} seq={s}");

    try
    {
        byte[] raw = udp.Receive(ref from);
        if (!PacketCodec.TryDecode(raw, out var rType, out var rSeq, out var rPayload))
        {
            Console.WriteLine("<- повреждённый ответ");
            return;
        }

        string details = rType switch
        {
            PacketType.AuthResult     => rPayload[0] == 1 ? "вход выполнен" : "отказ",
            PacketType.PositionUpdate => MovementPayload.TryParse(rPayload, out var m)
                ? $"позиция {m.X}, {m.Y}, {m.Z}" : "?",
            PacketType.ShootResult    => $"оружие #{rPayload[0]}, попадание={rPayload[1]}",
            _                         => ""
        };
        Console.WriteLine($"<- {rType} seq={rSeq} {details}");
    }
    catch (SocketException)
    {
        Console.WriteLine($"<- таймаут (ответ на seq={s} не пришёл)");
    }
}