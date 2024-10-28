using System;
using System.Buffers.Binary;
using System.Text;

namespace Benchmark.MsgPackVsHandWritten;

public static class ByteExt
{
    public static void WriteString(this Span<byte> span, string text, ref int offset)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset, 2), (ushort)text.Length);
        Encoding.UTF8.GetBytes(text, span.Slice(offset, text.Length));

        offset += 2 + text.Length;
    }

    public static string ReadString(this ReadOnlySpan<byte> span, ref int offset)
    {
        ushort length = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(offset, 2));
        string text = Encoding.UTF8.GetString(span.Slice(offset, length));

        offset += 2 + text.Length;

        return text;
    }

    public static void WriteDateTimeOffset(this Span<byte> span, DateTimeOffset time, ref int offset)
    {
        // Convert Ticks to bytes (8 bytes) and copy to span
        BinaryPrimitives.WriteInt64LittleEndian(span.Slice(offset, 8), time.Ticks);
        // Convert Offset to minutes and then to bytes (2 bytes) and copy to span
        BinaryPrimitives.WriteUInt16LittleEndian(span.Slice(offset + 8, 2), (ushort)time.Offset.TotalMinutes);

        offset += 10;
    }

    public static DateTimeOffset ReadDateTimeOffset(this ReadOnlySpan<byte> span, ref int offset)
    {
        long ticks = BinaryPrimitives.ReadInt64LittleEndian(span.Slice(offset, 8));
        TimeSpan offsetTime = TimeSpan.FromMinutes(BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(offset + 8, 2)));

        offset += 10;

        return new DateTimeOffset(ticks, offsetTime);
    }

    public static void WriteInt(this Span<byte> span, int value, ref int offset)
    {
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(offset, 4), value);

        offset += 4;
    }

    public static int ReadInt(this ReadOnlySpan<byte> span, ref int offset)
    {
        int value = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(offset, 4));

        offset += 4;

        return value;
    }

    public static void WriteDecimal(this Span<byte> span, decimal value, ref int offset)
    {
        int[] bits = decimal.GetBits(value);

        for (int i = 0; i < bits.Length; i++)
        {
            BinaryPrimitives.WriteInt32LittleEndian(span.Slice(offset + i * 4, 4), bits[i]);
        }

        offset += 16;
    }

    public static decimal ReadDecimal(this ReadOnlySpan<byte> span, ref int offset)
    {
        int[] bits = new int[4];

        for (int i = 0; i < bits.Length; i++)
        {
            bits[i] = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(offset + i * 4, 4));
        }

        offset += 16;

        return new decimal(bits);
    }

    public static void WriteBool(this Span<byte> span, bool value, ref int offset)
    {
        span[offset] = value ? (byte)1 : (byte)0;

        offset++;
    }

    public static bool ReadBool(this ReadOnlySpan<byte> span, ref int offset)
    {
        bool value = span[offset] == 1;

        offset++;

        return value;
    }
}