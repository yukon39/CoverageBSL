using com.github.yukon39.CoverageBSL.data.core;
using com.github.yukon39.CoverageBSL.httpDebug;
using NUnit.Framework;
using System;
using System.IO;

namespace CoverageBSL.Tests.data.core
{
    class CoreExceptionTest
    {
        [Test]
        public void TestDeserialization()
        {
            // Given
            var XmlFile = Path.Join(TestContext.CurrentContext.TestDirectory,
                    "data", "core", "CoreExceptionTest.xml");
            var XmlString = File.ReadAllText(XmlFile);

            // When
            var coreException = HTTPDebugSerializer.Deserialize<CoreException>(XmlString, null);

            // Then
            Assert.AreEqual(coreException.CLSID, Guid.Parse("5372caa7-07b9-4767-9776-53b510236d93"));
        }
    }
}
