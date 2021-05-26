using com.github.yukon39.DebugBSL.Client.Impl;
using com.github.yukon39.DebugBSL.Client.Internal;
using System;

namespace com.github.yukon39.DebugBSL.Client
{
    public class DebuggerClientFactory
    {
        public static IDebuggerClient NewInstance(string debuggerURI)
        {
            var rootURI = new Uri(debuggerURI);
            var executor = DebuggerClientExecutor.Create(rootURI);
            return DebuggerClient.NewInstance(executor);
        }
    }
}
