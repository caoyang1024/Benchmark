using System;
using System.Buffers.Text;
using System.Runtime.InteropServices;

namespace Benchmark.GUIDvsULID;

public static class UlidHelper
{
    private const char Equal = '=';
    private const char Hyphen = '-';
    private const char Underscore = '_';
    private const char Slash = '/';
    private const byte SlashByte = (byte)'/';
    private const char Plus = '+';
    private const byte PlusByte = (byte)'+';

    /// <summary>
    /// get a base64 encoded ulid
    /// </summary>
    /// <returns></returns>
    public static string GetUlid()
    {
        return ToBase64(Ulid.NewUlid());
    }

    private static string ToBase64(Ulid ulid)
    {
        Span<byte> bytes = stackalloc byte[16];
        Span<byte> base64Chars = stackalloc byte[24];

        MemoryMarshal.TryWrite(bytes, in ulid);

        Base64.EncodeToUtf8(bytes, base64Chars, out _, out _);

        Span<char> chars = stackalloc char[22];

        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = base64Chars[i] switch
            {
                SlashByte => Hyphen,
                PlusByte => Underscore,
                _ => (char)base64Chars[i]
            };
        }

        return new string(chars);
    }

    private static Ulid FromBase64(ReadOnlySpan<char> base64)
    {
        Span<char> base64Chars = stackalloc char[24];

        for (int i = 0; i < base64.Length; i++)
        {
            base64Chars[i] = base64[i] switch
            {
                Hyphen => Slash,
                Underscore => Plus,
                _ => base64[i]
            };
        }

        base64Chars[22] = Equal;
        base64Chars[23] = Equal;

        Span<byte> ulidBytes = stackalloc byte[16];

        Convert.TryFromBase64Chars(base64Chars, ulidBytes, out _);

        return new Ulid(ulidBytes);
    }

    public static Ulid FromBase64(string base64)
    {
        return FromBase64(base64.AsSpan());
    }

    public static string GetUlidPlain()
    {
        var ulid = Ulid.NewUlid();

        return ulid.ToBase64();
    }

    public static Ulid FromBase64Plain(string base64)
    {
        var bytes = Convert.FromBase64String(base64);

        return new Ulid(bytes);
    }
}