using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace com.github.yukon39.CoverageBSL.AddIn.Tests
{
    public class CoverableLinesBuildTest
    {
        [Test]
        public void Build_ReturnsCoverageDataForBslFiles()
        {
            var resourcesPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "Resources");

            var builder = CoverableLinesContextClass.ScriptConstructor(resourcesPath);
            builder.AddSourceDirectory(".");

            var coverageData = builder.Build();

            Assert.That(coverageData.Data.Count(), Is.GreaterThan(0));

            var totalLines = 0;
            foreach (CoverageModuleInfoContextClass moduleInfo in coverageData.Data)
            {
                Assert.That(moduleInfo.ModuleId.URL, Is.Not.Empty);
                totalLines += moduleInfo.LineInfo.Count();
            }

            Assert.That(totalLines, Is.GreaterThan(0));
        }
    }
}
