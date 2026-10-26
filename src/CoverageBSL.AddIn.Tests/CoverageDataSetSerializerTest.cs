using com.github.yukon39.CoverageBSL.AddIn;
using com.github.yukon39.CoverageBSL.AddIn.CoverageData;
using com.github.yukon39.CoverageBSL.AddIn.CoverageData.Serialization;
using com.github.yukon39.CoverageBSL.AddIn.Internal;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace com.github.yukon39.CoverageBSL.AddIn.Tests
{
    public class CoverageDataSetSerializerTest
    {
        private string tempDirectory = null!;

        [SetUp]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, recursive: true);
            }
        }

        [Test]
        public void Raw_RoundTrip_PreservesCoverageDataSet()
        {
            var source = CreateSampleDataSet();
            var filePath = Path.Combine(tempDirectory, "coverage.raw.json");

            source.WriteToFile(filePath, CoverageDataFormatEnum.RAW);
            var restored = CoverageDataSet.ReadFromFile(filePath, CoverageDataFormatEnum.RAW);

            AssertDataSetsEqual(source, restored);
        }

        [Test]
        public void GenericCoverage_RoundTrip_PreservesCoverageDataSet()
        {
            var source = CreateSampleDataSet();
            var filePath = Path.Combine(tempDirectory, "coverage.generic.xml");

            source.WriteToFile(filePath, CoverageDataFormatEnum.GenericCoverage);
            var restored = CoverageDataSet.ReadFromFile(filePath, CoverageDataFormatEnum.GenericCoverage);

            AssertDataSetsEqual(source, restored);
        }

        [Test]
        public void GenericCoverage_Write_UsesGenericCoverageSchema()
        {
            var source = CreateSampleDataSet();
            var filePath = Path.Combine(tempDirectory, "coverage.generic.xml");

            source.WriteToFile(filePath, CoverageDataFormatEnum.GenericCoverage);

            var content = File.ReadAllText(filePath);
            Assert.That(content, Does.Contain("<coverage version=\"1\">"));
            Assert.That(content, Does.Contain("<file path=\"cf/xml/CommonModules/ОбщийМодульСервер/Ext/Module.bsl\">"));
            Assert.That(content, Does.Contain("<lineToCover lineNumber=\"5\" covered=\"true\"/>"));
            Assert.That(content, Does.Contain("<lineToCover lineNumber=\"10\" covered=\"false\"/>"));
        }

        [Test]
        public void Raw_Write_UsesCoverageDataSetSchema()
        {
            var source = CreateSampleDataSet();
            var filePath = Path.Combine(tempDirectory, "coverage.raw.json");

            source.WriteToFile(filePath, CoverageDataFormatEnum.RAW);

            var content = File.ReadAllText(filePath);
            Assert.That(content, Does.Contain("\"Modules\""));
            Assert.That(content, Does.Contain("\"ModuleUrl\""));
            Assert.That(content, Does.Contain("\"LineInfo\""));
            Assert.That(content, Does.Contain("\"LineNo\""));
            Assert.That(content, Does.Contain("\"HasCoverage\""));
            Assert.That(content, Does.Not.Contain("\"SourcePath\""));
        }

        [Test]
        public void CoverableLines_WriteToFile_WritesBuiltCoverableLines()
        {
            var coverableLines = CoverableLinesContextClass.ScriptConstructor(TestResourcePaths.SourcesPath);
            coverableLines.AddSourceDirectory("cf/xml");
            coverableLines.BuildCoverableLines();

            var filePath = Path.Combine(tempDirectory, "lines.generic.xml");
            coverableLines.WriteToFile(filePath, CoverageDataFormatEnum.GenericCoverage);

            Assert.That(File.Exists(filePath), Is.True);
            Assert.That(new FileInfo(filePath).Length, Is.GreaterThan(0));
        }

        private static CoverageDataSet CreateSampleDataSet()
        {
            var dataSet = new CoverageDataSet();
            var module = dataSet.GetOrAdd("cf/xml/CommonModules/ОбщийМодульСервер/Ext/Module.bsl");
            module.AddLine(10);
            module.MarkCovered(5);
            module.MarkCovered(6);
            return dataSet;
        }

        private static void AssertDataSetsEqual(CoverageDataSet expected, CoverageDataSet actual)
        {
            var expectedModules = expected.Modules.OrderBy(module => module.ModuleUrl).ToArray();
            var actualModules = actual.Modules.OrderBy(module => module.ModuleUrl).ToArray();

            Assert.That(actualModules, Has.Length.EqualTo(expectedModules.Length));

            for (var index = 0; index < expectedModules.Length; index++)
            {
                Assert.That(actualModules[index].ModuleUrl, Is.EqualTo(expectedModules[index].ModuleUrl));

                var expectedLines = expectedModules[index].LineInfo
                    .Select(line => (line.LineNo, line.HasCoverage))
                    .ToArray();
                var actualLines = actualModules[index].LineInfo
                    .Select(line => (line.LineNo, line.HasCoverage))
                    .ToArray();

                Assert.That(actualLines, Is.EquivalentTo(expectedLines));
            }
        }
    }
}
