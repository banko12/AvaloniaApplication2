using System;
using System.Linq;
using System.Reactive.Linq;

namespace BtkApp;

static class StringUtils
{
    public static string RemoveGaps(string src) =>
        string.Create(src.Length - src.Count(c => c == ' '), src, (span, x) =>
        {
            for (int i = 0, j = 0; i < x.Length; i++)
            {
                if (x[i] != ' ')
                {
                    span[j++] = x[i];
                }
            }
        });
}
