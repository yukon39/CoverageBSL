using OneScript.Language.SyntaxAnalysis.AstNodes;
using OneScript.Sources;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    /// <summary>
    /// Извлекает строки к покрытию по правилам Coverage41C (<c>LinesToCoverage</c>):
    /// сбор кандидатов → отбор по правилам → номер строки.
    /// </summary>
    internal static class AstCoverableLinesCollector
    {
        public static SortedSet<int> Collect(ModuleNode module, SourceCode source)
        {
            var lines = new SortedSet<int>();
            void AddLine(int line)
            {
                if (line > 0)
                {
                    lines.Add(line);
                }
            }

            foreach (var lexem in SourceLexemeEnumerator.Enumerate(source))
            {
                if (!CoverableLinesRules.TerminalTokens.Contains(lexem.Token))
                {
                    continue;
                }

                AddLine(lexem.Location.LineNumber);
            }

            if (module == null)
            {
                return lines;
            }

            foreach (var node in AstDescendants.Enumerate(module))
            {
                if (!CoverableLinesRules.NodeKinds.Contains(node.Kind))
                {
                    continue;
                }

                if (!GlobalCallsFilter.ShouldIncludeNode(node))
                {
                    continue;
                }

                AddLine(CoverableLineResolver.GetLine(node));
            }

            return lines;
        }
    }
}
