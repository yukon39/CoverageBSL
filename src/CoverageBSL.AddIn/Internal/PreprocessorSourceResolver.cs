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
            var normalized = source.GetSourceCode()
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace('\r', '\n');
            var lines = normalized.Split('\n');
            var output = new string[lines.Length];
            var state = new ConditionalState(conditional);

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (TryHandleDirective(line, state, out var directiveHandled))
                {
                    output[i] = string.Empty;
                    if (directiveHandled)
                    {
                        continue;
                    }
                }

                output[i] = state.IsLineActive() ? line : string.Empty;
            }

            var builder = new StringBuilder();
            for (var i = 0; i < output.Length; i++)
            {
                builder.Append(output[i]);
                if (i < output.Length - 1)
                {
                    builder.Append("\r\n");
                }
            }

            return SourceCodeBuilder.Create()
                .FromString(builder.ToString())
                .WithName(source.Name)
                .Build();
        }

        private static bool TryHandleDirective(string line, ConditionalState state, out bool handled)
        {
            handled = false;
            var trimmed = line.TrimStart();
            if (!trimmed.StartsWith("#", StringComparison.Ordinal))
            {
                return false;
            }

            var directive = trimmed.Substring(1).TrimStart();
            directive = TrimDirectiveTerminator(directive);
            if (directive.Length == 0)
            {
                return false;
            }

            if (IsRegionStart(directive))
            {
                handled = true;
                return true;
            }

            if (IsRegionEnd(directive))
            {
                handled = true;
                return true;
            }

            if (StartsWithKeyword(directive, "Если", "If"))
            {
                var condition = ExtractCondition(directive, "Если", "If");
                state.EnterIf(state.Evaluate(condition));
                handled = true;
                return true;
            }

            if (StartsWithKeyword(directive, "ИначеЕсли", "ElsIf"))
            {
                var condition = ExtractCondition(directive, "ИначеЕсли", "ElsIf");
                state.EnterElseIf(state.Evaluate(condition));
                handled = true;
                return true;
            }

            if (StartsWithKeyword(directive, "Иначе", "Else"))
            {
                state.EnterElse();
                handled = true;
                return true;
            }

            if (StartsWithKeyword(directive, "КонецЕсли", "EndIf"))
            {
                state.LeaveIf();
                handled = true;
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

        private static bool IsRegionStart(string directive) =>
            StartsWithKeyword(directive, "Область", "Region");

        private static bool IsRegionEnd(string directive) =>
            StartsWithKeyword(directive, "КонецОбласти", "EndRegion");

        private static bool StartsWithKeyword(string directive, string ru, string en)
        {
            return directive.StartsWith(ru, StringComparison.OrdinalIgnoreCase)
                || directive.StartsWith(en, StringComparison.OrdinalIgnoreCase);
        }

        private static string ExtractCondition(string directive, string ruKeyword, string enKeyword)
        {
            var condition = RemoveKeyword(directive, ruKeyword);
            condition = RemoveKeyword(condition, enKeyword);
            condition = RemoveTrailingKeyword(condition, "Тогда");
            condition = RemoveTrailingKeyword(condition, "Then");
            return condition.Trim();
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

        private static string RemoveKeyword(string text, string keyword)
        {
            if (text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return text.Substring(keyword.Length).TrimStart();
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
                var active = parent && condition;
                _inclusive.Push(active);
                _branchTaken.Push(condition);
            }

            public void EnterElseIf(bool condition)
            {
                var parent = GetParentInclusive();
                var previousBranchTaken = _branchTaken.Pop();
                _inclusive.Pop();

                var active = parent && !previousBranchTaken && condition;
                _inclusive.Push(active);
                _branchTaken.Push(previousBranchTaken || condition);
            }

            public void EnterElse()
            {
                var parent = GetParentInclusive();
                var previousBranchTaken = _branchTaken.Pop();
                _inclusive.Pop();
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

            private bool GetParentInclusive()
            {
                var values = _inclusive.ToArray();
                return values.Length >= 2 && values[1];
            }

            private bool EvaluateOr(string expression)
            {
                var parts = SplitTopLevel(expression, "Или", "Or");
                foreach (var part in parts)
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
                var parts = SplitTopLevel(expression, "И", "And");
                foreach (var part in parts)
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
                    var argument = trimmed.Substring(trimmed.IndexOf(' ') + 1).Trim();
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

                return _conditional?.IsDefined(symbol) ?? false;
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

                    if (depth == 0 && TryReadKeyword(expression, i, ru, out var ruLength)
                        || depth == 0 && TryReadKeyword(expression, i, en, out ruLength))
                    {
                        parts.Add(current.ToString());
                        current.Clear();
                        i += ruLength - 1;
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
