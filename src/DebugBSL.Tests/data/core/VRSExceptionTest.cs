﻿using com.github.yukon39.DebugBSL.data.core;
using NUnit.Framework;
using System;

namespace com.github.yukon39.DebugBSL.Tests.data.core
{
    class VRSExceptionTest
    {
        [Test]
        public void TestDeserialization()
        {
            // Given
            var xmlString = UtilsTest.XmlString("data", "core", "VRSExceptionTest.xml");

            // When
            var vrsException = DebuggerXmlSerializer.Deserialize<VRSException>(xmlString);

            // Then
            Assert.AreEqual(vrsException.CLSID, Guid.Parse("580392e6-ba49-4280-ac67-fcd6f2180121"));
            Assert.AreEqual(vrsException.Reason, 400);
        }
    }
}
