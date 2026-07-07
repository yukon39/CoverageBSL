using System;

namespace com.github.yukon39.CoverageBSL.AddIn.Converters
{
    internal static class CoverageModuleUrlHelper
    {
        public static bool Matches(string moduleUrl, string sourceModuleUrl)
        {
            if (string.IsNullOrWhiteSpace(moduleUrl) || string.IsNullOrWhiteSpace(sourceModuleUrl))
            {
                return false;
            }

            return string.Equals(Normalize(moduleUrl), Normalize(sourceModuleUrl), StringComparison.OrdinalIgnoreCase);
        }

        public static string Normalize(string moduleUrl)
        {
            if (string.IsNullOrWhiteSpace(moduleUrl))
            {
                return string.Empty;
            }

            if (moduleUrl.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
            {
                return moduleUrl;
            }

            return "file://" + moduleUrl;
        }
    }
}
