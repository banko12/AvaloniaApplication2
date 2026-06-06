using AvaloniaEdit.Highlighting;
using B;
using B.Ux;
using BHA.Terminal.Highlighting;
using System.Text.RegularExpressions;
using static B.ShortColours;

namespace BtkApp;

static class Highlighter
{
    public static IHighlightingDefinition Get()
    {
        var highlighter = new BasicHighlighter();

        var commentColor = green.Darken(0.2).Regular().WithBackground(gray.Fade(0.93));
        var okColor = green.Darken(0.3).Bold();
        var valColor = blue.Nudge(90, 1, 0.7).Bold();
        var braceColor = blue.Nudge(0, 0.5, 1).Regular();
        var delimColor = purple.Nudge(30, 1.5, 1.7).Bold();

        var okRule = KnownRegex.Words("ok").WithColor(okColor);

        var valSpan = HSpan.New(":(?=[^{])", "(?=[,}])")
            .WithColor(valColor)
            .WithStartEndInclude(false)
            .WithRules(
                new Regex("pair completed").WithColor(black.Bold()),
                new Regex("false").WithColor(red.Bold()),
                new Regex("true").WithColor(green.Bold())
            );

        var nestedBlock = HSpan.New("{", "}")
        .WithColor(gray.ToHcolor())
        .WithStartEndColor(braceColor)
        .WithSpans(valSpan)
        .WithRules(new Regex(":").WithColor(delimColor))
        ;

        static string PositiveLookback(string s) => $"(?<={s}";

        //  (?<="[^,}]+)
        //  (?!:)
        //creates a regex to match the value of a given parameter
        // in "name":"value"

        var paramNameColor = blue.Nudge(0, 0.5, 0.7).Italic();

        var paramNameSpan =
        HSpan.New(
            """
        (?<=[^:])"(?=[a-zA-Z0-9]+")
        """,
            """
        "(?=\s*:)
        """
        )
        .WithColor(paramNameColor)
        .WithRules(KnownRegex.Words("passkey").WithColor(paramNameColor.Clone().Bold()))
        .WithStartEndColor(green.Regular(), red.Regular()) //for debugging
        ;

        static Regex NamedParameter(string name)
        {
            //note: this pattern uses lookbehind, and JavaScript
            //doesn't support lookbehind, so tools like Jex report an error
            //https://regex101.com/ in PCRE works
            //https://www.debuggex.com/

            var s = $"""
             (?<="{name}":")[^"]+
             """;

            return new Regex(s);
        }

        var pk = NamedParameter("passkey").WithColor(orange.Bold());

        //top block
        //start: of line,possible empty space, followed by open curly
        //end : close curly , followed by possible empty space , followed by end of line
        HSpan.New("^\\s*{", "}\\s*$")
        .WithColor(gray.Regular())
        .WithStartEndColor(braceColor)
        .WithSpans(nestedBlock, paramNameSpan)
        .WithRules(okRule, pk)
        .AddTo(highlighter);

        //an AT line span. Starts with AT. Inside, we colorize the equal sign, which separates parameters
        HSpan.LineStartingWith("AT")
        .WithColor(blue.Bold())
        .WithRules(new Regex("=").WithColor(delimColor))
        .AddTo(highlighter);

        //comment span
        HSpan.LineStartingWith("//")
       .WithColor(commentColor)
       .AddTo(highlighter);

        return highlighter;
    }

}

