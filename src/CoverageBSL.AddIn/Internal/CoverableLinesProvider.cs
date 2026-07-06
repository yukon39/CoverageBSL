using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using OneScript.Language;
using OneScript.Language.LexicalAnalysis;
using OneScript.Language.SyntaxAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;
using OneScript.Sources;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    internal static class CoverableLinesProvider
    {
        private static readonly HashSet<string> KnownModuleAnnotations = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "НаСервере", "НаКлиенте", "AtServer", "AtClient"
        };

        public static CoverageLineInfosContextClass FromFile(string sourcePath) =>
            FromSourceCode(SourceCodeBuilder.Create().FromFile(sourcePath).Build());

        public static CoverageLineInfosContextClass FromSource(string source, string location) =>
            FromSourceCode(SourceCodeBuilder.Create().FromString(source).WithName(location).Build());

        private static CoverageLineInfosContextClass FromSourceCode(SourceCode source) =>
            ToLineInfoList(ExtractCoverableLines(source));

        internal static SortedSet<int> ExtractCoverableLines(SourceCode source)
        {
            var errorSink = new ListErrorSink();
            var conditional = new ConditionalDirectiveHandler(errorSink);
            conditional.Define("Клиент");

            var resolved = PreprocessorSourceResolver.Resolve(source, conditional);
            var handlers = new PreprocessorHandlers(new IDirectiveHandler[]
            {
                conditional,
                new RegionDirectiveHandler(errorSink),
                new SingleWordModuleAnnotationHandler(KnownModuleAnnotations, errorSink)
            });

            var lexer = new DefaultLexer { Iterator = resolved.CreateIterator() };
            var module = new DefaultBslParser(lexer, errorSink, handlers).ParseStatefulModule() as ModuleNode;

            return AstCoverableLinesCollector.Collect(module, resolved);
        }

        private static CoverageLineInfosContextClass ToLineInfoList(IEnumerable<int> lineNumbers)
        {
            var lineInfos = new List<PerformanceInfoLine>();
            foreach (var lineNo in lineNumbers)
            {
                lineInfos.Add(new PerformanceInfoLine { LineNo = lineNo, Frequency = 0 });
            }

            return new CoverageLineInfosContextClass(lineInfos);
        }
    }
}
