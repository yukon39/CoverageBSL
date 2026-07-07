using com.github.yukon39.CoverageBSL.AddIn.Debugger;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal static class CoverageDataSetModuleKeyBuilder
    {
        public static string Build(CoverageModuleIdWrapper moduleId)
        {
            if (!string.IsNullOrWhiteSpace(moduleId.URL))
            {
                return moduleId.URL;
            }

            return string.Join("|",
                moduleId.ModuleType,
                moduleId.ExtensionName,
                moduleId.ObjectID,
                moduleId.PropertyID);
        }
    }
}
