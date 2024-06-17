using System;
using System.Buffers.Text;
using System.Runtime.InteropServices;

namespace Benchmark.GUIDvsULID;

public static class GuidHelper
{
    private const char Equal = '=';
    private const char Hyphen = '-';
    private const char Underscore = '_';
    private const char Slash = '/';
    private const byte SlashByte = (byte)'/';
    private const char Plus = '+';
    private const byte PlusByte = (byte)'+';

    /// <summary>
    /// get a base64 encoded guid
    /// </summary>
    /// <returns></returns>
    public static string GetGuid()
    {
        return ToBase64(Guid.NewGuid());
    }

    private static string ToBase64(Guid guid)
    {
        Span<byte> guidBytes = stackalloc byte[16];
        Span<byte> base64Chars = stackalloc byte[24];

        MemoryMarshal.TryWrite(guidBytes, ref guid);

        Base64.EncodeToUtf8(guidBytes, base64Chars, out _, out _);

        Span<char> guidChars = stackalloc char[22];

        for (int i = 0; i < guidChars.Length; i++)
        {
            guidChars[i] = base64Chars[i] switch
            {
                SlashByte => Hyphen,
                PlusByte => Underscore,
                _ => (char)base64Chars[i]
            };
        }

        return new string(guidChars);
    }

    private static Guid FromBase64(ReadOnlySpan<char> guid64)
    {
        Span<char> base64Chars = stackalloc char[24];

        for (int i = 0; i < guid64.Length; i++)
        {
            base64Chars[i] = guid64[i] switch
            {
                Hyphen => Slash,
                Underscore => Plus,
                _ => guid64[i]
            };
        }

        base64Chars[22] = Equal;
        base64Chars[23] = Equal;

        Span<byte> guidBytes = stackalloc byte[16];

        Convert.TryFromBase64Chars(base64Chars, guidBytes, out _);

        return new Guid(guidBytes);
    }

    public static Guid FromBase64(string guid64)
    {
        return FromBase64(guid64.AsSpan());
    }
}