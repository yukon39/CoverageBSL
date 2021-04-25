using com.github.yukon39.DebugBSL.debugger.debugRDBGRequestResponse;
using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL
{
    public interface IDebuggerClient
    {
        Task TestAsync();

        Task<string> ApiVersionAsync();

        Task<T> ExecuteAsync<T>(IRDBGRequest request, IDebuggerClientRequestParameters parameters) where T : IRDBGResponse;

        IDebuggerClientSession CreateSession(string infobaseAlias);
    }
}
