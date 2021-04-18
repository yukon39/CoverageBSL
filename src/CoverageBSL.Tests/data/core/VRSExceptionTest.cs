using com.github.yukon39.CoverageBSL.data.core;
using com.github.yukon39.CoverageBSL.httpDebug;
using NUnit.Framework;
using System;

namespace CoverageBSL.Tests.data.core
{
    class VRSExceptionTest
    {
        [Test]
        public void TestDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("data", "core", "VRSExceptionTest.xml");

            // When
            var vrsException = HTTPDebugSerializer.Deserialize<VRSException>(xmlString);

            // Then
            Assert.AreEqual(vrsException.CLSID, Guid.Parse("580392e6-ba49-4280-ac67-fcd6f2180121"));
            Assert.AreEqual(vrsException.Reason, 400);
        }
    }
}
