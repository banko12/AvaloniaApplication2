using ROSC = System.ReadOnlySpan<char>;

namespace BH.BleuIO;

public static class HexString
{

    private const string hexdigit = "0123456789ABCDEF";
    
    public static string FromBytes(byte[] bytes) => string.Create(
                length: 2 * bytes.Length,
                state: bytes,
                action: static (span, x) =>
                {
                    for (int i = 0, j = 0; i < x.Length; i++)
                    {
                        span[j++] = hexdigit[x[i] >> 4];
                        span[j++] = hexdigit[x[i] & 0b1111];
                    }
                });

    public static int ToInt(ROSC sp)
    {
        int number = 0;
        foreach (char c in sp)
        {
            var d = FromHexDigit(c);
             if (d < 0) return -1;
            number = (number << 4) + d;
        }

        return number;
    }


    public static byte[] ToBytes(ROSC x)
    {
        var n = x.Length / 2;
        var buf = new byte[n];

        for (int i = 0, a = 0; i < n; i++, a += 2)
        {
            var d1=FromHexDigit(x[a]);
            if(d1<0) return [];
            var d0=FromHexDigit(x[a+1]);
            if(d0<0) return [];

            buf[i] = (byte)((d1 << 4) + d0);
        }

        return buf;
    }

    public static int FromHexDigit(char c) => c switch
    {
        '0' => 0,
        '1' => 1,
        '2' => 2,
        '3' => 3,
        '4' => 4,
        '5' => 5,
        '6' => 6,
        '7' => 7,
        '8' => 8,
        '9' => 9,

        'a' => 10,
        'A' => 10,

        'b' => 11,
        'B' => 11,

        'c' => 12,
        'C' => 12,

        'd' => 13,
        'D' => 13,

        'e' => 14,
        'E' => 14,

        'f' => 15,
        'F' => 15,
        _ => -1  // throw new Exception($"Bad hex digit: {c}")
    };

}
