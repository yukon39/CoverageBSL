using com.github.yukon39.CoverageBSL.data.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL.data
{
    public static class ErrorProcessingManager
    {
        public static string BriefErrorDescription(GenericException genericException) => genericException.Description;
    }
}
