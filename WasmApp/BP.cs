using B;
using B.Ux;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using static B.ShortColours;

namespace AvaloniaApplication2;

sealed class BP:IDisposable
{
    private readonly DisposeManager disp;
    public void Dispose() => disp.Dispose();

    private readonly Subject<string> outgoing;


    private readonly Subject<string> incomingLines;
    public IObservable<string> IncomingLines => incomingLines;

    public IObservable<string> Outgoing => outgoing;

    static readonly char[] NL = ['\r', '\n'];
    static readonly byte[] terminator = "\r"u8.ToArray();

    private string buffer = "";  //this is our accumulator



    public BP()
    {
        disp = new DisposeManager();
        outgoing = new Subject<string>().DisposedBy(disp);
        incomingLines = new Subject<string>().DisposedBy(disp);


        WebSerial.Current.DataReceived += OnDataReceived;

        disp.Push(() =>
        {
            WebSerial.Current.DataReceived -= OnDataReceived;
        });


    }


    public async Task Close()
    {
        try
        {
            await WebSerial.Current.CloseAsync();
        }
        catch (Exception ex)
        {
            ex.Message.Log(red);
        }
    }

    public async Task<bool> Open()
    {

        var x = await WebSerial.Current.OpenAsync(115200);

        return x;
        //if (x)
        //{
        //    "Port opened successfully".Log(green);
        //}
        //else
        //{
        //    "Failed to open port".Log(red);
        //    return;
        //}



       



        //  port = new SerialPort(Port, Baud);
        // port.DtrEnable = true;  //IMPORTANT: required since firmware version 2.6.0 
        // port.Open();
        // port.DataReceived += OnDataReceived;
    }

    void OnDataReceived(byte[] bytes)
    {
        string textChunk = Encoding.ASCII.GetString(bytes);
        $"{textChunk}".Log();
        Accumulate(textChunk);
    }


    private void Accumulate(string s)
    {
#if UARTDEBUG
            var s1= s.Replace('\r', 'Ď');
            s1 = s1.Replace('\n', 'Ă');
            $"-> '{s1}'".Dump();
#endif

        buffer += s;

        //take out lines from the buffer while possible
        while (true)
        {
            var ind = buffer.IndexOfAny(NL);
            if (ind < 0) return;

            var line = buffer[..(ind + 1)];
            var remainingLength = buffer.Length - line.Length;
            buffer = remainingLength == 0 ? "" : buffer.Substring(ind + 1, remainingLength);

            if (string.IsNullOrWhiteSpace(line)) continue;

            line = line.Trim(NL);
            incomingLines.OnNext(line);
        }
    }


    readonly byte[] ctrlc = [0x03];
    public async Task  SendCtrlC()
    {
        await WebSerial.Current.WriteAsync(ctrlc);
    }


    public async Task Send(string data)
    {
        try
        {
            data = data.TrimEnd(NL);
            outgoing.OnNext(data);
            var bytes = Encoding.UTF8.GetBytes(data);
            bytes = [.. bytes, .. terminator];

            await WebSerial.Current.WriteAsync(bytes);
            //port.Write(bytes, 0, bytes.Length);
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.Message);
        }

    }
}