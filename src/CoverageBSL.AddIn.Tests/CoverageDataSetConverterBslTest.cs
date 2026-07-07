using com.github.yukon39.CoverageBSL.AddIn;
using com.github.yukon39.CoverageBSL.AddIn.Converters;
using com.github.yukon39.CoverageBSL.AddIn.Internal;
using NUnit.Framework;
using OneScript.StandardLibrary.Json;
using System.Linq;

namespace com.github.yukon39.CoverageBSL.AddIn.Tests
{
    public class CoverageDataSetConverterBslTest
    {
        private const string ExtensionName = "Расширение1";
        private const string ExternalModuleUrl = "file://ВнешняяОбработка1.epf";

        private const int ExpectedConfigurationModuleCount = 76;
        private const int ExpectedConfigurationCoveredLineCount = 250;

        private const int ExpectedExtensionModuleCount = 2;
        private const int ExpectedExtensionCoveredLineCount = 4;

        private const int ExpectedExternalModuleCount = 2;
        private const int ExpectedExternalCoveredLineCount = 5;

        [TestCase("cf/xml", SourceFilesFormatEnum.XML, "", "", ExpectedConfigurationModuleCount, ExpectedConfigurationCoveredLineCount)]
        [TestCase("cf/edt", SourceFilesFormatEnum.EDT, "", "", ExpectedConfigurationModuleCount, ExpectedConfigurationCoveredLineCount)]
        [TestCase("cfe/xml", SourceFilesFormatEnum.XML, ExtensionName, "", ExpectedExtensionModuleCount, ExpectedExtensionCoveredLineCount)]
        [TestCase("cfe/edt", SourceFilesFormatEnum.EDT, ExtensionName, "", ExpectedExtensionModuleCount, ExpectedExtensionCoveredLineCount)]
        [TestCase("epf/xml", SourceFilesFormatEnum.XML, "", ExternalModuleUrl, ExpectedExternalModuleCount, ExpectedExternalCoveredLineCount)]
        [TestCase("epf/edt", SourceFilesFormatEnum.EDT, "", ExternalModuleUrl, ExpectedExternalModuleCount, ExpectedExternalCoveredLineCount)]
        public void Convert_FromCoverageJson_ReturnsExpectedModules(
            string sourceDirectory,
            SourceFilesFormatEnum format,
            string extensionName,
            string moduleUrl,
            int expectedModuleCount,
            int expectedCoveredLineCount)
        {
            var coverageData = DeserializeCoverageJson();
            var sourceDirectories = CreateSourceDirectories(sourceDirectory, format, extensionName, moduleUrl);

            var dataSet = coverageData.ToCoverageDataSet(TestResourcePaths.SourcesPath, sourceDirectories);
            var modules = dataSet.Modules.ToArray();

            Assert.That(modules, Has.Length.EqualTo(expectedModuleCount));
            Assert.That(modules[0].ModuleUrl, Does.StartWith(sourceDirectory));
            Assert.That(dataSet.TotalLineCount, Is.EqualTo(expectedCoveredLineCount));
        }

        private static SourceDirectoryData[] CreateSourceDirectories(
            string sourceDirectory,
            SourceFilesFormatEnum format,
            string extensionName,
            string moduleUrl)
        {
            if (!string.IsNullOrEmpty(extensionName))
            {
                return [new SourceDirectoryData(sourceDirectory, format, extensionName)];
            }

            if (!string.IsNullOrEmpty(moduleUrl))
            {
                return [new SourceDirectoryData(sourceDirectory, format, moduleUrl: moduleUrl)];
            }

            return [new SourceDirectoryData(sourceDirectory, format)];
        }

        private static CoverageDataContextClass DeserializeCoverageJson()
        {
            var reader = JSONReader.Constructor();
            reader.OpenFile(TestResourcePaths.ResourcePath("coverage.json"));

            var coverageData = CoverageDataContextClass.ScriptConstructor();
            coverageData.DeserializeJson(reader);
            return coverageData;
        }
    }
}
