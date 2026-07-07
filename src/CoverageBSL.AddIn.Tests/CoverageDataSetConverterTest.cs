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
        private const string ExtensionName = "Расширение1";
        private const string ExternalModuleUrl = "file://ВнешняяОбработка1.epf";

        private static readonly string SourcesPath = TestResourcePaths.SourcesPath;
        private static readonly SourceDirectoryData[] XmlSourceDirectories =
        [
            new(TestResourcePaths.ConfigurationXmlSourcesPath, SourceFilesFormatEnum.XML),
            new(TestResourcePaths.ExtensionXmlSourcesPath, SourceFilesFormatEnum.XML, ExtensionName),
            new(
                TestResourcePaths.ExternalXmlSourcesPath,
                SourceFilesFormatEnum.XML,
                moduleUrl: ExternalModuleUrl)
        ];

        [Test]
        public void Convert_FromCoverageJson_BuildsModulesWithCoveredLines()
        {
            var coverageData = DeserializeCoverageJson();
            var dataSet = coverageData.ToCoverageDataSet(SourcesPath, XmlSourceDirectories);

            var expectedModuleCount = coverageData.Data
                .Cast<CoverageModuleInfoContextClass>()
                .Select(moduleInfo => CoverageDataSetModuleKeyBuilder.Build(
                    moduleInfo.ModuleId,
                    SourcesPath,
                    XmlSourceDirectories))
                .Where(moduleUrl => moduleUrl != null)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();
            Assert.That(dataSet.Modules.Count(), Is.EqualTo(expectedModuleCount));

            var calculationRegisterManager = dataSet.Get(
                "cf/xml/CalculationRegisters/РегистрРасчета1/Ext/ManagerModule.bsl");
            Assert.That(calculationRegisterManager, Is.Not.Null);
            Assert.That(
                calculationRegisterManager!.LineInfo.Select(line => line.LineNo),
                Is.EquivalentTo(new[] { 5, 6 }));

            var extensionManager = dataSet.Get(
                "cfe/xml/DataProcessors/Расш1_Обработка1/Ext/ManagerModule.bsl");
            Assert.That(extensionManager, Is.Not.Null);
            Assert.That(
                extensionManager!.LineInfo.Select(line => line.LineNo),
                Is.EquivalentTo(new[] { 5, 6 }));

            var externalForm = dataSet.Get(
                "epf/xml/ВнешняяОбработка1/Forms/Форма/Ext/Form/Module.bsl");
            Assert.That(externalForm, Is.Not.Null);
            Assert.That(
                externalForm!.LineInfo.Select(line => line.LineNo),
                Is.EquivalentTo(new[] { 6, 7, 8 }));

            var externalObject = dataSet.Get(
                "epf/xml/ВнешняяОбработка1/Ext/ObjectModule.bsl");
            Assert.That(externalObject, Is.Not.Null);
            Assert.That(
                externalObject!.LineInfo.Select(line => line.LineNo),
                Is.EquivalentTo(new[] { 5, 6 }));
        }

        [Test]
        public void BuildModuleKey_ConfigModule_ResolvesRelativeModulePath()
        {
            var moduleId = CreateConfigModuleId(
                "40d90f01-6080-49b4-a575-ceeabeb4c1b1",
                "d1b64a2c-8078-4982-8190-8f81aefda192");

            var moduleUrl = CoverageDataSetModuleKeyBuilder.Build(
                moduleId,
                SourcesPath,
                XmlSourceDirectories);

            Assert.That(moduleUrl, Is.EqualTo("cf/xml/CalculationRegisters/РегистрРасчета1/Ext/ManagerModule.bsl"));
        }

        [Test]
        public void BuildModuleKey_ExtensionModule_ResolvesRelativeModulePath()
        {
            var moduleId = CreateExtensionModuleId(
                "de483c0a-d17d-4fb6-be3e-573d4045ff79",
                "d1b64a2c-8078-4982-8190-8f81aefda192");

            var moduleUrl = CoverageDataSetModuleKeyBuilder.Build(
                moduleId,
                SourcesPath,
                XmlSourceDirectories);

            Assert.That(moduleUrl, Is.EqualTo("cfe/xml/DataProcessors/Расш1_Обработка1/Ext/ManagerModule.bsl"));
        }

        [Test]
        public void BuildModuleKey_ExtMdModule_ResolvesRelativeModulePath()
        {
            var moduleId = CreateExternalModuleId(
                "38e352a5-9e82-4e60-9244-b81cc3eed67e",
                "32e087ab-1491-49b6-aba7-43571b41ac2b");

            var moduleUrl = CoverageDataSetModuleKeyBuilder.Build(
                moduleId,
                SourcesPath,
                XmlSourceDirectories);

            Assert.That(moduleUrl, Is.EqualTo("epf/xml/ВнешняяОбработка1/Forms/Форма/Ext/Form/Module.bsl"));
        }

        [Test]
        public void BuildModuleKey_ExtMdModule_WithoutMatchingSourceDirectory_ReturnsNull()
        {
            var moduleId = CreateExternalModuleId(
                "38e352a5-9e82-4e60-9244-b81cc3eed67e",
                "32e087ab-1491-49b6-aba7-43571b41ac2b");

            var moduleUrl = CoverageDataSetModuleKeyBuilder.Build(
                moduleId,
                SourcesPath,
                [new(TestResourcePaths.ConfigurationXmlSourcesPath, SourceFilesFormatEnum.XML)]);

            Assert.That(moduleUrl, Is.Null);
        }

        [Test]
        public void BuildModuleKey_ExtensionModule_WithoutMatchingSourceDirectory_ReturnsNull()
        {
            var moduleId = CreateExtensionModuleId(
                "de483c0a-d17d-4fb6-be3e-573d4045ff79",
                "d1b64a2c-8078-4982-8190-8f81aefda192");

            var moduleUrl = CoverageDataSetModuleKeyBuilder.Build(
                moduleId,
                SourcesPath,
                [new(TestResourcePaths.ConfigurationXmlSourcesPath, SourceFilesFormatEnum.XML)]);

            Assert.That(moduleUrl, Is.Null);
        }

        private static CoverageModuleIdWrapper CreateConfigModuleId(string objectId, string propertyId)
        {
            var moduleId = CoverageModuleIdWrapper.ScriptConstructor();
            moduleId.ModuleType = "ConfigModule";
            moduleId.ExtensionName = string.Empty;
            moduleId.ObjectID = objectId;
            moduleId.PropertyID = propertyId;
            return moduleId;
        }

        private static CoverageModuleIdWrapper CreateExtensionModuleId(string objectId, string propertyId)
        {
            var moduleId = CoverageModuleIdWrapper.ScriptConstructor();
            moduleId.ModuleType = "ExtensionModule";
            moduleId.ExtensionName = ExtensionName;
            moduleId.ObjectID = objectId;
            moduleId.PropertyID = propertyId;
            return moduleId;
        }

        private static CoverageModuleIdWrapper CreateExternalModuleId(string objectId, string propertyId)
        {
            var moduleId = CoverageModuleIdWrapper.ScriptConstructor();
            moduleId.ModuleType = "ExtMDModule";
            moduleId.URL = ExternalModuleUrl;
            moduleId.ObjectID = objectId;
            moduleId.PropertyID = propertyId;
            return moduleId;
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
