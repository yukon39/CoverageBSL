using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal.Ast
{
    /// <summary>
    /// Явные правила покрытия — аналог <c>contexts</c> и <c>tokenTypes</c> в Coverage41C <c>LinesToCoverage</c>.
    /// </summary>
    internal static class CoverableLinesRules
    {
        /// <summary>
        /// Терминальные токены (структурные ключевые слова), как <c>tokenTypes</c> в Coverage41C.
        /// <c>Token.For</c> соответствует <c>ForStatementContext</c> (строка «Для»), не <c>ForLoop</c> AST.
        /// </summary>
        public static readonly HashSet<Token> TerminalTokens = new HashSet<Token>
        {
            Token.For,
            Token.Loop,
            Token.EndLoop,
            Token.EndProcedure,
            Token.EndFunction,
            Token.EndIf,
            Token.EndTry,
        };

        /// <summary>
        /// Узлы AST операторов, как <c>contexts</c> в Coverage41C.
        /// </summary>
        public static readonly HashSet<NodeKind> NodeKinds = new HashSet<NodeKind>
        {
            NodeKind.Assignment,
            NodeKind.GlobalCall,
            NodeKind.MethodCall,
            NodeKind.Goto,
            NodeKind.ReturnStatement,
            NodeKind.BreakStatement,
            NodeKind.ContinueStatement,
            NodeKind.Condition,
            NodeKind.RaiseException,
            NodeKind.ForEachLoop,
            NodeKind.WhileLoop,
            NodeKind.ExecuteStatement,
            NodeKind.AddHandler,
            NodeKind.RemoveHandler,
        };
    }
}
