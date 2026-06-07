using System;
using ROSC = System.ReadOnlySpan<char>;


namespace BH.BleuIO;

public delegate void VParser(ROSC name, ROSC value);

public static class BleuParser
{
    static readonly char[] ws = [' ', '\t'];
    static readonly char[] coma = [','];
    
    
    /// <summary>
    /// Parses the line, calling the delegate for each leaf (name,value) pair
    /// </summary>
    public static bool ParseObject(ROSC line, VParser vp)
    {
        line = line.Trim(ws);
        if (line is ['{', .. var content, '}'])
        {
            return ParseObjectContent(content, vp);
        }

        return false;
    }



    public static bool ParseObjectContent(ROSC p, VParser vp)
    {
        var sp = new SpanStringParser(p);
        ROSC valspan;

        while (true)
        {
            if (sp.IsEmpty()) break;

            var b = sp.TakeName(out var name);
            if (!b)
            {
               // throw new Exception("expected a name");
                return false;
                
            }

            //skip to the first non-white space character of the value
            b = sp.SkipToValue();
            if (!b)
            {
                return false;
                //throw new Exception("expected ':' ; didn't get it");
            }
 


            if (sp.buffer[0] == '{')
            {
                //The value is an object. Descend into it
                sp.Skip(1);  //skip the {
                var ind = FindMatchingBrace(sp.buffer);

                if (ind < 0)
                {
                    throw new Exception($"No matching brace");
                }

                sp.Take(ind, out valspan);

                ParseObjectContent(valspan, vp);

                sp.SkipToDelimiter(coma);  //skip past the }, to the next name value pair
            }
            else
            {
                //the value is not an object
                if (sp.buffer[0] == '"')
                {
                    if (!sp.TakeQuotedName(out var dd))
                    {
                        //the value started with " but the rest of it couldn't be read
                        //throw new Exception("No matching quote");
                        
                        return false;
                    };
                    
                    vp(name, dd);
                    sp.SkipToDelimiter(coma);
                }
                else
                {
                    valspan = sp.TakeToDelimiter(coma);
                    valspan = valspan.Trim(ws);
                    vp(name, valspan);
                }
            }
        }
    
    
        return true;
    }



    /// <summary>
    /// Called after encoutering an open brace {  with the argument equal to the remainder of the string
    /// after the opening brace. The string may contain other opening braces. Returns the index of the 
    /// matching closing brace
    /// </summary>
    private static int FindMatchingBrace(ROSC sp)
    {
        int nbraces = 1;
        for (int i = 0; i < sp.Length; i++)
        {
            if (sp[i] == '{')
            {
                nbraces++;
                continue;
            }

            if (sp[i] == '}')
            {
                nbraces--;
                if (nbraces == 0) return i;
                else continue;
            }
        }

        return -1;
    }


    static bool TakeQuotedName(this ref SpanStringParser sp, out ROSC name)
    {
        name = ROSC.Empty;

        if (!sp.Take(1, out var r))
        {
            //  "input is white space only / empty".Dump();
            return false;
        }

        if (r[0] != '"')
        {
            //  "name not starting with quote".Dump();
            return false;
        }

        var nn = sp.TakeUntil('"');

        if (!sp.Take(1, out r))
        {
            //  "name not closed by quote".Dump();
            return false;
        };

        name = nn;
        return true;
    }

    static bool TakeUnquotedName(this ref SpanStringParser sp, out ROSC name)
    {
        var n = sp.TakeUntil(':');
        name = n.Trim(ws);
        return true;
    }



    public static bool TakeName(this ref SpanStringParser sp, out ROSC name)
    {

        sp.SkipWhile(ws);

        if (sp.buffer[0] == '"')
        {
            return sp.TakeQuotedName(out name);
        }
        else
        {
            return sp.TakeUnquotedName(out name);
        }

    }


    public static bool SkipToValue(this ref SpanStringParser sp)
    {
        //eat white space up to the expected :
        sp.SkipWhile(ws);

        if (!sp.Take(1, out var r))
        {
            //   "only white looking for :".Dump();
            return false;
        }

        if (r[0] != ':')
        {
            //    "expected ':' ".Dump();
            return false;
        }

        //eat white space after the  ':'
        sp.SkipWhile(ws);

        return true;
    }

}