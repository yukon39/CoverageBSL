using com.github.yukon39.CoverageBSL.AddIn.Converters;
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

        [TestCaseSource(nameof(XmlModulePathCases))]
        public void Resolve_Xml_ReturnsExpectedModulePath(string objectId, string propertyId, string expectedRelativePath)
        {
            AssertModulePath(XmlProjectPath, SourceFilesFormatEnum.XML, objectId, propertyId, expectedRelativePath);
        }

        [TestCaseSource(nameof(EdtModulePathCases))]
        public void Resolve_Edt_ReturnsExpectedModulePath(string objectId, string propertyId, string expectedRelativePath)
        {
            AssertModulePath(EdtProjectPath, SourceFilesFormatEnum.EDT, objectId, propertyId, expectedRelativePath);
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
                "52895f8c-2982-4f1d-bfb0-75b3dd5db3d3",
                "d5963243-262e-4398-b4d7-fb16d06484f6",
                "CommonModules/ОбщийМодульВызовСервера/Module.bsl");
            yield return Case(
                "a722a8f2-1ea9-4984-ae17-96eb28b01d15",
                "32e087ab-1491-49b6-aba7-43571b41ac2b",
                "CommonForms/Форма/Module.bsl");
        }

        private static TestCaseData Case(string objectId, string propertyId, string expectedRelativePath)
            => new TestCaseData(objectId, propertyId, expectedRelativePath);

        private static void AssertModulePath(
            string projectPath,
            SourceFilesFormatEnum format,
            string objectId,
            string propertyId,
            string expectedRelativePath)
        {
            var moduleId = CreateModuleId(objectId, propertyId);
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

        private static CoverageModuleIdWrapper CreateModuleId(string objectId, string propertyId)
        {
            var moduleId = CoverageModuleIdWrapper.ScriptConstructor();
            moduleId.ModuleType = "ConfigModule";
            moduleId.ObjectID = objectId;
            moduleId.PropertyID = propertyId;
            return moduleId;
        }
    }
}
