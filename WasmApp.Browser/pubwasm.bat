dotnet publish -c Release -o .\bin\wasm

explorer .\bin\wasm

cd .\bin\wasm\wwwroot
dotnet serve -o