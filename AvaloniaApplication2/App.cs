using Avalonia.Controls;
using B.Ux;
using System.Threading.Tasks;

namespace AvaloniaApplication2;

public class App : WasmApp
{
    public override Task BuildUi(ContentControl container) => AppUi.Build(container);
}
