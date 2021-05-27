using com.github.yukon39.DebugBSL.debugger.debugBaseData;
using System;
using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL
{
    public interface ICoverageSession
    {
        AttachDebugUIResult Attach(char[] password);
        Task<AttachDebugUIResult> AttachAsync(char[] password);

        void Detach();
        Task DetachAsync();

        Guid StartCoverageCapture();
        Task<Guid> StartCoverageCaptureAsync();

        ICoverageData StopCoverageCapture();
        Task<ICoverageData> StopCoverageCaptureAsync();
    }
}
