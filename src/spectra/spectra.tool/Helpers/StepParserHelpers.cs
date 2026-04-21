using System.Text.RegularExpressions;

namespace spectra.tool.Helpers;

internal static class StepParserHelpers
{
    internal static readonly Regex StepComment = new(
        @"//\s*\[\s*step\s*:\s*(?<stepname>(?!\s+\])[^\]]+?)\s*\]\s*(?<stepdesc>.*)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
}