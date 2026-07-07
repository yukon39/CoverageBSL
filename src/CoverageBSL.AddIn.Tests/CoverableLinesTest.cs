using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Internal;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace com.github.yukon39.CoverageBSL.AddIn.Tests
{
    public class CoverableLinesTest
    {
        [TestCase("simple.bsl", new[]
        {
            3, 8, 15, 18, 19, 20, 21, 23, 24, 29, 30, 31, 32, 33, 34, 35, 36, 37, 41, 42, 48,
            50, 53, 57, 60, 63, 66, 67, 68, 69, 70, 72
        })]
        [TestCase("tocover.bsl", new[]
        {
            13, 17, 19, 22, 23, 24, 27, 29, 30, 31, 33, 34, 35, 38, 39, 41, 42, 45, 46, 50, 51, 52, 54,
            56, 58, 60, 66, 67, 69, 70, 71, 73, 74, 75, 76, 78, 79, 80, 81, 82, 84, 92, 93, 94, 98, 103, 105,
            112
        })]
        [TestCase("error.bsl", new[] { 4, 6, 8 })]
        [TestCase("if.bsl", new[] { 3, 8, 10, 16, 17, 19, 21, 22, 23 })]
        [TestCase("assigment.bsl", new[]
        {
            5, 11, 18, 22, 26, 32, 38, 44, 46, 52, 58, 63, 68, 70, 79, 84, 92, 102, 106, 110, 114,
            117, 121, 125, 127, 128
        })]
        [TestCase("do.bsl", new[]
        {
            4, 5, 6, 7, 8, 12, 16, 22, 23, 25, 30, 31, 32, 33, 34, 35, 36, 37, 39, 43, 44, 53, 54,
            56, 61, 64, 65, 66, 67, 68, 70, 75, 76, 77, 78, 79, 80, 82, 85, 86, 89, 95, 96, 97, 99, 102, 103, 106,
            107, 109, 110, 111, 112
        })]
        [TestCase("other.bsl", new[]
        {
            4, 10, 15, 18, 19, 21, 34, 48, 50, 53, 55, 59, 61, 62, 64, 65,
            70, 72, 76, 80, 84, 87, 91, 92, 94, 95, 96
        })]
        [TestCase("notcovered.bsl", new[]
        {
            6, 7, 8, 9, 13, 14, 16, 17, 19, 21, 22, 23
        })]
        [TestCase("filtered.bsl", new[]
        {
            3, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 23, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 62, 63, 65, 70, 73, 75, 77, 79, 81, 83, 85, 87, 89, 91, 93, 95, 97, 99, 101, 103, 105, 107, 109, 111, 113, 115, 117, 119, 122, 126, 128, 130, 132, 134, 135, 136, 137, 139, 143, 146, 149, 152, 155, 156, 157, 158
        })]
        [TestCase("opcode.bsl", new[]
        {
            3, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48, 50, 52, 54, 56, 58, 60, 62, 64, 66, 68, 70, 72, 74, 76, 78, 80, 82, 84, 86, 88, 90, 92, 94, 96, 98, 100, 102, 104, 106, 108, 111, 113, 115, 117, 119, 121, 123, 125, 127, 129, 131, 133, 135, 137, 139, 141, 143, 145, 147, 149, 152, 153, 155, 156, 159, 161
        })]
        public void BslFixture_ReturnsExpectedCoverableLines(string fixtureName, int[] expected)
        {
            var sourcePath = ResourcePath(fixtureName);
            var actual = ToLineNumbers(CoverableLinesProvider.FromFile(sourcePath));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void FromSource_ForEachSplitAcrossSeveralLines_UsesForLine()
        {
            var source = """
                Процедура Проверить()
                    Для


                    Каждого Элемент Из Массив
                    Цикл
                    КонецЦикла;
                КонецПроцедуры
                """;

            var actual = ToLineNumbers(CoverableLinesProvider.FromSource(source, "inline.bsl"));

            Assert.That(actual, Contains.Item(2));
            Assert.That(actual, Does.Not.Contain(5));
            Assert.That(actual, Contains.Item(6));
        }

        private static string ResourcePath(string fixtureName) =>
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                "Resources",
                fixtureName);

        private static int[] ToLineNumbers(CoverageLineInfosContextClass coverableLines)
        {
            var lineNumbers = new List<int>();
            foreach (CoverageLineInfoWrapper lineInfo in coverableLines)
            {
                lineNumbers.Add(lineInfo.LineNo);
            }

            return lineNumbers.ToArray();
        }
    }
}
