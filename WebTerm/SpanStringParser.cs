using System.Linq;
using System;
using System.Runtime.CompilerServices;

namespace BH.BleuIO;

using ROSC = ReadOnlySpan<char>;

public ref struct SpanStringParser(ROSC x)
{
    public ROSC buffer = x;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsEmpty() => buffer.IsEmpty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ROSC Remaining() => buffer;

    public ROSC TakeToDelimiter(params ROSC cs)
    {
        var chunk = TakeUntil(cs);
        SkipWhile(cs);
        return chunk;
    }

    /// <summary>
    /// Take until encountering the first delimiter, then skip all following delimiters
    /// </summary>
    /// <param name="cs"></param>
    public void SkipToDelimiter(params ROSC cs)
    {
        TakeUntil(cs);  
        SkipWhile(cs);
    }

    /// <summary>
    /// Take until encountering the first delimiter, or the end of span.
    /// </summary>
    /// <param name="cs">delimiters</param>
    /// <returns></returns>
    public ROSC TakeUntil( params ROSC cs)
    {
        var index = buffer.IndexOfAny(cs);

        if (index < 0)
        {
            var res = buffer;
            buffer = ROSC.Empty;
            return res;
        }

        if (index == 0)
        {
            return ROSC.Empty;
        }

        var chunk = buffer[..index];
        var remainder = buffer[index..];
        buffer = remainder;
        return chunk;
    }

    /// <summary>
    /// Skip any delimiters
    /// </summary>
    /// <param name="cs"></param>
    /// <returns></returns>
    public ROSC SkipWhile(params ROSC cs)
    {
        int j = -1;
        for (int i = 0; i < buffer.Length; i++)
        {
            if (!cs.Contains(buffer[i]))
            {
                j = i;
                break;
            }
        }

        //j is now the first index
        if (j < 0)
        {
            //all delimiters
            var res = buffer;
            buffer = ROSC.Empty;
            return res;
        }

        var chunk = buffer[..j];
        var remainder = buffer[j..];
        buffer = remainder;
        return chunk;
    }

    /// <summary>
    /// Take n characters into the out span. If there aren't enouch characters,
    /// return false, with the out span empty
    /// </summary>
    public bool Take(int n, out ROSC r)
    {
        if (buffer.Length < n)
        {
            r = ROSC.Empty;
            return false;
        }

        r = buffer[..n];
        buffer = buffer[n..];
        return true;
    }

    public bool Skip(int n)
    {
        if (buffer.Length < n)
        {
            buffer = [];
            return false;
        }

        buffer = buffer[n..];
        return true;
    }

    public ROSC TakeWhile(Func<char, bool> predicate)
    {
        var n = GetIndex(predicate);
        Take(n, out var res);
        return res;
    }

    readonly int GetIndex(Func<char, bool> predicate)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (!predicate(buffer[i]))
            {
                return i;
            }
        }

        return buffer.Length;
    }
}
