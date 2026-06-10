using System;
using System.Collections.Generic;
using System.Linq;
using ROSC = System.ReadOnlySpan<char>;

namespace BH.BleuIO;

/// <summary>
/// A collection of parsers for the most often used responses, suc as Connect, notifications, etc
/// </summary>
public static class ResponseParsers
{
    // {777:"0000","evt":{"handle":"000c","hex":"0x300A","len":2}}
    public static bool IsNotification(ROSC ln, out int handle, out byte[] buf)
    {
        byte[] r = [];
        int h = 0;

        BleuParser.ParseObject(ln, f);

        handle = h;
        buf = r;

        return (h > 0 && buf.Length > 0);

        void f(ROSC name, ROSC value)
        {
            switch (name)
            {
                case "handle": h = HexString.ToInt(value); break;
                case "hex":
                    if (value.Length <= 2) r =[];
                    else r = HexString.ToBytes(value[2..]);
                    break;
            }
        }
    }

    /// <summary>
    /// When parsing the response to ATV1, the dongle could have been in ATV0
    /// so we need to be prepared to parse either a verbose or non-verbose reply.
    /// This is the only command where we concern ourselves with a non-verboase reply. 
    /// Switching to verbose mode is done once, at dongle initialization.
    /// All other commands are executed in verbose mode.
    /// </summary>
    public static bool IsVerbose(ROSC ln, out int res)
    {
        bool hasA = false; int r = 0;
        BleuParser.ParseObject(ln, f);
        res = r;
        return hasA;

        void f(ROSC name, ROSC value)
        {
            switch (name)
            {
                case "A": 
                    hasA = true; 
                    break;
                case "err": 
                    r = HexString.ToInt(value); 
                    break;
            }
        }
    }

    /// <summary>
    /// Check if the response is a result , i.e of the kind 
    /// {"A":74,"err":0,"errMsg":"ok"}
    /// </summary>
    public static bool IsResult(ROSC ln, out int res)
    {
        bool hasA = false; int r = 0;
        BleuParser.ParseObject(ln, f);
        res = r;
        return hasA;

        void f(ROSC name, ROSC value)
        {
            switch (name)
            {
                case "A": hasA = true; break;
                case "err": r = HexString.ToInt(value); break;
                  
            }
        }
    }

    public static bool IsBonded(ROSC ln)
    {
        bool pairCompleted = false, bonded=false, mitm=false;

        BleuParser.ParseObject(ln, f);

        return pairCompleted && bonded;

        void f(ROSC name, ROSC value)
        {
            if (name is "action" && value is "pair completed") pairCompleted = true;
            if (name is "bond" && value is "true") bonded = true;
            if (name is "mitm" && value is "true") pairCompleted = true;
        }

    }

    public static Func<string,bool> LineHasValues(params (string name, string value)[] pattern) 
    {

        return MatchFunction;

        bool MatchFunction(string ln)
        {
            bool[] res = new bool[pattern.Length];
            for (int i = 0; i < pattern.Length; i++) res[i]=false;
            bool allMatch=true;

            BleuParser.ParseObject(ln, f);

            for (int i = 0; i < pattern.Length; i++)
            {
                if( res[i] == false ) return false;
            }

            return true;

            //called for each (name,value) pair in the string
            void f(ROSC name, ROSC value)
            {

                for (int i = 0; i < pattern.Length; i++)
                {
                    if (name.SequenceEqual(pattern[i].name) && value.SequenceEqual(pattern[i].value)) res[i] = true;
                }
            }
        }

    }

    public static bool IsSecureConnected(ROSC ln)
    {
        bool secLevelSet = false;
        BleuParser.ParseObject(ln, f);
        return secLevelSet;

        void f(ROSC name, ROSC value)
        {
            //{256:"0000","evt":{"action":"sec level set","secLvl":4}}
            if (name is "secLvl" && value is "4") secLevelSet =true;
        }
    }

    public static bool IsConnected(ROSC ln, out bool wasAlreadyConnected)
    {
        bool res = false, existing=false ;
        BleuParser.ParseObject(ln, f);

        wasAlreadyConnected = existing;
        return res;

        void f(ROSC name, ROSC value)
        {
            //{256:"0000","evt":{"action":"connected","addr":"f5:ff:ff:22:11:94","CImin":18,"CImax":18}}
            if (name is "action" && value is "connected") res = true;
           
            //{"A":91,"err":2,"errMsg":"Already connected to: F5:FF:FF:22:11:94"}
            if (name is "err" && value is "2") {res = true;  existing=true; }        
        }
    }
}
