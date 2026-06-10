dotnet publish -c Release -o .\bin\wasmaot -p:RunAOTCompilation=true -p:WasmStripILAfterAOT=true -p:PublishTrimmed=true

explorer .\bin\wasmaot
:: dotnet serve -o