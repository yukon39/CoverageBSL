using OneScript.Contexts.Enums;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [EnumerationType("CoverageDataFormat", "ФорматДанныхПокрытия")]
    public enum CoverageDataFormatEnum
    {
        [EnumValue("RAW")]
        RAW,

        [EnumValue("GenericCoverage")]
        GenericCoverage
    }
}
