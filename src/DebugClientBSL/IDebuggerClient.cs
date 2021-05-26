using System.Threading.Tasks;

namespace com.github.yukon39.DebugBSL.Client
{
    public interface IDebuggerClient
    {
        Task TestAsync();

        Task<string> ApiVersionAsync();

        IDebuggerClientSession CreateSession(string infobaseAlias);
    }
}
