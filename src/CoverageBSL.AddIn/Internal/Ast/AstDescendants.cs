using OneScript.Language.SyntaxAnalysis.AstNodes;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal.Ast
{
    /// <summary>
    /// Обход всех потомков AST (аналог <c>Trees.getDescendants</c> в Coverage41C).
    /// </summary>
    internal static class AstDescendants
    {
        public static IEnumerable<BslSyntaxNode> Enumerate(BslSyntaxNode root)
        {
            if (root == null)
            {
                yield break;
            }

            yield return root;
            foreach (var child in root.Children)
            {
                foreach (var descendant in Enumerate(child))
                {
                    yield return descendant;
                }
            }
        }
    }
}
