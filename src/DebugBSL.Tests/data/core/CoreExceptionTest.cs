using com.github.yukon39.DebugBSL.data.core;
using NUnit.Framework;
using System;

namespace com.github.yukon39.DebugBSL.Tests.data.core
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
            Assert.That(coreException.CLSID, Is.EqualTo(Guid.Parse("5372caa7-07b9-4767-9776-53b510236d93")));
        }
    }
}
