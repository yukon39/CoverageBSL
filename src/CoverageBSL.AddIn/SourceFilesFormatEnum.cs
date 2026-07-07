using OneScript.Contexts.Enums;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [EnumerationType("SourceFilesFormat", "ФорматыИсходныхФайлов")]
    public enum SourceFilesFormatEnum
    {
        [EnumValue("EDT")]
        EDT,

        [EnumValue("XML")]
        XML
    }
}
