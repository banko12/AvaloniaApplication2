import { dotnet } from './_framework/dotnet.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = dotnetRuntime.getConfig();

//get the exports from the main assembly, which includes our WebSerialInterop class and its methods
const exports = await dotnetRuntime.getAssemblyExports(config.mainAssemblyName); 

// Bind it to globalThis using the clean, single-word variable
globalThis.DotNetSerialListener = {
    receiveBytes: (byteArray) =>
                    exports
                    .BH.Experimental.WebSerial  //the namespace where our WebSerialInterop class lives
                    .WebSerialInterop
                    .ReceiveBytes(byteArray)
};



await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
