namespace QuickGraph.Graphviz.Dot
{
    using System.Text.RegularExpressions;
    using System.Diagnostics.Contracts;

    public sealed class GraphvizRecordEscaper
    {
        private readonly Regex escapeRegExp = new Regex("(?<Eol>\\n)|(?<Common>\\[|\\]|\\||<|>|\"| )", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Multiline);

        public string Escape(string text)
        {
            Contract.Requires(text != null);

            return escapeRegExp.Replace(text, new MatchEvaluator(MatchEvaluator));
        }

        public static string MatchEvaluator(Match m)
        {
            if (m.Groups["Common"] != null)
            {
                return string.Format(@"\{0}", m.Value);
            }
            return @"\n";
        }
    }
}

