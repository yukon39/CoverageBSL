using com.github.yukon39.CoverageBSL;
using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using com.github.yukon39.CoverageBSL.AddIn.Internal;
using com.github.yukon39.CoverageBSL.AddIn.Utils;
using OneScript.Commons;
using OneScript.Contexts;
using OneScript.Exceptions;
using ScriptEngine.Machine.Contexts;
using System;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn
{
    [ContextClass(typeName: "CoverableLines", typeAlias: "СтрокиКПокрытию")]
    public class CoverableLinesContextClass : AutoContext<CoverableLinesContextClass>
    {
        private readonly ICoverageData coverageData;

        private CoverableLinesContextClass(ICoverageData coverageData)
            => this.coverageData = coverageData;

        [ScriptConstructor]
        public static CoverableLinesContextClass ScriptConstructor() =>
            new CoverableLinesContextClass(null);

        [ContextMethod("FromFile", "ИзФайла")]
        public CoverageLineInfosContextClass FromFile(string sourcePath)
        {
            try
            {
                return CoverableLinesProvider.FromFile(sourcePath);
            }
            catch (Exception e)
            {
                var message = Locale.NStr(
                    "en = 'Get coverable lines from file error';" +
                    "ru = 'Ошибка получения строк к покрытию из файла'");
                Logger.Error(message, e);
                throw RuntimeExceptionFactory.NewException(message, e);
            }
        }

        [ContextMethod("FromSource", "ИзИсточника")]
        public CoverageLineInfosContextClass FromSource(string source, string location)
        {
            try
            {
                return CoverableLinesProvider.FromSource(source, location);
            }
            catch (Exception e)
            {
                var message = Locale.NStr(
                    "en = 'Get coverable lines from source error';" +
                    "ru = 'Ошибка получения строк к покрытию из источника'");
                Logger.Error(message, e);
                throw RuntimeExceptionFactory.NewException(message, e);
            }
        }

        [ContextMethod("PrepareModule", "ПодготовитьМодуль")]
        public CoverageDataContextClass PrepareModule(
            string moduleUrl,
            CoverageLineInfosContextClass coverableLines)
        {
            if (coverageData == null)
            {
                var message = Locale.NStr(
                    "en = 'Coverage data is required';" +
                    "ru = 'Не указаны данные покрытия'");
                throw new RuntimeException(message);
            }

            try
            {
                CoverableLinesPreparer.PrepareData(coverageData, moduleUrl, ToLineNumbers(coverableLines));
                return new CoverageDataContextClass(coverageData);
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

        [ContextMethod("PrepareModuleFromSource", "ПодготовитьМодульИзИсточника")]
        public CoverageDataContextClass PrepareModuleFromSource(string moduleUrl, string sourcePath)
            => PrepareModule(moduleUrl, FromFile(sourcePath));

        private static IEnumerable<int> ToLineNumbers(CoverageLineInfosContextClass coverableLines)
        {
            var lineNumbers = new List<int>();
            foreach (CoverageLineInfoWrapper lineInfo in coverableLines)
            {
                lineNumbers.Add(lineInfo.LineNo);
            }

            return lineNumbers;
        }
    }
}
