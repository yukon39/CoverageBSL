using OneScript.Language.SyntaxAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    /// <summary>
    /// Определение номера строки для покрытия — аналог <c>getLine</c> в Coverage41C.
    /// </summary>
    internal static class CoverableLineResolver
    {
        public static int GetLine(BslSyntaxNode node) => node.Kind switch
        {
            NodeKind.Assignment => GetAssignmentLine(node),
            NodeKind.MethodCall => GetMethodCallLine(node),
            NodeKind.ForEachLoop => GetForEachLoopLine((ForEachLoopNode)node),
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

        private static int GetForEachLoopLine(ForEachLoopNode forEachLoop)
        {
            var line = forEachLoop.Location.LineNumber;
            if (forEachLoop.IteratorVariable != null
                && forEachLoop.IteratorVariable.Location.LineNumber == line)
            {
                return line;
            }

            // In split "Для Каждого" forms the AST location can point to the line
            // after "Для", while Coverage41C expects the loop header line itself.
            return line > 1 ? line - 1 : line;
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
