using com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers;
using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers.Edt
{
    internal sealed class EdtCoverageModulePathResolver : CoverageModulePathResolverBase, ICoverageModulePathResolver
    {
        private static readonly XmlReaderSettings MetadataReaderSettings = new()
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            DtdProcessing = DtdProcessing.Prohibit,
            CloseInput = true,
        };

        public override SourceFilesFormatEnum Format => SourceFilesFormatEnum.EDT;

        public string MetadataFileExtension => ".mdo";

        public override string GetSourceRoot(string projectPath) => Path.Combine(projectPath, "src");

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
                CoverageModuleKind.ManagedApplicationModule => Path.Combine(sourcePath, "Configuration", "ManagedApplicationModule.bsl"),
                CoverageModuleKind.SessionModule => Path.Combine(sourcePath, "Configuration", "SessionModule.bsl"),
                CoverageModuleKind.ExternalConnectionModule => Path.Combine(sourcePath, "Configuration", "ExternalConnectionModule.bsl"),
                CoverageModuleKind.OrdinaryApplicationModule => Path.Combine(sourcePath, "Configuration", "OrdinaryApplicationModule.bsl"),
                CoverageModuleKind.CommonModule => Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, "Module.bsl"),
                CoverageModuleKind.ManagerModule => Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, "ManagerModule.bsl"),
                CoverageModuleKind.ObjectModule => Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, "ObjectModule.bsl"),
                CoverageModuleKind.RecordSetModule => ResolveRecordSetPath(fileOrDirectory),
                CoverageModuleKind.ValueManagerModule => Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, "ValueManagerModule.bsl"),
                CoverageModuleKind.SettingsManagerModule => Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, "ManagerModule.bsl"),
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
            return metadataFile == null
                ? null
                : Path.Combine(Path.GetDirectoryName(metadataFile)!, "ObjectModule.bsl");
        }

        private string ResolveExternalFormModule(string sourcePath, string objectId)
        {
            var formDirectory = FindNestedChildDirectory(sourcePath, objectId, ["."], "forms", "Forms");
            return formDirectory == null ? null : ResolveFormPath(formDirectory);
        }

        public string FindMetadataFile(string sourcePath, string objectId, string subDirectory)
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

            if (!MetadataPathHelper.IsUnderDirectory(filePath, searchRoot))
            {
                return null;
            }

            return MetadataPathHelper.HasExtension(filePath, MetadataFileExtension) ? filePath : null;
        }

        public string FindMetadataFileInDirectories(string sourcePath, string objectId, IEnumerable<string> directories)
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

        public string FindNestedChildByUuid(string sourcePath, string objectId, IEnumerable<string> directories, string childDirectory)
        {
            foreach (var directory in directories)
            {
                var path = FindNestedChildByUuid(sourcePath, objectId, directory, childDirectory);
                if (path != null)
                {
                    return path;
                }
            }

            return null;
        }

        public string FindNestedChildByUuid(string sourcePath, string objectId, string directory, string childDirectory)
        {
            if (!MetadataUuidIndex.TryGetFilePath(sourcePath, MetadataFileExtension, objectId, out var filePath))
            {
                return null;
            }

            if (!MetadataPathHelper.HasExtension(filePath, MetadataFileExtension))
            {
                return null;
            }

            var searchRoot = Path.Combine(sourcePath, directory);
            if (!MetadataPathHelper.IsUnderDirectory(filePath, searchRoot))
            {
                return null;
            }

            return MetadataPathHelper.ContainsDirectorySegment(filePath, childDirectory) ? filePath : null;
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

            return FindNestedChildDirectory(
                sourcePath,
                objectId,
                ["CalculationRegisters"],
                "recalculations",
                "Recalculations");
        }

        private string FindFormTarget(string sourcePath, string objectId)
        {
            var directFile = FindNestedChildByUuid(sourcePath, objectId, CoverageModuleDirectories.FormModuleDirectories, "Forms");
            if (directFile != null)
            {
                return directFile;
            }

            var formDirectory = FindNestedChildDirectory(
                sourcePath,
                objectId,
                CoverageModuleDirectories.FormModuleDirectories,
                "forms",
                "Forms");
            if (formDirectory != null)
            {
                return formDirectory;
            }

            return FindMetadataFile(sourcePath, objectId, "CommonForms");
        }

        private string FindCommandTarget(string sourcePath, string objectId)
        {
            var commandDirectory = FindNestedChildDirectory(
                sourcePath,
                objectId,
                CoverageModuleDirectories.CommandModuleDirectories,
                "commands",
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

                foreach (var metadataFile in Directory.EnumerateFiles(searchRoot, "*.mdo", SearchOption.AllDirectories))
                {
                    if (!TryReadNestedChildName(metadataFile, objectId, childElementLocalName, out var childName))
                    {
                        continue;
                    }

                    var candidatePath = Path.Combine(
                        Path.GetDirectoryName(metadataFile)!,
                        childDirectorySegment,
                        childName);
                    if (!Directory.Exists(candidatePath))
                    {
                        continue;
                    }

                    return candidatePath;
                }
            }

            return null;
        }

        private static string ResolveRecordSetPath(string fileOrDirectory)
        {
            if (Directory.Exists(fileOrDirectory))
            {
                return Path.Combine(fileOrDirectory, "RecordSetModule.bsl");
            }

            return Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, "RecordSetModule.bsl");
        }

        private static string ResolveFormPath(string fileOrDirectory)
        {
            if (Directory.Exists(fileOrDirectory))
            {
                return Path.Combine(fileOrDirectory, "Module.bsl");
            }

            return Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, "Module.bsl");
        }

        private static string ResolveCommandPath(string fileOrDirectory)
        {
            if (Directory.Exists(fileOrDirectory))
            {
                return Path.Combine(fileOrDirectory, "CommandModule.bsl");
            }

            return Path.Combine(Path.GetDirectoryName(fileOrDirectory)!, "CommandModule.bsl");
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

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.EndElement
                            && string.Equals(reader.LocalName, childElementLocalName, StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }

                        if (reader.NodeType != XmlNodeType.Element
                            || !string.Equals(reader.LocalName, "name", StringComparison.OrdinalIgnoreCase))
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
    }
}
