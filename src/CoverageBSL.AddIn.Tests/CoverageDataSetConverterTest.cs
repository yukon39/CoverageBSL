using com.github.yukon39.CoverageBSL.AddIn.Converters;
using com.github.yukon39.CoverageBSL.AddIn.CoverageData;
using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Internal;
using NUnit.Framework;
using OneScript.StandardLibrary.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.github.yukon39.CoverageBSL.AddIn.Tests
{
    public class CoverageDataSetConverterTest
    {
        private static readonly string XmlSourcesPath = TestResourcePaths.ConfigurationXmlSourcesPath;
        private static readonly SourceDirectoryData[] XmlSourceDirectories =
        [
            new(XmlSourcesPath, SourceFilesFormatEnum.XML)
        ];

        [Test]
        public void Convert_FromCoverageJson_BuildsModulesWithCoveredLines()
        {
            var coverageData = DeserializeCoverageJson();
            var dataSet = coverageData.ToCoverageDataSet(XmlSourcesPath, XmlSourceDirectories);

            var expectedModuleCount = coverageData.Data
                .Cast<CoverageModuleInfoContextClass>()
                .Select(moduleInfo => CoverageDataSetModuleKeyBuilder.Build(
                    moduleInfo.ModuleId,
                    XmlSourcesPath,
                    XmlSourceDirectories))
                .Where(moduleUrl => moduleUrl != null)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();
            Assert.That(dataSet.Modules.Count(), Is.EqualTo(expectedModuleCount));

            var calculationRegisterManager = dataSet.Get(
                "CalculationRegisters/РегистрРасчета1/Ext/ManagerModule.bsl");
            Assert.That(calculationRegisterManager, Is.Not.Null);
            Assert.That(
                calculationRegisterManager!.LineInfo.Select(line => line.LineNo),
                Is.EquivalentTo(new[] { 5, 6 }));

            var coveredLines = calculationRegisterManager.LineInfo.Where(line => line.HasCoverage).ToArray();
            Assert.That(coveredLines, Has.Length.EqualTo(2));
            Assert.That(coveredLines.All(line => line.HasCoverage), Is.True);
        }

        [Test]
        public void BuildModuleKey_ConfigModule_ResolvesRelativeModulePath()
        {
            var moduleId = CoverageModuleIdWrapper.ScriptConstructor();
            moduleId.ModuleType = "ConfigModule";
            moduleId.ExtensionName = string.Empty;
            moduleId.ObjectID = "40d90f01-6080-49b4-a575-ceeabeb4c1b1";
            moduleId.PropertyID = "d1b64a2c-8078-4982-8190-8f81aefda192";

            var moduleUrl = CoverageDataSetModuleKeyBuilder.Build(
                moduleId,
                XmlSourcesPath,
                XmlSourceDirectories);

            Assert.That(moduleUrl, Is.EqualTo("CalculationRegisters/РегистрРасчета1/Ext/ManagerModule.bsl"));
        }

        [Test]
        public void BuildModuleKey_WithUrl_UsesUrl()
        {
            var moduleId = CoverageModuleIdWrapper.ScriptConstructor();
            moduleId.URL = "file://external.epf";

            var moduleUrl = CoverageDataSetModuleKeyBuilder.Build(
                moduleId,
                XmlSourcesPath,
                XmlSourceDirectories);

            Assert.That(moduleUrl, Is.EqualTo("file://external.epf"));
        }

        [Test]
        public void BuildModuleKey_ExtensionModule_ReturnsNull()
        {
            var moduleId = CoverageModuleIdWrapper.ScriptConstructor();
            moduleId.ModuleType = "ExtensionModule";
            moduleId.ExtensionName = "Расширение1";
            moduleId.ObjectID = "de483c0a-d17d-4fb6-be3e-573d4045ff79";
            moduleId.PropertyID = "d1b64a2c-8078-4982-8190-8f81aefda192";

            var moduleUrl = CoverageDataSetModuleKeyBuilder.Build(
                moduleId,
                XmlSourcesPath,
                XmlSourceDirectories);

            Assert.That(moduleUrl, Is.Null);
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
