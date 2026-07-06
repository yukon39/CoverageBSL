using OneScript.Language.SyntaxAnalysis;
using OneScript.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    /// <summary>
    /// Разворачивает директивы препроцессора в тексте модуля, сохраняя номера строк
    /// (как <c>BSLTokenizer</c> в bsl-parser: директивные строки и исключённые ветки — пустые).
    /// </summary>
    internal static class PreprocessorSourceResolver
    {
        public static SourceCode Resolve(SourceCode source, ConditionalDirectiveHandler conditional)
        {
            var lines = source.GetSourceCode()
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace('\r', '\n')
                .Split('\n');

            var output = new string[lines.Length];
            var state = new ConditionalState(conditional);

            for (var i = 0; i < lines.Length; i++)
            {
                if (TryHandleDirective(lines[i], state))
                {
                    output[i] = string.Empty;
                }
                else
                {
                    output[i] = state.IsLineActive() ? lines[i] : string.Empty;
                }
            }

            return SourceCodeBuilder.Create()
                .FromString(string.Join("\r\n", output))
                .WithName(source.Name)
                .Build();
        }

        private static bool TryHandleDirective(string line, ConditionalState state)
        {
            var trimmed = line.TrimStart();
            if (!trimmed.StartsWith("#", StringComparison.Ordinal))
            {
                return false;
            }

            var directive = TrimDirectiveTerminator(trimmed.Substring(1).TrimStart());
            if (directive.Length == 0)
            {
                return false;
            }

            if (StartsWithKeyword(directive, "Область", "Region")
                || StartsWithKeyword(directive, "КонецОбласти", "EndRegion"))
            {
                return true;
            }

            if (StartsWithKeyword(directive, "Если", "If"))
            {
                state.EnterIf(state.Evaluate(ExtractCondition(directive, "Если", "If")));
                return true;
            }

            if (StartsWithKeyword(directive, "ИначеЕсли", "ElsIf"))
            {
                state.EnterElseIf(state.Evaluate(ExtractCondition(directive, "ИначеЕсли", "ElsIf")));
                return true;
            }

            if (StartsWithKeyword(directive, "Иначе", "Else"))
            {
                state.EnterElse();
                return true;
            }

            if (StartsWithKeyword(directive, "КонецЕсли", "EndIf"))
            {
                state.LeaveIf();
                return true;
            }

            return false;
        }

        private static string TrimDirectiveTerminator(string directive)
        {
            var trimmed = directive.TrimEnd();
            if (trimmed.EndsWith(';'))
            {
                return trimmed.Substring(0, trimmed.Length - 1).TrimEnd();
            }

            return trimmed;
        }

        private static bool StartsWithKeyword(string directive, string ru, string en) =>
            directive.StartsWith(ru, StringComparison.OrdinalIgnoreCase)
            || directive.StartsWith(en, StringComparison.OrdinalIgnoreCase);

        private static string ExtractCondition(string directive, string ruKeyword, string enKeyword)
        {
            if (directive.StartsWith(ruKeyword, StringComparison.OrdinalIgnoreCase))
            {
                directive = directive.Substring(ruKeyword.Length);
            }
            else if (directive.StartsWith(enKeyword, StringComparison.OrdinalIgnoreCase))
            {
                directive = directive.Substring(enKeyword.Length);
            }

            directive = RemoveTrailingKeyword(directive.TrimStart(), "Тогда");
            directive = RemoveTrailingKeyword(directive, "Then");
            return directive.Trim();
        }

        private static string RemoveTrailingKeyword(string text, string keyword)
        {
            var trimmed = text.TrimEnd();
            if (trimmed.EndsWith(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return trimmed.Substring(0, trimmed.Length - keyword.Length).TrimEnd();
            }

            return text;
        }

        private sealed class ConditionalState
        {
            private readonly ConditionalDirectiveHandler _conditional;
            private readonly Stack<bool> _inclusive = new Stack<bool>();
            private readonly Stack<bool> _branchTaken = new Stack<bool>();

            public ConditionalState(ConditionalDirectiveHandler conditional)
            {
                _conditional = conditional;
                _inclusive.Push(true);
            }

            public bool IsLineActive() => _inclusive.Peek();

            public void EnterIf(bool condition)
            {
                var parent = _inclusive.Peek();
                _inclusive.Push(parent && condition);
                _branchTaken.Push(condition);
            }

            public void EnterElseIf(bool condition)
            {
                var previousBranchTaken = _branchTaken.Pop();
                _inclusive.Pop();
                var parent = _inclusive.Peek();

                _inclusive.Push(parent && !previousBranchTaken && condition);
                _branchTaken.Push(previousBranchTaken || condition);
            }

            public void EnterElse()
            {
                var previousBranchTaken = _branchTaken.Pop();
                _inclusive.Pop();
                var parent = _inclusive.Peek();

                _inclusive.Push(parent && !previousBranchTaken);
                _branchTaken.Push(true);
            }

            public void LeaveIf()
            {
                _inclusive.Pop();
                _branchTaken.Pop();
            }

            public bool Evaluate(string expression) =>
                string.IsNullOrWhiteSpace(expression) || EvaluateOr(expression);

            private bool EvaluateOr(string expression)
            {
                foreach (var part in SplitTopLevel(expression, "Или", "Or"))
                {
                    if (EvaluateAnd(part))
                    {
                        return true;
                    }
                }

                return false;
            }

            private bool EvaluateAnd(string expression)
            {
                foreach (var part in SplitTopLevel(expression, "И", "And"))
                {
                    if (!EvaluateNot(part))
                    {
                        return false;
                    }
                }

                return true;
            }

            private bool EvaluateNot(string expression)
            {
                var trimmed = expression.Trim();
                if (StartsWithKeyword(trimmed, "НЕ", "Not"))
                {
                    var spaceIndex = trimmed.IndexOf(' ');
                    var argument = spaceIndex >= 0 ? trimmed.Substring(spaceIndex + 1).Trim() : string.Empty;
                    return !EvaluateSymbol(argument);
                }

                return EvaluateSymbol(trimmed);
            }

            private bool EvaluateSymbol(string symbol)
            {
                if (symbol.StartsWith('(') && symbol.EndsWith(')'))
                {
                    return Evaluate(symbol.Substring(1, symbol.Length - 2));
                }

                return _conditional.IsDefined(symbol);
            }

            private static IEnumerable<string> SplitTopLevel(string expression, string ru, string en)
            {
                var parts = new List<string>();
                var current = new StringBuilder();
                var depth = 0;

                for (var i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == '(')
                    {
                        depth++;
                    }
                    else if (expression[i] == ')')
                    {
                        depth--;
                    }

                    if (depth == 0 && TryReadKeyword(expression, i, ru, out var keywordLength)
                        || depth == 0 && TryReadKeyword(expression, i, en, out keywordLength))
                    {
                        parts.Add(current.ToString());
                        current.Clear();
                        i += keywordLength - 1;
                        continue;
                    }

                    current.Append(expression[i]);
                }

                parts.Add(current.ToString());
                return parts;
            }

            private static bool TryReadKeyword(string expression, int index, string keyword, out int length)
            {
                length = 0;
                if (index > 0 && !char.IsWhiteSpace(expression[index - 1]))
                {
                    return false;
                }

                if (index + keyword.Length > expression.Length)
                {
                    return false;
                }

                if (!expression.AsSpan(index, keyword.Length).Equals(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var end = index + keyword.Length;
                if (end < expression.Length && !char.IsWhiteSpace(expression[end]))
                {
                    return false;
                }

                length = keyword.Length;
                return true;
            }
        }
    }
}
