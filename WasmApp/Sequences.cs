using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaApplication2;
using B;
using B.Ux;
using BH.BleuIO;

using static B.ShortColours;
using ROSC = System.ReadOnlySpan<char>;

namespace BtkApp;

static class Sequences
{
    static BleuPort port;

    public static void SetApi(BleuPort x) { port = x; }

    //public static async Task UseRandomAddress(bool isPrivate)
    //{
    //    var r = await Link.Call("_priv", (short)(isPrivate ? 1 : 0));
    //    if (!r.Success) throw new Exception("_adr failed");
    //}

    //public static async Task<string> GetAddress()
    //{
    //    var r = await Link.Call("_adr");
    //    if (!r.Success) throw new Exception("_adr failed");
    //    if (r.R.Length != 3) throw new Exception("badly formatted BLE address response");
    //    var address = $"{r.R[0]:X4}{r.R[1]:X4}{r.R[2]:X4}";
    //    return address;
    //}

    //public static async Task<string> GetPin(bool useRandomNonce)
    //{
    //    var nonce = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

    //    if (useRandomNonce)
    //    {
    //        "Random nonce".Log(green);
    //        Random.Shared.NextBytes(nonce);
    //    }
    //    else
    //    {
    //        "Fixed nonce".Log(green);
    //    }

    //    nonce.ByteArrayToString().Log();
    //    var r = await Link.Bin.Call("gpin", nonce);
    //    return $"{r.R[0]:X2} {r.R[1]:X2} {r.R[2]:X2}";
    //}

    public static async Task SetSecurity(string pin)
    {
        var p = pin.Apply(StringUtils.RemoveGaps);
        await RunCommands(
            "AT+GAPIOCAP=4",
            "AT+SECLVL=4",
            $"AT+SETPASSKEY={p}"
        );
    }

    public static async Task NoSecurity()
    {
        await RunCommands("AT+SECLVL=1");
    }

    static bool IsBondedAddress(ROSC ln, out string address)
    {
        string addr = string.Empty;
        BleuParser.ParseObject(ln, f);
        address = addr;
        return addr != string.Empty;

        void f(ROSC name, ROSC value)
        {
            switch (name)
            {
                case "bondedAddr":
                    addr = value.ToString().Replace(":", "");
                    break;
            }
        }
    }

    public static async Task<string> GetFirstBondedAddress()
    {
        string addr = string.Empty;
        try
        {
            var cmd = $"AT+GETBOND";
            await port.Expect(cmd, x => IsBondedAddress(x, out addr), 1000);
            return addr;

        }
        catch (Exception ex)
        {
            ex.Message.Log(red);
            return addr;
        }
    }

    static readonly Func<string, bool> SecurityLevel4 = ResponseParsers.LineHasValues(
                    ("action", "sec level set"),
                    ("secLvl", "4")
                    );
    //public static async Task ConnectBonded(string addr, int timeoutMs = 8000)
    //{
    //    addr = BleuPacketResponder.AddByteSeparators(addr);

    //    await api.BleuPort.ExpectThrow(
    //       "AT+SECLVL=4",
    //       x => ResponseParsers.IsResult(x, out var res),
    //       1000);

    //    await api.BleuPort.ExpectThrow(
    //        cmd: $"AT+CONNECTBOND={addr}",
    //        recognizer: SecurityLevel4,
    //        timeoutMs);
    //}

    //public static async Task Bond(int timeoutMs = 10000)
    //{
    //    await port.ExpectThrow(
    //        "AT+GAPPAIR=BOND",
    //        x => ResponseParsers.IsBonded(x),
    //        timeoutMs);
    //}

    public static async Task RunCommands(params IEnumerable<string> cmds)
    {
        const int dms = 200;
        //note: this will return quickly if we are already in verbose mode
        //if we are not, it will not recognize the plain text response and
        //will "fail" after the timeout 
        //we don't really check for the result. All we are doing is that if we are already
        //in verbose mode, we don't incur waiting extra time
        foreach (var cmd in cmds)
        {
            await port.ExpectThrow(cmd, x => ResponseParsers.IsResult(x, out var res), dms);
        }
    }

    public static async Task SenCtrlC()
    {
        await port.SendCtrlC();
      //  await port.SendCtrlC();
    }
}
