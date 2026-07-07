using OneScript.Language.LexicalAnalysis;
using OneScript.Sources;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    /// <summary>
    /// Перечисление всех лексем исходника (без отбора).
    /// </summary>
    internal static class SourceLexemeEnumerator
    {
        public static IEnumerable<Lexem> Enumerate(SourceCode source)
        {
            var lexer = PreprocessingLexerFactory.Create(source, out _, out _);

            while (true)
            {
                var lexem = lexer.NextLexem();
                if (lexem.Type == LexemType.EndOfText)
                {
                    yield break;
                }

                yield return lexem;
            }
        }
    }
}
