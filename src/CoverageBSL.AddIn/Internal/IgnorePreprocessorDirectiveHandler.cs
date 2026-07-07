using OneScript.Exceptions;
using OneScript.Language;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    /// <summary>
    /// Пропускает любую директиву <c>#</c> до конца строки, не вычисляя условия.
    /// Код всех веток остаётся в AST — как в Coverage41C / bsl-parser.
    /// </summary>
    internal sealed class IgnorePreprocessorDirectiveHandler : DirectiveHandlerBase
    {
        public IgnorePreprocessorDirectiveHandler(IErrorSink errorSink)
            : base(errorSink)
        {
        }

        public override bool HandleDirective(ref Lexem lastExtractedLexem, ILexer lexer)
        {
            while (true)
            {
                var next = lexer.NextLexemOnSameLine();
                if (next.Type == LexemType.EndOfText)
                {
                    break;
                }
            }

            lastExtractedLexem = LexemFromNewLine(lexer);
            return true;
        }

        private static Lexem LexemFromNewLine(ILexer lexer)
        {
            var lex = lexer.NextLexem();

            if (!lexer.Iterator.OnNewLine)
            {
                throw new SyntaxErrorException(lexer.GetErrorPosition(), "Недопустимые символы в директиве");
            }

            return lex;
        }
    }
}
