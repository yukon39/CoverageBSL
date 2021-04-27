using com.github.yukon39.DebugBSL;
using com.github.yukon39.DebugBSL.data.core;
using NUnit.Framework;
using System;

namespace CoverageBSL.Tests.data.core
{
    class CoreExceptionTest
    {
        [Test]
        public void TestDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("data", "core", "CoreExceptionTest.xml");

            // When
            var coreException = DebuggerXmlSerializer.Deserialize<CoreException>(xmlString);

            // Then
            Assert.AreEqual(coreException.CLSID, Guid.Parse("5372caa7-07b9-4767-9776-53b510236d93"));
        }
    }
}
