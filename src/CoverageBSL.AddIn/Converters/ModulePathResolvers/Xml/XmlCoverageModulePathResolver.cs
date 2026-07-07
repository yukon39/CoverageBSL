using com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers;
using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers.Xml
{
    internal sealed class XmlCoverageModulePathResolver : CoverageModulePathResolverBase
    {
        private static readonly XmlReaderSettings MetadataReaderSettings = new()
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            DtdProcessing = DtdProcessing.Prohibit,
            CloseInput = true,
        };

        public override SourceFilesFormatEnum Format => SourceFilesFormatEnum.XML;

        protected override string MetadataFileExtension => ".xml";

        public override string GetSourceRoot(string projectPath) => projectPath;

        protected override string ResolveConfigModule(CoverageModuleIdWrapper moduleId, string sourcePath)
        {
            var kind = moduleId.PropertyID.FromPropertyId();
            var fileOrDirectory = FindMetadataPath(sourcePath, moduleId.ObjectID, kind);
            if (fileOrDirectory == null)
            {
                return null;
            }

            return kind switch
            {
                CoverageModuleKind.ManagedApplicationModule => Path.Combine(sourcePath, "Ext", "ManagedApplicationModule.bsl"),
                CoverageModuleKind.SessionModule => Path.Combine(sourcePath, "Ext", "SessionModule.bsl"),
                CoverageModuleKind.ExternalConnectionModule => Path.Combine(sourcePath, "Ext", "ExternalConnectionModule.bsl"),
                CoverageModuleKind.OrdinaryApplicationModule => Path.Combine(sourcePath, "Ext", "OrdinaryApplicationModule.bsl"),
                CoverageModuleKind.CommonModule => Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, Path.GetFileNameWithoutExtension(fileOrDirectory), "Ext", "Module.bsl"),
                CoverageModuleKind.ManagerModule => Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, Path.GetFileNameWithoutExtension(fileOrDirectory), "Ext", "ManagerModule.bsl"),
                CoverageModuleKind.ObjectModule => Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, Path.GetFileNameWithoutExtension(fileOrDirectory), "Ext", "ObjectModule.bsl"),
                CoverageModuleKind.RecordSetModule => ResolveRecordSetPath(fileOrDirectory),
                CoverageModuleKind.ValueManagerModule => Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, Path.GetFileNameWithoutExtension(fileOrDirectory), "Ext", "ValueManagerModule.bsl"),
                CoverageModuleKind.SettingsManagerModule => Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, Path.GetFileNameWithoutExtension(fileOrDirectory), "Ext", "ManagerModule.bsl"),
                CoverageModuleKind.FormModule => ResolveFormPath(fileOrDirectory),
                CoverageModuleKind.CommandModule => ResolveCommandPath(fileOrDirectory),
                _ => null
            };
        }

        protected override string ResolveExternalModule(CoverageModuleIdWrapper moduleId, string sourcePath)
        {
            var kind = moduleId.PropertyID.FromPropertyId();

            return kind switch
            {
                CoverageModuleKind.ObjectModule => ResolveExternalObjectModule(sourcePath, moduleId.ObjectID),
                CoverageModuleKind.FormModule => ResolveExternalFormModule(sourcePath, moduleId.ObjectID),
                _ => null
            };
        }

        private string ResolveExternalObjectModule(string sourcePath, string objectId)
        {
            var metadataFile = FindMetadataFile(sourcePath, objectId, ".");
            if (metadataFile == null)
            {
                return null;
            }

            return Path.Combine(
                Path.GetDirectoryName(metadataFile)!,
                Path.GetFileNameWithoutExtension(metadataFile),
                "Ext",
                "ObjectModule.bsl");
        }

        private string ResolveExternalFormModule(string sourcePath, string objectId)
        {
            var metadataFile = FindNestedChildByUuid(sourcePath, objectId, ".", "Forms");
            return metadataFile == null ? null : ResolveFormPath(metadataFile);
        }

        private string FindMetadataFile(string sourcePath, string objectId, string subDirectory)
        {
            var searchRoot = Path.GetFullPath(Path.Combine(sourcePath, subDirectory));
            if (!Directory.Exists(searchRoot))
            {
                return null;
            }

            if (!MetadataUuidIndex.TryGetFilePath(sourcePath, MetadataFileExtension, objectId, out var filePath))
            {
                return null;
            }

            if (!MetadataPathHelper.IsDirectChild(filePath, searchRoot))
            {
                return null;
            }

            return MetadataPathHelper.HasExtension(filePath, MetadataFileExtension) ? filePath : null;
        }

        private string FindMetadataFileInDirectories(string sourcePath, string objectId, IEnumerable<string> directories)
        {
            foreach (var directory in directories)
            {
                var filePath = FindMetadataFile(sourcePath, objectId, directory);
                if (filePath != null)
                {
                    return filePath;
                }
            }

            return null;
        }

        private string FindMetadataPath(string sourcePath, string objectId, CoverageModuleKind kind)
        {
            return kind switch
            {
                CoverageModuleKind.ManagedApplicationModule or
                CoverageModuleKind.SessionModule or
                CoverageModuleKind.ExternalConnectionModule or
                CoverageModuleKind.OrdinaryApplicationModule => FindMetadataFile(sourcePath, objectId, "."),
                CoverageModuleKind.CommonModule => FindMetadataFileInDirectories(sourcePath, objectId, CoverageModuleDirectories.CommonModuleDirectories),
                CoverageModuleKind.ManagerModule => FindMetadataFileInDirectories(sourcePath, objectId, CoverageModuleDirectories.ManagerModuleDirectories),
                CoverageModuleKind.ObjectModule => FindMetadataFileInDirectories(sourcePath, objectId, CoverageModuleDirectories.ObjectModuleDirectories),
                CoverageModuleKind.RecordSetModule => FindRecordSetTarget(sourcePath, objectId),
                CoverageModuleKind.ValueManagerModule => FindMetadataFile(sourcePath, objectId, "Constants"),
                CoverageModuleKind.SettingsManagerModule => FindMetadataFile(sourcePath, objectId, "SettingsStorages"),
                CoverageModuleKind.FormModule => FindFormTarget(sourcePath, objectId),
                CoverageModuleKind.CommandModule => FindCommandTarget(sourcePath, objectId),
                _ => null
            };
        }

        private string FindRecordSetTarget(string sourcePath, string objectId)
        {
            var directFile = FindMetadataFileInDirectories(sourcePath, objectId, CoverageModuleDirectories.RecordSetModuleDirectories);
            if (directFile != null)
            {
                return directFile;
            }

            return FindNestedChildByUuid(sourcePath, objectId, "CalculationRegisters", "Recalculations");
        }

        private string FindFormTarget(string sourcePath, string objectId)
        {
            var directFile = FindNestedChildByUuid(sourcePath, objectId, CoverageModuleDirectories.FormModuleDirectories, "Forms");
            return directFile ?? FindMetadataFile(sourcePath, objectId, "CommonForms");
        }

        private string FindCommandTarget(string sourcePath, string objectId)
        {
            var commandDirectory = FindNestedChildDirectory(
                sourcePath,
                objectId,
                CoverageModuleDirectories.CommandModuleDirectories,
                "Command",
                "Commands");
            if (commandDirectory != null)
            {
                return commandDirectory;
            }

            return FindMetadataFile(sourcePath, objectId, "CommonCommands");
        }

        private string FindNestedChildDirectory(
            string sourcePath,
            string objectId,
            IEnumerable<string> parentDirectories,
            string childElementLocalName,
            string childDirectorySegment)
        {
            foreach (var parentDirectory in parentDirectories)
            {
                var searchRoot = Path.Combine(sourcePath, parentDirectory);
                if (!Directory.Exists(searchRoot))
                {
                    continue;
                }

                foreach (var metadataFile in Directory.EnumerateFiles(searchRoot, "*.xml", SearchOption.AllDirectories))
                {
                    if (!TryReadNestedChildName(metadataFile, objectId, childElementLocalName, out var childName))
                    {
                        continue;
                    }

                    var objectDirectory = Path.Combine(
                        Path.GetDirectoryName(metadataFile)!,
                        Path.GetFileNameWithoutExtension(metadataFile));
                    var candidateDirectory = Path.Combine(objectDirectory, childDirectorySegment, childName);
                    if (!Directory.Exists(candidateDirectory))
                    {
                        continue;
                    }

                    return candidateDirectory;
                }
            }

            return null;
        }

        private static bool TryReadNestedChildName(
            string metadataFilePath,
            string objectId,
            string childElementLocalName,
            out string childName)
        {
            childName = null;

            try
            {
                using var reader = XmlReader.Create(metadataFilePath, MetadataReaderSettings);
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element
                        || !string.Equals(reader.LocalName, childElementLocalName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var uuid = reader.GetAttribute("uuid");
                    if (!string.Equals(uuid, objectId, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (reader.IsEmptyElement)
                    {
                        return false;
                    }

                    var elementDepth = reader.Depth;
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.EndElement
                            && reader.Depth == elementDepth
                            && string.Equals(reader.LocalName, childElementLocalName, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        if (reader.NodeType != XmlNodeType.Element
                            || !string.Equals(reader.LocalName, "Name", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        childName = reader.ReadElementContentAsString();
                        return !string.IsNullOrEmpty(childName);
                    }
                }
            }
            catch (XmlException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }

            return false;
        }

        private static string ResolveRecordSetPath(string filePath)
            => Path.Combine(
                Path.GetDirectoryName(filePath)!,
                Path.GetFileNameWithoutExtension(filePath),
                "Ext",
                "RecordSetModule.bsl");

        private static string ResolveFormPath(string filePath)
            => Path.Combine(
                Path.GetDirectoryName(filePath)!,
                Path.GetFileNameWithoutExtension(filePath),
                "Ext",
                "Form",
                "Module.bsl");

        private static string ResolveCommandPath(string fileOrDirectory)
        {
            if (Directory.Exists(fileOrDirectory))
            {
                return Path.Combine(fileOrDirectory, "Ext", "CommandModule.bsl");
            }

            return Path.Combine(
                Path.GetDirectoryName(fileOrDirectory)!,
                Path.GetFileNameWithoutExtension(fileOrDirectory),
                "Ext",
                "CommandModule.bsl");
        }
    }
}
