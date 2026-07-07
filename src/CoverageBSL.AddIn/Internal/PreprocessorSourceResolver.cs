using OneScript.Sources;
using System;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    /// <summary>
    /// Убирает строки с директивами препроцессора, сохраняя номера строк.
    /// Код всех веток <c>#Если</c> остаётся — как в Coverage41C / bsl-parser (директивы в AST, без вычисления условий).
    /// </summary>
    internal static class PreprocessorSourceResolver
    {
        public static SourceCode Resolve(SourceCode source)
        {
            var lines = source.GetSourceCode()
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Replace('\r', '\n')
                .Split('\n');

            var output = new string[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                output[i] = IsPreprocessorDirectiveLine(lines[i]) ? string.Empty : lines[i];
            }

            return SourceCodeBuilder.Create()
                .FromString(string.Join("\r\n", output))
                .WithName(source.Name)
                .Build();
        }

        private static bool IsPreprocessorDirectiveLine(string line)
        {
            var trimmed = line.TrimStart();
            if (!trimmed.StartsWith("#", StringComparison.Ordinal))
            {
                return false;
            }

            return trimmed.Length > 1 && !char.IsWhiteSpace(trimmed[1]);
        }
    }
}
