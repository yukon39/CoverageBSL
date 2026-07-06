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
        private readonly string workingArea;
        private readonly List<string> sourceDirectories = [];

        private CoverableLinesContextClass(string workingArea)
            => this.workingArea = workingArea ?? string.Empty;

        [ScriptConstructor]
        public static CoverableLinesContextClass ScriptConstructor(string workingArea = "")
            => new(workingArea);

        [ContextProperty("WorkingArea", "РабочаяОбласть")]
        public string WorkingArea => workingArea;

        [ContextMethod("AddSourceDirectory", "ДобавитьКаталогИсходныхФайлов")]
        public void AddSourceDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                var message = Locale.NStr(
                    "en = 'Source directory path is required';" +
                    "ru = 'Не указан путь к каталогу исходных файлов'");
                throw new RuntimeException(message);
            }

            sourceDirectories.Add(path);
        }

        [ContextMethod("Build", "Сформировать")]
        public CoverageDataContextClass Build()
        {
            if (sourceDirectories.Count == 0)
            {
                var message = Locale.NStr(
                    "en = 'Source directories are required';" +
                    "ru = 'Не указаны каталоги исходных файлов'");
                throw new RuntimeException(message);
            }

            try
            {
                var coverageData = CoverableLinesBuilder.Build(workingArea, sourceDirectories);
                return new CoverageDataContextClass(coverageData);
            }
            catch (Exception e)
            {
                var message = Locale.NStr(
                    "en = 'Build coverable lines error';" +
                    "ru = 'Ошибка формирования строк к покрытию'");
                Logger.Error(message, e);
                throw RuntimeExceptionFactory.NewException(message, e);
            }
        }

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
    }
}
