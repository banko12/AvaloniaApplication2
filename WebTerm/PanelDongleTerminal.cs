using B;
//using B.Term;
//using B.Term.Highlighting;
using B.Ux;
//using B.Wpf;
using B.NA.Ux3.Facade;
//using ICSharpCode.AvalonEdit.Highlighting;
using System.Text.RegularExpressions;
//using System.Windows.Controls;
using static B.ShortColours;
using Avalonia.Controls;
using BHA.Terminal;
using AvaloniaEdit.Highlighting;
using BHA.Terminal.Highlighting;

namespace BtkApp;

class PanelDongleTerminal : UiGroup<Border>
{
    public Terminal Terminal { get; private set; }
    public PanelDongleTerminal()
    {
        static HighlightingSpan line(string start, Colour c) => HSpan.LineStartingWith(start)
                                                                .WithColor(c.ToHcolor().Italic())
                                                                .WithStartEndColor(clear.ToHcolor());

        var highlighter = BleuHighlighter.Get();
        line("-g", green.Darken(0.3)).AddTo(highlighter);
        line("-r", red).AddTo(highlighter);

        var (header,  btnCopy, btnClear) = ui.HeaderWithCopyAndClear("Dongle");
        header.PlaceInside(root);

        Terminal = new Terminal()
                    .WithHighlighter(highlighter)
                    .Margin(10).PlaceInside(header);

        btnClear.WithClick(() => Terminal.Clear());
        btnCopy.WithTooltip("Copy").WithClick(() => Terminal.Copy());

        Terminal.ShowLineNumbers(false, false);
    }
}

static class BleuHighlighter
{
    public static IHighlightingDefinition Get()
    {
        var highlighter = new BasicHighlighter();

        var commentColor = green.Darken(0.2).Regular().WithBackground(gray.Fade(0.93));
        var okColor = green.Darken(0.3).Bold();
        var valColor = blue.Nudge(90, 1, 0.7).Bold();
        var braceColor = blue.Nudge(0, 0.5, 1).Regular();
        var delimColor = purple.Nudge(30, 1.5, 1.7).Bold();

        var valSpan = HSpan.New(":(?=[^{])", "(?=[,}])")
            .WithColor(valColor)
            .WithStartEndInclude(false)
            .WithRules(
                new Regex("pair completed").WithColor(black.Bold()),
                new Regex("false").WithColor(red.Bold()),
                new Regex("true").WithColor(green.Bold())
            );

        var nestedBlock = HSpan.New("{", "}")
        .WithColor(gray.Regular())
        .WithStartEndColor(braceColor)
        .WithSpans(valSpan)
        .WithRules(new Regex(":").WithColor(delimColor))
        ;

        //top block
        //start of line,possible empty space, followed by open curly
        //close curly , followed by possible empty space , followed by end of line
        HSpan.New("^\\s*{", "}\\s*$")
        .WithColor(gray.Regular())
        .WithStartEndColor(braceColor)
        .WithSpans(nestedBlock)
        .WithRules(KnownRegex.Words("ok").WithColor(okColor))
        .AddTo(highlighter);

        //an AT+ line span. Starts with AT+ . Inside, we colorize the equal sign, which separates parameters
        HSpan.LineStartingWith("AT")
        .WithColor(blue.Bold())
        .WithRules(new Regex("=").WithColor(delimColor))
        .AddTo(highlighter);

        //a comment line. Inside the comment line we highlight hex numbers - just for illustration
        HSpan.LineStartingWith("//")
        .WithColor(commentColor)
        .AddTo(highlighter);

        return highlighter;
    }
}
