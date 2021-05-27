using System.Threading.Tasks;

namespace com.github.yukon39.CoverageBSL
{
    public interface ICoverageManager
    {
        string APIVersion();
        Task<string> ApiVersionAsync();

        void TestConnection();
        Task TestConnectionAsync();

        ICoverageSession NewCoverageSession(string infobaseAlias);
    }
}
