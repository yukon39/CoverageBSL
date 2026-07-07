using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace com.github.yukon39.CoverageBSL.AddIn.Tests
{
    public class CoverableLinesBuildTest
    {
        [Test]
        public void BuildCoverableLines_BuildsCoverableLinesWithoutErrors()
        {
            var resourcesPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "Resources");

            var builder = CoverableLinesContextClass.ScriptConstructor(resourcesPath);
            builder.AddSourceDirectory(".");

            Assert.DoesNotThrow((System.Action)(() => builder.BuildCoverableLines()));
        }
    }
}
