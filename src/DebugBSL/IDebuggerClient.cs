namespace com.github.yukon39.DebugBSL
{
    public interface IDebuggerClient
    {
        public void Test();

        public string ApiVersion();

        public IDebuggerClientSession CreateSession(string infobaseAlias);
    }
}
