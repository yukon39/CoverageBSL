using com.github.yukon39.CoverageBSL.AddIn.Converters;
using com.github.yukon39.CoverageBSL.AddIn.Internal;
using NUnit.Framework;
using System.Linq;

namespace com.github.yukon39.CoverageBSL.AddIn.Tests
{
    public class CoverableLinesBuildTest
    {
        private const int ExpectedConfigurationModuleCount = 79;
        private const int ExpectedConfigurationCoverableLineCount = 245;

        private const int ExpectedExtensionModuleCount = 2;
        private const int ExpectedExtensionCoverableLineCount = 4;

        private const int ExpectedExternalModuleCount = 2;
        private const int ExpectedExternalCoverableLineCount = 7;

        [TestCase("cf/xml", SourceFilesFormatEnum.XML, ExpectedConfigurationModuleCount, ExpectedConfigurationCoverableLineCount)]
        [TestCase("cf/edt", SourceFilesFormatEnum.EDT, ExpectedConfigurationModuleCount, ExpectedConfigurationCoverableLineCount)]
        [TestCase("cfe/xml", SourceFilesFormatEnum.XML, ExpectedExtensionModuleCount, ExpectedExtensionCoverableLineCount)]
        [TestCase("cfe/edt", SourceFilesFormatEnum.EDT, ExpectedExtensionModuleCount, ExpectedExtensionCoverableLineCount)]
        [TestCase("epf/xml", SourceFilesFormatEnum.XML, ExpectedExternalModuleCount, ExpectedExternalCoverableLineCount)]
        [TestCase("epf/edt", SourceFilesFormatEnum.EDT, ExpectedExternalModuleCount, ExpectedExternalCoverableLineCount)]
        public void BuildCoverableLines_ReturnsExpectedModules(
            string sourceDirectory,
            SourceFilesFormatEnum format,
            int expectedModuleCount,
            int expectedCoverableLineCount)
        {
            var dataSet = CoverableLinesBuilder.Build(
                TestResourcePaths.SourcesPath,
                [new SourceDirectoryData(sourceDirectory, format)]);

            var modules = dataSet.Modules.ToArray();

            Assert.That(modules, Has.Length.EqualTo(expectedModuleCount));
            Assert.That(modules[0].ModuleUrl, Does.StartWith(sourceDirectory));
            Assert.That(dataSet.TotalLineCount, Is.EqualTo(expectedCoverableLineCount));
        }
    }
}
