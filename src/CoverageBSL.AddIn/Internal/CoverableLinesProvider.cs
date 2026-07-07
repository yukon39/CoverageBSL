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
        public static CoverageLineInfosContextClass FromFile(string sourcePath) =>
            FromSourceCode(SourceCodeBuilder.Create().FromFile(sourcePath).Build());

        internal static IEnumerable<int> ExtractCoverableLinesFromFile(string sourcePath) =>
            ExtractCoverableLines(SourceCodeBuilder.Create().FromFile(sourcePath).Build());

        public static CoverageLineInfosContextClass FromSource(string source, string location) =>
            FromSourceCode(SourceCodeBuilder.Create().FromString(source).WithName(location).Build());

        private static CoverageLineInfosContextClass FromSourceCode(SourceCode source) =>
            ToLineInfoList(ExtractCoverableLines(source));

        internal static SortedSet<int> ExtractCoverableLines(SourceCode source)
        {
            var errorSink = new ListErrorSink();
            var conditional = new ConditionalDirectiveHandler(errorSink);

            var resolved = PreprocessorSourceResolver.Resolve(source);
            var handlers = new PreprocessorHandlers(new IDirectiveHandler[]
            {
                conditional,
                new RegionDirectiveHandler(errorSink)
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
