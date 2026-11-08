using System.Buffers.Binary;

namespace Shared;

public static class PacketCodec
{
    public const int HeaderSize = 9;
    
    public static byte[] Encode(PacketType type, ushort seq, byte[] payload)
    {
        var buf = new byte[HeaderSize + payload.Length];

        buf[0] = (byte)type;
        BinaryPrimitives.WriteUInt16BigEndian(buf.AsSpan(1), seq);
        BinaryPrimitives.WriteUInt16BigEndian(buf.AsSpan(3), (ushort)payload.Length);
        payload.CopyTo(buf, HeaderSize);
        
        uint crc = Crc32.Compute(buf);
        BinaryPrimitives.WriteUInt32BigEndian(buf.AsSpan(5), crc);
        return buf;
    }
    
    public static bool TryDecode(byte[] data, out PacketType type, out ushort seq, out byte[] payload)
    {
        type = 0; seq = 0; payload = Array.Empty<byte>();

        if (data.Length < HeaderSize) return false;

        ushort size = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(3));
        if (data.Length != HeaderSize + size) return false;

        uint received = BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(5));
        
        var copy = (byte[])data.Clone();
        copy.AsSpan(5, 4).Clear();
        if (Crc32.Compute(copy) != received) return false;

        type = (PacketType)data[0];
        seq = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(1));
        payload = data.AsSpan(HeaderSize).ToArray();
        return true;
    }
}