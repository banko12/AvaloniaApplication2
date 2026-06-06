dotnet publish -c Release -o .\bin\wasm

cd .\bin\wasm\wwwroot
dotnet serve -o