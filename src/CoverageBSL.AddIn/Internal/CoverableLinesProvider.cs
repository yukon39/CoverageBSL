using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using OneScript.Language;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;
using OneScript.Sources;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    internal static class CoverableLinesProvider
    {
        public static CoverageLineInfosContextClass FromFile(string sourcePath)
        {
            var source = SourceCodeBuilder.Create().FromFile(sourcePath).Build();
            return ToLineInfoList(ExtractCoverableLines(source));
        }

        public static CoverageLineInfosContextClass FromSource(string source, string location)
        {
            var sourceCode = SourceCodeBuilder.Create()
                .FromString(source)
                .WithName(location)
                .Build();
            return ToLineInfoList(ExtractCoverableLines(sourceCode));
        }

        internal static SortedSet<int> ExtractCoverableLines(SourceCode source)
        {
            var errorSink = new ListErrorSink();
            var handlers = CreatePreprocessorHandlers(errorSink);
            var conditional = handlers.Get<ConditionalDirectiveHandler>()
                ?? throw new InvalidOperationException("ConditionalDirectiveHandler is not registered.");
            conditional.Define("Клиент");

            var resolved = PreprocessorSourceResolver.Resolve(source, conditional);
            var module = ParseModule(resolved, errorSink, handlers);
            return AstCoverableLinesCollector.Collect(module, resolved);
        }

        private static readonly HashSet<string> KnownModuleAnnotations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "НаСервере", "НаКлиенте", "AtServer", "AtClient"
        };

        private static ModuleNode ParseModule(
            SourceCode resolved,
            IErrorSink errorSink,
            PreprocessorHandlers handlers)
        {
            var lexer = new DefaultLexer { Iterator = resolved.CreateIterator() };
            var parser = new DefaultBslParser(lexer, errorSink, handlers);

            return parser.ParseStatefulModule() as ModuleNode;
        }

        private static PreprocessorHandlers CreatePreprocessorHandlers(IErrorSink errorSink) =>
            new PreprocessorHandlers(new IDirectiveHandler[]
            {
                new ConditionalDirectiveHandler(errorSink),
                new RegionDirectiveHandler(errorSink),
                new SingleWordModuleAnnotationHandler(KnownModuleAnnotations, errorSink)
            });

        private static CoverageLineInfosContextClass ToLineInfoList(IEnumerable<int> lineNumbers)
        {
            var lineInfos = new List<PerformanceInfoLine>();
            foreach (var lineNo in lineNumbers)
            {
                lineInfos.Add(new PerformanceInfoLine
                {
                    LineNo = lineNo,
                    Frequency = 0
                });
            }

            return new CoverageLineInfosContextClass(lineInfos);
        }
    }
}
