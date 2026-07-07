using System.IO;
using System.Reflection;

namespace com.github.yukon39.CoverageBSL.AddIn.Tests
{
    internal static class TestResourcePaths
    {
        private static string ResourcesDirectory =>
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Resources");

        public static string ResourcePath(string fixtureName) =>
            Path.Combine(ResourcesDirectory, fixtureName);

        public static string ConfigurationXmlSourcesPath =>
            Path.Combine(ResourcesDirectory, "sources", "cf", "xml");

        public static string ConfigurationEdtProjectPath =>
            Path.Combine(ResourcesDirectory, "sources", "cf", "edt");
    }
}
