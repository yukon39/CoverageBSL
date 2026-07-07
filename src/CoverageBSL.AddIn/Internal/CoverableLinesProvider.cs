using com.github.yukon39.CoverageBSL.AddIn.Internal.Ast;
using OneScript.Language.SyntaxAnalysis;
using OneScript.Language.SyntaxAnalysis.AstNodes;
using OneScript.Sources;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Internal
{
    internal static class CoverableLinesProvider
    {
        public static IEnumerable<int> FromFile(string sourcePath) =>
            ExtractCoverableLines(SourceCodeBuilder.Create().FromFile(sourcePath).Build());

        internal static IEnumerable<int> ExtractCoverableLinesFromFile(string sourcePath) =>
            ExtractCoverableLines(SourceCodeBuilder.Create().FromFile(sourcePath).Build());

        public static IEnumerable<int> FromSource(string source, string location) =>
            ExtractCoverableLines(SourceCodeBuilder.Create().FromString(source).WithName(location).Build());

        internal static SortedSet<int> ExtractCoverableLines(SourceCode source)
        {
            var lexer = PreprocessingLexerFactory.Create(source, out var handlers, out var errorSink);
            var module = new DefaultBslParser(lexer, errorSink, handlers).ParseStatefulModule() as ModuleNode;
            return AstCoverableLinesCollector.Collect(module, source);
        }
    }
}
