using OneScript.Language;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis;
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
            var errorSink = new ListErrorSink();
            var handlers = new PreprocessorHandlers(new IDirectiveHandler[]
            {
                new IgnorePreprocessorDirectiveHandler(errorSink)
            });

            var baseLexer = new DefaultLexer { Iterator = source.CreateIterator() };
            var lexer = new PreprocessingLexer(baseLexer)
            {
                Handlers = handlers,
                ErrorSink = errorSink
            };

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
