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

        public static string SourcesPath =>
            Path.Combine(ResourcesDirectory, "sources");

        public static string ExtensionXmlSourcesPath =>
            Path.Combine(ResourcesDirectory, "sources", "cfe", "xml");

        public static string ExtensionEdtProjectPath =>
            Path.Combine(ResourcesDirectory, "sources", "cfe", "edt");

        public static string ExternalXmlSourcesPath =>
            Path.Combine(ResourcesDirectory, "sources", "epf", "xml");

        public static string ExternalEdtProjectPath =>
            Path.Combine(ResourcesDirectory, "sources", "epf", "edt");
    }
}
