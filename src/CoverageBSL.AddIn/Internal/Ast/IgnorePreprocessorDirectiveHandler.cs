using OneScript.Language;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal.Ast
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
            while (lexer.NextLexemOnSameLine().Type != LexemType.EndOfText)
            {
            }

            lastExtractedLexem = lexer.NextLexem();
            return true;
        }
    }
}
