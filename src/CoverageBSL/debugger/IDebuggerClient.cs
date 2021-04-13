namespace com.github.yukon39.CoverageBSL.debugger
{
    public interface IDebuggerClient
    {
        public void Test();

        public string ApiVersion();

        public IDebuggerSession CreateSession(string infobaseAlias);
    }
}
