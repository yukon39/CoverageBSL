using OneScript.Language.Extensions;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;
using OneScript.Sources;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    /// <summary>
    /// Извлекает строки к покрытию из AST модуля по правилам Coverage41C (<c>LinesToCoverage</c>).
    /// </summary>
    internal static class AstCoverableLinesCollector
    {
        public static SortedSet<int> Collect(ModuleNode module, SourceCode resolvedSource)
        {
            var lines = new SortedSet<int>();
            if (module != null)
            {
                var walker = new CoverableLinesWalker();
                walker.Visit(module);
                lines = walker.Lines;
            }

            CollectStructureKeywordLines(resolvedSource, lines);
            return lines;
        }

        /// <summary>
        /// Ключевые слова структуры (Для, Цикл, КонецЦикла, …) — как terminal tokenTypes в Coverage41C.
        /// </summary>
        private static void CollectStructureKeywordLines(SourceCode source, SortedSet<int> lines)
        {
            var lexer = new DefaultLexer { Iterator = source.CreateIterator() };
            while (true)
            {
                var lexem = lexer.NextLexem();
                if (lexem.Type == LexemType.EndOfText)
                {
                    break;
                }

                if (!IsStructureKeyword(lexem.Token))
                {
                    continue;
                }

                var line = lexem.Location.LineNumber;
                if (line > 0)
                {
                    lines.Add(line);
                }
            }
        }

        private static bool IsStructureKeyword(Token token) =>
            token == Token.For
            || token == Token.Loop
            || token == Token.EndLoop
            || token == Token.EndProcedure
            || token == Token.EndFunction
            || token == Token.EndIf
            || token == Token.EndTry;

        private sealed class CoverableLinesWalker : BslSyntaxWalker
        {
            public SortedSet<int> Lines { get; } = new SortedSet<int>();

            private void AddLine(int line)
            {
                if (line > 0)
                {
                    Lines.Add(line);
                }
            }

            private void AddStartLine(BslSyntaxNode node) => AddLine(node.Location.LineNumber);

            private void VisitWithStartLine(BslSyntaxNode node, Action visitChildren)
            {
                AddStartLine(node);
                visitChildren();
            }

            protected override void VisitAssignment(BslSyntaxNode assignment)
            {
                AddLine(GetAssignmentLine(assignment));
                base.VisitAssignment(assignment);
            }

            protected override void VisitGlobalFunctionCall(CallNode node) => VisitGlobalCall(node);

            protected override void VisitGlobalProcedureCall(CallNode node) => VisitGlobalCall(node);

            protected override void VisitObjectFunctionCall(BslSyntaxNode node)
            {
                TryAddMethodCall(node);
                base.VisitObjectFunctionCall(node);
            }

            protected override void VisitObjectProcedureCall(BslSyntaxNode node)
            {
                TryAddMethodCall(node);
                base.VisitObjectProcedureCall(node);
            }

            protected override void VisitBinaryOperation(BinaryOperationNode node)
            {
                VisitExpression(node.Children[0]);
                VisitExpression(node.Children[1]);
            }

            protected override void VisitUnaryOperation(UnaryOperationNode node)
            {
                VisitExpression(node.Children[0]);
            }

            protected override void VisitTernaryOperation(BslSyntaxNode node)
            {
                VisitExpression(node.Children[0]);
                VisitExpression(node.Children[1]);
                VisitExpression(node.Children[2]);
            }

            protected override void VisitIfNode(ConditionNode node) =>
                VisitWithStartLine(node, () => base.VisitIfNode(node));

            protected override void VisitElseIfNode(ConditionNode node) =>
                VisitWithStartLine(node, () => base.VisitElseIfNode(node));

            protected override void VisitWhileNode(WhileLoopNode node) =>
                VisitWithStartLine(node, () => base.VisitWhileNode(node));

            protected override void VisitForEachLoopNode(ForEachLoopNode node)
            {
                AddLine(GetForEachLoopLine(node));
                base.VisitForEachLoopNode(node);
            }

            protected override void VisitForLoopNode(ForLoopNode node) => base.VisitForLoopNode(node);

            protected override void VisitBreakNode(LineMarkerNode node) => AddStartLine(node);

            protected override void VisitContinueNode(LineMarkerNode node) => AddStartLine(node);

            protected override void VisitReturnNode(BslSyntaxNode node) =>
                VisitWithStartLine(node, () => base.VisitReturnNode(node));

            protected override void VisitRaiseNode(BslSyntaxNode node) =>
                VisitWithStartLine(node, () => base.VisitRaiseNode(node));

            protected override void VisitGotoNode(NonTerminalNode node) => AddStartLine(node);

            protected override void VisitExecuteStatement(BslSyntaxNode node) =>
                VisitWithStartLine(node, () => base.VisitExecuteStatement(node));

            protected override void VisitHandlerOperation(BslSyntaxNode node)
            {
                AddStartLine(node);
                foreach (var child in node.Children)
                {
                    VisitExpression(child);
                }
            }

            protected override void VisitForInitializer(BslSyntaxNode node)
            {
                if (node.Children.Count >= 2)
                {
                    VisitForLoopInitialValue(node.Children[1]);
                }
            }

            private void VisitGlobalCall(CallNode node)
            {
                TryAddGlobalCall(node);
                VisitCallArguments(node);
            }

            private void TryAddGlobalCall(CallNode node)
            {
                if (node.Identifier != null && GlobalCallsFilter.ShouldInclude(node.Identifier.GetIdentifier()))
                {
                    AddLine(node.Identifier.Location.LineNumber);
                }
            }

            private void TryAddMethodCall(BslSyntaxNode node)
            {
                if (node.Kind == NodeKind.MethodCall)
                {
                    AddLine(GetMethodCallLine(node));
                }
            }

            private void VisitCallArguments(CallNode node)
            {
                if (node.ArgumentList == null)
                {
                    return;
                }

                foreach (var argument in node.ArgumentList.Children)
                {
                    if (argument.Children.Count > 0)
                    {
                        VisitExpression(argument.Children[0]);
                    }
                }
            }

            private static int GetAssignmentLine(BslSyntaxNode assignment)
            {
                if (assignment.Children.Count == 0)
                {
                    return assignment.Location.LineNumber;
                }

                var leftmost = FindLeftmostIdentifier(assignment.Children[0]);
                return leftmost?.Location.LineNumber ?? assignment.Children[0].Location.LineNumber;
            }

            private static int GetForEachLoopLine(ForEachLoopNode forEachLoop)
            {
                var line = forEachLoop.Location.LineNumber;
                if (forEachLoop.IteratorVariable != null
                    && forEachLoop.IteratorVariable.Location.LineNumber == line)
                {
                    return line;
                }

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
}
