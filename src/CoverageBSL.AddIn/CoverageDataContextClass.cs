using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Internal;
using com.github.yukon39.CoverageBSL.AddIn.Utils;
using OneScript.Commons;
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
using ScriptEngine.Machine.Contexts;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [ContextClass(typeName: "CoverageData", typeAlias: "ДанныеПокрытия")]
    public class CoverageDataContextClass : AutoContext<CoverageDataContextClass>
    {
        private readonly ICoverageData coverageData;

        public CoverageDataContextClass(ICoverageData coverageData)
        {
            this.coverageData = coverageData;
            TotalDurability = coverageData.TotalDurability;
            Data = new CoverageModuleInfoList(coverageData.Data);
        }

        [ScriptConstructor]
        public static CoverageDataContextClass ScriptConstructor()
            => new CoverageDataContextClass();

        private CoverageDataContextClass()
            => Data = new CoverageModuleInfoList();

        [ContextProperty("TotalDurability", "ОбщаяПродолжительность")]
        public long TotalDurability { get; set; }

        [ContextProperty("Data", "Данные")]
        public CoverageModuleInfoList Data { get; private set; }

        [ContextMethod("SerializeJSON", "СериализоватьJSON")]
        public void SerializeJson(JSONWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(TotalDurability));
            JSONUtils.WriteValue(writer, TotalDurability);

            writer.WritePropertyName(nameof(Data));
            Data.SerializeJson(writer);

            writer.WriteEndObject();
        }

        [ContextMethod("PrepareModuleCoverableLines", "ПодготовитьСтрокиКПокрытию")]
        public void PrepareModuleCoverableLines(string moduleUrl, CoverageLineInfosContextClass coverableLines)
        {
            try
            {
                var lineNumbers = new List<int>();
                foreach (CoverageLineInfoWrapper lineInfo in coverableLines)
                {
                    lineNumbers.Add(lineInfo.LineNo);
                }

                if (coverageData != null)
                {
                    CoverableLinesPreparer.PrepareData(coverageData, moduleUrl, lineNumbers);
                }
            }
            catch (Exception e)
            {
                var message = Locale.NStr(
                    "en = 'Prepare coverable lines error';" +
                    "ru = 'Ошибка подготовки строк к покрытию'");
                Logger.Error(message, e);
                throw RuntimeExceptionFactory.NewException(message, e);
            }
        }

        [ContextMethod("PrepareModuleCoverableLinesFromSource", "ПодготовитьСтрокиКПокрытиюИзИсточника")]
        public void PrepareModuleCoverableLinesFromSource(string moduleUrl, string sourcePath)
        {
            try
            {
                var coverableLines = Internal.CoverableLinesProvider.FromFile(sourcePath);
                PrepareModuleCoverableLines(moduleUrl, coverableLines);
            }
            catch (Exception e)
            {
                var message = Locale.NStr(
                    "en = 'Prepare coverable lines from source error';" +
                    "ru = 'Ошибка подготовки строк к покрытию из источника'");
                Logger.Error(message, e);
                throw RuntimeExceptionFactory.NewException(message, e);
            }
        }
    }
}
