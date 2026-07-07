using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using NUnit.Framework;
using OneScript.StandardLibrary.Json;
using System.IO;
using System.Reflection;

namespace com.github.yukon39.CoverageBSL.AddIn.Tests
{
    public class CoverageDataContextClassTest
    {
        [Test]
        public void DeserializeJson_ReadsCoverageFromJsonFile()
        {
            var filePath = ResourcePath("coverage.json");
            var reader = JSONReader.Constructor();
            reader.OpenFile(filePath);

            var coverageData = CoverageDataContextClass.ScriptConstructor();
            coverageData.DeserializeJson(reader);

            Assert.That(coverageData.TotalDurability, Is.EqualTo(9407954));
            Assert.That(coverageData.Data.Count(), Is.GreaterThan(0));

            var secondModule = GetSecondModule(coverageData);
            Assert.That(secondModule.ModuleId.ModuleType, Is.EqualTo("ConfigModule"));
            Assert.That(secondModule.ModuleId.ObjectID, Is.EqualTo("40d90f01-6080-49b4-a575-ceeabeb4c1b1"));
            Assert.That(secondModule.ModuleId.PropertyID, Is.EqualTo("d1b64a2c-8078-4982-8190-8f81aefda192"));

            var secondLine = GetSecondLine(secondModule);
            Assert.That(secondLine.LineNo, Is.EqualTo(6));
            Assert.That(secondLine.Frequency, Is.EqualTo(1));
            Assert.That(secondLine.Durability, Is.EqualTo(39));
            Assert.That(secondLine.PureDurability, Is.EqualTo(39));
        }

        private static string ResourcePath(string fixtureName) =>
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "Resources",
                fixtureName);

        private static CoverageModuleInfoContextClass GetSecondModule(CoverageDataContextClass coverageData)
        {
            var index = 0;
            foreach (CoverageModuleInfoContextClass moduleInfo in coverageData.Data)
            {
                index++;
                if (index == 2)
                {
                    return moduleInfo;
                }
            }

            Assert.Fail("Coverage data does not contain second module.");
            return null!;
        }

        private static CoverageLineInfoWrapper GetSecondLine(CoverageModuleInfoContextClass moduleInfo)
        {
            var index = 0;
            foreach (CoverageLineInfoWrapper lineInfo in moduleInfo.LineInfo)
            {
                index++;
                if (index == 2)
                {
                    return lineInfo;
                }
            }

            Assert.Fail("Coverage module does not contain second line.");
            return null!;
        }
    }
}
