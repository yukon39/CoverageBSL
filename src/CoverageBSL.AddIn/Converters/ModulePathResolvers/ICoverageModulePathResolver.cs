using com.github.yukon39.CoverageBSL.AddIn.Debugger;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers
{
    internal interface ICoverageModulePathResolver
    {
        SourceFilesFormatEnum Format { get; }

        string MetadataFileExtension { get; }

        string GetSourceRoot(string projectPath);

        string Resolve(CoverageModuleIdWrapper moduleId, string projectPath);

        string FindMetadataFile(string sourcePath, string objectId, string subDirectory);

        string FindMetadataFileInDirectories(string sourcePath, string objectId, IEnumerable<string> directories);

        string FindNestedChildByUuid(string sourcePath, string objectId, string directory, string childDirectory);

        string FindNestedChildByUuid(string sourcePath, string objectId, IEnumerable<string> directories, string childDirectory);
    }
}
