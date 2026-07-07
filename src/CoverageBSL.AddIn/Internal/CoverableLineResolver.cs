using OneScript.Language.SyntaxAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;
using OneScript.Sources;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    /// <summary>
    /// Определение номера строки для покрытия — аналог <c>getLine</c> в Coverage41C.
    /// </summary>
    internal static class CoverableLineResolver
    {
        public static int GetLine(BslSyntaxNode node, SourceCode source) => node.Kind switch
        {
            NodeKind.Assignment => GetAssignmentLine(node),
            NodeKind.MethodCall => GetMethodCallLine(node),
            NodeKind.ForEachLoop => GetForEachLoopLine((ForEachLoopNode)node, source),
            NodeKind.GlobalCall => GetGlobalCallLine((CallNode)node),
            _ => node.Location.LineNumber,
        };

        private static int GetAssignmentLine(BslSyntaxNode assignment)
        {
            if (assignment.Children.Count == 0)
            {
                return assignment.Location.LineNumber;
            }

            var leftmost = FindLeftmostIdentifier(assignment.Children[0]);
            return leftmost?.Location.LineNumber ?? assignment.Children[0].Location.LineNumber;
        }

        private static int GetGlobalCallLine(CallNode call) =>
            call.Identifier?.Location.LineNumber ?? 0;

        private static int GetForEachLoopLine(ForEachLoopNode forEachLoop, SourceCode source)
        {
            var line = forEachLoop.Location.LineNumber;
            return FindForKeywordLine(source, line) ?? line;
        }

        private static int? FindForKeywordLine(SourceCode source, int eachLine)
        {
            int? forLine = null;

            foreach (var lexem in SourceLexemeEnumerator.Enumerate(source))
            {
                if (lexem.Location.LineNumber > eachLine)
                {
                    break;
                }

                if (lexem.Token == OneScript.Language.LexicalAnalysis.Token.For)
                {
                    forLine = lexem.Location.LineNumber;
                    continue;
                }

                if (lexem.Token == OneScript.Language.LexicalAnalysis.Token.Each
                    && lexem.Location.LineNumber == eachLine)
                {
                    return forLine;
                }
            }

            return null;
        }

        private static int GetMethodCallLine(BslSyntaxNode methodCallNode)
        {
            var chainRoot = methodCallNode;
            while (chainRoot.Parent?.Kind == NodeKind.DereferenceOperation)
            {
                chainRoot = chainRoot.Parent;
            }

            var identifier = FindLeftmostIdentifier(chainRoot);
            return identifier?.Location.LineNumber ?? methodCallNode.Location.LineNumber;
        }

        private static TerminalNode FindLeftmostIdentifier(BslSyntaxNode node)
        {
            if (node is TerminalNode terminal && terminal.Kind == NodeKind.Identifier)
            {
                return terminal;
            }

            foreach (var child in node.Children)
            {
                var found = FindLeftmostIdentifier(child);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}
