using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Internal.Ast;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using OneScript.Language.SyntaxAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;
using OneScript.Sources;
using System.Collections.Generic;
using System.Linq;

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
            var lexer = PreprocessingLexerFactory.Create(source, out var handlers, out var errorSink);
            var module = new DefaultBslParser(lexer, errorSink, handlers).ParseStatefulModule() as ModuleNode;
            return AstCoverableLinesCollector.Collect(module, source);
        }

        private static CoverageLineInfosContextClass ToLineInfoList(IEnumerable<int> lineNumbers)
            => new CoverageLineInfosContextClass(
                lineNumbers
                    .Select(lineNo => new PerformanceInfoLine { LineNo = lineNo, Frequency = 0 })
                    .ToList());
    }
}
