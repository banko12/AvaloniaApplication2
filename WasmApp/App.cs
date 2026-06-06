using Avalonia.Controls;
using System.Threading.Tasks;

namespace AvaloniaApplication2;

/// <summary>
/// To create a custom app, we just need to derive from WasmApp and implement the BuildUi method
/// </summary>
public sealed class App : WasmApp
{
    public override Task BuildUi(ContentControl container) => AppUi.Build(container);
}
