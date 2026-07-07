using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using System.Collections.Generic;
using System.IO;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal sealed class EdtCoverageModulePathResolver : CoverageModulePathResolverBase, ICoverageModulePathResolver
    {
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

            return FindNestedChildByUuid(sourcePath, objectId, "CalculationRegisters", "Recalculations");
        }

        private string FindFormTarget(string sourcePath, string objectId)
        {
            var directFile = FindNestedChildByUuid(sourcePath, objectId, CoverageModuleDirectories.FormModuleDirectories, "Forms");
            return directFile ?? FindMetadataFile(sourcePath, objectId, "CommonForms");
        }

        private string FindCommandTarget(string sourcePath, string objectId)
        {
            var directFile = FindNestedChildByUuid(sourcePath, objectId, CoverageModuleDirectories.CommandModuleDirectories, "Commands");
            return directFile ?? FindMetadataFile(sourcePath, objectId, "CommonCommands");
        }

        private static string ResolveRecordSetPath(string filePath)
            => Path.Combine(Path.GetDirectoryName(filePath)!, "RecordSetModule.bsl");

        private static string ResolveFormPath(string filePath)
            => Path.Combine(Path.GetDirectoryName(filePath)!, "Module.bsl");

        private static string ResolveCommandPath(string filePath)
            => Path.Combine(Path.GetDirectoryName(filePath)!, "CommandModule.bsl");
    }
}
