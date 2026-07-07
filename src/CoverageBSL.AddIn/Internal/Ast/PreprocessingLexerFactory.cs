using OneScript.Language;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis;
using OneScript.Sources;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal.Ast
{
    internal static class PreprocessingLexerFactory
    {
        public static PreprocessingLexer Create(SourceCode source, out PreprocessorHandlers handlers, out ListErrorSink errorSink)
        {
            errorSink = new ListErrorSink();
            handlers = new PreprocessorHandlers(new IDirectiveHandler[]
            {
                new IgnorePreprocessorDirectiveHandler(errorSink)
            });

            var baseLexer = new DefaultLexer { Iterator = source.CreateIterator() };
            return new PreprocessingLexer(baseLexer)
            {
                Handlers = handlers,
                ErrorSink = errorSink
            };
        }
    }
}
