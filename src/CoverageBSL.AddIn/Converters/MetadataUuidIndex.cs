using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal static class MetadataUuidIndex
    {
        private static readonly XmlReaderSettings ReaderSettings = new()
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            DtdProcessing = DtdProcessing.Prohibit,
            CloseInput = true,
        };

        private readonly record struct IndexKey(string SourceRoot, string MetadataFileExtension);

        private static readonly ConcurrentDictionary<IndexKey, IReadOnlyDictionary<string, string>> Cache = new();

        public static bool TryGetFilePath(
            string sourceRoot,
            string metadataFileExtension,
            string objectId,
            out string filePath)
        {
            filePath = null;
            if (string.IsNullOrEmpty(objectId))
            {
                return false;
            }

            var key = new IndexKey(Path.GetFullPath(sourceRoot), metadataFileExtension);
            var index = Cache.GetOrAdd(key, static indexKey => Build(indexKey.SourceRoot, indexKey.MetadataFileExtension));
            return index.TryGetValue(objectId, out filePath);
        }

        private static IReadOnlyDictionary<string, string> Build(string sourceRoot, string metadataFileExtension)
        {
            var index = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!Directory.Exists(sourceRoot))
            {
                return index;
            }

            foreach (var filePath in Directory.EnumerateFiles(
                sourceRoot,
                $"*{metadataFileExtension}",
                SearchOption.AllDirectories))
            {
                if (!TryReadRootUuid(filePath, out var uuid))
                {
                    continue;
                }

                index.TryAdd(uuid, filePath);
            }

            return index;
        }

        internal static bool TryReadRootUuid(string filePath, out string uuid)
        {
            uuid = null;

            try
            {
                using var reader = XmlReader.Create(filePath, ReaderSettings);
                reader.MoveToContent();
                if (reader.NodeType != XmlNodeType.Element)
                {
                    return false;
                }

                uuid = reader.GetAttribute("uuid");
                if (!string.IsNullOrEmpty(uuid))
                {
                    return true;
                }

                if (reader.IsEmptyElement || !reader.Read())
                {
                    return false;
                }

                while (reader.NodeType == XmlNodeType.Whitespace)
                {
                    if (!reader.Read())
                    {
                        return false;
                    }
                }

                if (reader.NodeType != XmlNodeType.Element)
                {
                    return false;
                }

                uuid = reader.GetAttribute("uuid");
                return !string.IsNullOrEmpty(uuid);
            }
            catch (XmlException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
