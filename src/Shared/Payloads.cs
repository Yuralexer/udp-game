using System.Buffers.Binary;
using System.Text;

namespace UdpGame.Shared;

public readonly record struct MovementPayload(float X, float Y, float Z)
{
    public const int Size = 12;

    public byte[] ToBytes()
    {
        var b = new byte[Size];
        BinaryPrimitives.WriteSingleBigEndian(b.AsSpan(0), X);
        BinaryPrimitives.WriteSingleBigEndian(b.AsSpan(4), Y);
        BinaryPrimitives.WriteSingleBigEndian(b.AsSpan(8), Z);
        return b;
    }

    public static bool TryParse(byte[] p, out MovementPayload result)
    {
        result = default;
        if (p.Length != Size) return false;
        result = new MovementPayload(
            BinaryPrimitives.ReadSingleBigEndian(p.AsSpan(0)),
            BinaryPrimitives.ReadSingleBigEndian(p.AsSpan(4)),
            BinaryPrimitives.ReadSingleBigEndian(p.AsSpan(8)));
        return true;
    }
}

public readonly record struct ShootPayload(byte WeaponId)
{
    public byte[] ToBytes() => new[] { WeaponId };

    public static bool TryParse(byte[] p, out ShootPayload result)
    {
        result = default;
        if (p.Length != 1) return false;
        result = new ShootPayload(p[0]);
        return true;
    }
}

public readonly record struct AuthPayload(string Login, string Password)
{
    public byte[] ToBytes()
    {
        byte[] l = Encoding.UTF8.GetBytes(Login);
        byte[] p = Encoding.UTF8.GetBytes(Password);
        var b = new byte[2 + l.Length + p.Length];
        b[0] = (byte)l.Length;
        l.CopyTo(b, 1);
        b[1 + l.Length] = (byte)p.Length;
        p.CopyTo(b, 2 + l.Length);
        return b;
    }

    public static bool TryParse(byte[] b, out AuthPayload result)
    {
        result = default;
        if (b.Length < 2) return false;
        int lLen = b[0];
        if (b.Length < 2 + lLen) return false;
        int pLen = b[1 + lLen];
        if (b.Length != 2 + lLen + pLen) return false;
        result = new AuthPayload(
            Encoding.UTF8.GetString(b, 1, lLen),
            Encoding.UTF8.GetString(b, 2 + lLen, pLen));
        return true;
    }
}