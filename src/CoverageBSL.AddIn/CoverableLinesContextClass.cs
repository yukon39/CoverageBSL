using com.github.yukon39.CoverageBSL.AddIn.Converters;
using com.github.yukon39.CoverageBSL.AddIn.CoverageData;
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
        private readonly List<SourceDirectoryData> sourceDirectories = [];
        private CoverageDataSet coverageData = new CoverageDataSet();

        private CoverableLinesContextClass(string workingArea)
            => this.workingArea = workingArea ?? string.Empty;

        [ScriptConstructor]
        public static CoverableLinesContextClass ScriptConstructor(string workingArea = "")
            => new(workingArea);

        [ContextProperty("WorkingArea", "РабочаяОбласть")]
        public string WorkingArea => workingArea;

        [ContextMethod("AddSourceDirectory", "ДобавитьКаталогИсходныхФайлов")]
        public void AddSourceDirectory(string path, SourceFilesFormatEnum format = SourceFilesFormatEnum.XML)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                var message = Locale.NStr(
                    "en = 'Source directory path is required';" +
                    "ru = 'Не указан путь к каталогу исходных файлов'");
                throw new RuntimeException(message);
            }

            sourceDirectories.Add(new SourceDirectoryData(path, format));
        }

        [ContextMethod("AddExtensionSourceDirectory", "ДобавитьКаталогИсходныхФайловРасширения")]
        public void AddExtensionSourceDirectory(
            string extensionName,
            string path,
            SourceFilesFormatEnum format = SourceFilesFormatEnum.XML)
        {
            if (string.IsNullOrWhiteSpace(extensionName))
            {
                var message = Locale.NStr(
                    "en = 'Extension name is required';" +
                    "ru = 'Не указано имя расширения'");
                throw new RuntimeException(message);
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                var message = Locale.NStr(
                    "en = 'Source directory path is required';" +
                    "ru = 'Не указан путь к каталогу исходных файлов'");
                throw new RuntimeException(message);
            }

            sourceDirectories.Add(new SourceDirectoryData(path, format, extensionName));
        }

        [ContextMethod("AddExternalModuleSourceDirectory", "ДобавитьКаталогИсходныхФайловВнешнегоМодуля")]
        public void AddExternalModuleSourceDirectory(
            string moduleUrl,
            string path,
            SourceFilesFormatEnum format = SourceFilesFormatEnum.XML)
        {
            if (string.IsNullOrWhiteSpace(moduleUrl))
            {
                var message = Locale.NStr(
                    "en = 'External module URL is required';" +
                    "ru = 'Не указан URL внешнего модуля'");
                throw new RuntimeException(message);
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                var message = Locale.NStr(
                    "en = 'Source directory path is required';" +
                    "ru = 'Не указан путь к каталогу исходных файлов'");
                throw new RuntimeException(message);
            }

            sourceDirectories.Add(new SourceDirectoryData(
                path,
                format,
                moduleUrl: CoverageModuleUrlHelper.Normalize(moduleUrl)));
        }

        [ContextMethod("BuildCoverableLines", "СформироватьСтрокиКПокрытию")]
        public void BuildCoverableLines()
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
                coverageData = CoverableLinesBuilder.Build(workingArea, sourceDirectories);
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
        public void FromFile(string sourcePath)
        {
            try
            {
                coverageData.GetOrAdd(sourcePath).AddLines(CoverableLinesProvider.FromFile(sourcePath));
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
        public void FromSource(string source, string location)
        {
            try
            {
                coverageData.GetOrAdd(location).AddLines(CoverableLinesProvider.FromSource(source, location));
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

        [ContextMethod("WriteToFile", "ЗаписатьВФайл")]
        public void WriteToFile(string filePath, CoverageDataFormatEnum format)
        {
            try
            {
                coverageData.WriteToFile(filePath, format);
            }
            catch (Exception e)
            {
                var message = Locale.NStr(
                    "en = 'Write coverage data error';" +
                    "ru = 'Ошибка записи данных покрытия'");
                Logger.Error(message, e);
                throw RuntimeExceptionFactory.NewException(message, e);
            }
        }

        [ContextMethod("ReadFromFile", "ПрочитатьИзФайла")]
        public void ReadFromFile(string filePath, CoverageDataFormatEnum format)
        {
            try
            {
                coverageData = CoverageDataSet.ReadFromFile(filePath, format);
            }
            catch (Exception e)
            {
                var message = Locale.NStr(
                    "en = 'Read coverage data error';" +
                    "ru = 'Ошибка чтения данных покрытия'");
                Logger.Error(message, e);
                throw RuntimeExceptionFactory.NewException(message, e);
            }
        }
    }
}
