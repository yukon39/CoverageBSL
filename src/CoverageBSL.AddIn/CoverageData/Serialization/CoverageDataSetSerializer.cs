using com.github.yukon39.CoverageBSL.AddIn;
using System;

namespace com.github.yukon39.CoverageBSL.AddIn.CoverageData.Serialization
{
    internal static class CoverageDataSetSerializer
    {
        public static void Write(CoverageDataSet dataSet, string filePath, CoverageDataFormatEnum format)
        {
            switch (format)
            {
                case CoverageDataFormatEnum.RAW:
                    RawCoverageDataSetSerializer.Write(dataSet, filePath);
                    break;
                case CoverageDataFormatEnum.GenericCoverage:
                    GenericCoverageDataSetSerializer.Write(dataSet, filePath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public static CoverageDataSet Read(string filePath, CoverageDataFormatEnum format)
        {
            return format switch
            {
                CoverageDataFormatEnum.RAW => RawCoverageDataSetSerializer.Read(filePath),
                CoverageDataFormatEnum.GenericCoverage => GenericCoverageDataSetSerializer.Read(filePath),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }
    }
}
