using NUnit.Framework;
using System.IO;

namespace com.github.yukon39.DebugBSL.Tests
{
    static class UtilsTest
    {
        public static string XmlString(string path1, string path2, string path3)
        {
            var testDirectory = TestContext.CurrentContext.TestDirectory;
            var xmlFile = Path.Combine(testDirectory, path1, path2, path3);
            var xmlString = File.ReadAllText(xmlFile);

            return xmlString;
        }
    }
}
