using com.github.yukon39.CoverageBSL.AddIn.Debugger;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters.ModulePathResolvers
{
    internal interface ICoverageModulePathResolver
    {
        string GetSourceRoot(string projectPath);

        string Resolve(CoverageModuleIdWrapper moduleId, string projectPath);
    }
}
