using com.github.yukon39.CoverageBSL.AddIn.Converters;
using com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers;
using com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers.Edt;
using com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers.Xml;
using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Internal;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace com.github.yukon39.CoverageBSL.AddIn.Tests
{
    public class CoverageModulePathResolverTest
    {
        private static readonly string XmlProjectPath = TestResourcePaths.ConfigurationXmlSourcesPath;
        private static readonly string EdtProjectPath = TestResourcePaths.ConfigurationEdtProjectPath;
        private static readonly string ExtensionXmlProjectPath = TestResourcePaths.ExtensionXmlSourcesPath;
        private static readonly string ExtensionEdtProjectPath = TestResourcePaths.ExtensionEdtProjectPath;
        private static readonly string ExternalXmlProjectPath = TestResourcePaths.ExternalXmlSourcesPath;
        private static readonly string ExternalEdtProjectPath = TestResourcePaths.ExternalEdtProjectPath;

        [TestCaseSource(nameof(XmlModulePathCases))]
        public void Resolve_Xml_ReturnsExpectedModulePath(string objectId, string propertyId, string expectedRelativePath)
        {
            AssertModulePath(XmlProjectPath, SourceFilesFormatEnum.XML, "ConfigModule", objectId, propertyId, expectedRelativePath);
        }

        [TestCaseSource(nameof(EdtModulePathCases))]
        public void Resolve_Edt_ReturnsExpectedModulePath(string objectId, string propertyId, string expectedRelativePath)
        {
            AssertModulePath(EdtProjectPath, SourceFilesFormatEnum.EDT, "ConfigModule", objectId, propertyId, expectedRelativePath);
        }

        [TestCaseSource(nameof(XmlExtensionModulePathCases))]
        public void Resolve_XmlExtension_ReturnsExpectedModulePath(string objectId, string propertyId, string expectedRelativePath)
        {
            AssertModulePath(ExtensionXmlProjectPath, SourceFilesFormatEnum.XML, "ExtensionModule", objectId, propertyId, expectedRelativePath);
        }

        [TestCaseSource(nameof(EdtExtensionModulePathCases))]
        public void Resolve_EdtExtension_ReturnsExpectedModulePath(string objectId, string propertyId, string expectedRelativePath)
        {
            AssertModulePath(ExtensionEdtProjectPath, SourceFilesFormatEnum.EDT, "ExtensionModule", objectId, propertyId, expectedRelativePath);
        }

        [TestCaseSource(nameof(XmlExternalModulePathCases))]
        public void Resolve_XmlExternal_ReturnsExpectedModulePath(string objectId, string propertyId, string expectedRelativePath)
        {
            AssertModulePath(ExternalXmlProjectPath, SourceFilesFormatEnum.XML, "ExtMDModule", objectId, propertyId, expectedRelativePath);
        }

        [TestCaseSource(nameof(EdtExternalModulePathCases))]
        public void Resolve_EdtExternal_ReturnsExpectedModulePath(string objectId, string propertyId, string expectedRelativePath)
        {
            AssertModulePath(ExternalEdtProjectPath, SourceFilesFormatEnum.EDT, "ExtMDModule", objectId, propertyId, expectedRelativePath);
        }

        private static IEnumerable<TestCaseData> XmlModulePathCases()
        {
            yield return Case(
                "de7b6e19-305b-4dc7-b761-9762afaadae2",
                "9b7bbbae-9771-46f2-9e4d-2489e0ffc702",
                "Ext/SessionModule.bsl");
            yield return Case(
                "854f7300-9d83-4358-a8ae-eb33c9a42bbe",
                "3e58c91f-9aaa-4f42-8999-4baf33907b75",
                "Constants/Константа1/Ext/ValueManagerModule.bsl");
            yield return Case(
                "40d90f01-6080-49b4-a575-ceeabeb4c1b1",
                "d1b64a2c-8078-4982-8190-8f81aefda192",
                "CalculationRegisters/РегистрРасчета1/Ext/ManagerModule.bsl");
            yield return Case(
                "40d90f01-6080-49b4-a575-ceeabeb4c1b1",
                "9f36fd70-4bf4-47f6-b235-935f73aab43f",
                "CalculationRegisters/РегистрРасчета1/Ext/RecordSetModule.bsl");
            yield return Case(
                "3d66b766-54c8-423d-887d-6670cfc1b8cc",
                "9f36fd70-4bf4-47f6-b235-935f73aab43f",
                "CalculationRegisters/РегистрРасчета1/Recalculations/Перерасчет1/Ext/RecordSetModule.bsl");
            yield return Case(
                "52895f8c-2982-4f1d-bfb0-75b3dd5db3d3",
                "d5963243-262e-4398-b4d7-fb16d06484f6",
                "CommonModules/ОбщийМодульВызовСервера/Ext/Module.bsl");
            yield return Case(
                "a722a8f2-1ea9-4984-ae17-96eb28b01d15",
                "32e087ab-1491-49b6-aba7-43571b41ac2b",
                "CommonForms/Форма/Ext/Form/Module.bsl");
            yield return Case(
                "0fcc2b09-6188-4573-843d-41e906660823",
                "32e087ab-1491-49b6-aba7-43571b41ac2b",
                "Tasks/Задача1/Forms/Форма/Ext/Form/Module.bsl");
            yield return Case(
                "c8dc1e16-1c5d-49e2-a4a6-e4a0626f8221",
                "078a6af8-d22c-4248-9c33-7e90075a3d2c",
                "ExchangePlans/ПланОбмена1/Commands/Команда1/Ext/CommandModule.bsl");
            yield return Case(
                "4df936d5-34c4-4685-9821-04e311146955",
                "078a6af8-d22c-4248-9c33-7e90075a3d2c",
                "CommonCommands/ОбщаяКоманда1/Ext/CommandModule.bsl");
        }

        private static IEnumerable<TestCaseData> EdtModulePathCases()
        {
            yield return Case(
                "de7b6e19-305b-4dc7-b761-9762afaadae2",
                "9b7bbbae-9771-46f2-9e4d-2489e0ffc702",
                "Configuration/SessionModule.bsl");
            yield return Case(
                "854f7300-9d83-4358-a8ae-eb33c9a42bbe",
                "3e58c91f-9aaa-4f42-8999-4baf33907b75",
                "Constants/Константа1/ValueManagerModule.bsl");
            yield return Case(
                "40d90f01-6080-49b4-a575-ceeabeb4c1b1",
                "d1b64a2c-8078-4982-8190-8f81aefda192",
                "CalculationRegisters/РегистрРасчета1/ManagerModule.bsl");
            yield return Case(
                "40d90f01-6080-49b4-a575-ceeabeb4c1b1",
                "9f36fd70-4bf4-47f6-b235-935f73aab43f",
                "CalculationRegisters/РегистрРасчета1/RecordSetModule.bsl");
            yield return Case(
                "3d66b766-54c8-423d-887d-6670cfc1b8cc",
                "9f36fd70-4bf4-47f6-b235-935f73aab43f",
                "CalculationRegisters/РегистрРасчета1/Recalculations/Перерасчет1/RecordSetModule.bsl");
            yield return Case(
                "52895f8c-2982-4f1d-bfb0-75b3dd5db3d3",
                "d5963243-262e-4398-b4d7-fb16d06484f6",
                "CommonModules/ОбщийМодульВызовСервера/Module.bsl");
            yield return Case(
                "a722a8f2-1ea9-4984-ae17-96eb28b01d15",
                "32e087ab-1491-49b6-aba7-43571b41ac2b",
                "CommonForms/Форма/Module.bsl");
            yield return Case(
                "0fcc2b09-6188-4573-843d-41e906660823",
                "32e087ab-1491-49b6-aba7-43571b41ac2b",
                "Tasks/Задача1/Forms/Форма/Module.bsl");
            yield return Case(
                "c8dc1e16-1c5d-49e2-a4a6-e4a0626f8221",
                "078a6af8-d22c-4248-9c33-7e90075a3d2c",
                "ExchangePlans/ПланОбмена1/Commands/Команда1/CommandModule.bsl");
            yield return Case(
                "4df936d5-34c4-4685-9821-04e311146955",
                "078a6af8-d22c-4248-9c33-7e90075a3d2c",
                "CommonCommands/ОбщаяКоманда1/CommandModule.bsl");
        }

        private static IEnumerable<TestCaseData> XmlExtensionModulePathCases()
        {
            yield return Case(
                "de483c0a-d17d-4fb6-be3e-573d4045ff79",
                "d1b64a2c-8078-4982-8190-8f81aefda192",
                "DataProcessors/Расш1_Обработка1/Ext/ManagerModule.bsl");
            yield return Case(
                "fc0be29c-c7ec-4e83-ac62-e1217160fa07",
                "d5963243-262e-4398-b4d7-fb16d06484f6",
                "CommonModules/ОбщийМодульСервер/Ext/Module.bsl");
        }

        private static IEnumerable<TestCaseData> EdtExtensionModulePathCases()
        {
            yield return Case(
                "de483c0a-d17d-4fb6-be3e-573d4045ff79",
                "d1b64a2c-8078-4982-8190-8f81aefda192",
                "DataProcessors/Расш1_Обработка1/ManagerModule.bsl");
            yield return Case(
                "fc0be29c-c7ec-4e83-ac62-e1217160fa07",
                "d5963243-262e-4398-b4d7-fb16d06484f6",
                "CommonModules/ОбщийМодульСервер/Module.bsl");
        }

        private static IEnumerable<TestCaseData> XmlExternalModulePathCases()
        {
            yield return Case(
                "630ddfcc-52a6-4f8a-8eb9-417c5d39a7f1",
                "a637f77f-3840-441d-a1c3-699c8c5cb7e0",
                "ВнешняяОбработка1/Ext/ObjectModule.bsl");
            yield return Case(
                "38e352a5-9e82-4e60-9244-b81cc3eed67e",
                "32e087ab-1491-49b6-aba7-43571b41ac2b",
                "ВнешняяОбработка1/Forms/Форма/Ext/Form/Module.bsl");
        }

        private static IEnumerable<TestCaseData> EdtExternalModulePathCases()
        {
            yield return Case(
                "630ddfcc-52a6-4f8a-8eb9-417c5d39a7f1",
                "a637f77f-3840-441d-a1c3-699c8c5cb7e0",
                "ВнешняяОбработка1/ObjectModule.bsl");
            yield return Case(
                "38e352a5-9e82-4e60-9244-b81cc3eed67e",
                "32e087ab-1491-49b6-aba7-43571b41ac2b",
                "ВнешняяОбработка1/Forms/Форма/Module.bsl");
        }

        private static TestCaseData Case(string objectId, string propertyId, string expectedRelativePath)
            => new TestCaseData(objectId, propertyId, expectedRelativePath);

        private static void AssertModulePath(
            string projectPath,
            SourceFilesFormatEnum format,
            string moduleType,
            string objectId,
            string propertyId,
            string expectedRelativePath)
        {
            var moduleId = CreateModuleId(moduleType, objectId, propertyId);
            var actualPath = CoverageModulePathResolver.Resolve(
                moduleId,
                new SourceDirectoryData(projectPath, format));

            var resolver = format == SourceFilesFormatEnum.EDT
                ? (ICoverageModulePathResolver)new EdtCoverageModulePathResolver()
                : new XmlCoverageModulePathResolver();
            var expectedPath = Path.Combine(resolver.GetSourceRoot(projectPath), expectedRelativePath);

            Assert.That(actualPath, Is.Not.Null);
            Assert.That(
                Path.GetFullPath(actualPath!),
                Is.EqualTo(Path.GetFullPath(expectedPath)));
        }

        private static CoverageModuleIdWrapper CreateModuleId(string moduleType, string objectId, string propertyId)
        {
            var moduleId = CoverageModuleIdWrapper.ScriptConstructor();
            moduleId.ModuleType = moduleType;
            moduleId.ObjectID = objectId;
            moduleId.PropertyID = propertyId;
            return moduleId;
        }
    }
}
