using com.github.yukon39.DebugBSL.Client.Data;
using com.github.yukon39.DebugBSL.Client.Internal;
using System;

namespace com.github.yukon39.DebugBSL.Client.Impl
{
    public abstract class DebuggerClientEntityManager
    {
        protected readonly DebuggerClientExecutor Executor;
        protected readonly SessionContext Context;

        public DebuggerClientEntityManager(DebuggerClientExecutor executor, SessionContext context)
        {
            Executor = executor;
            Context = context;
        }
        public virtual void SubscribeSessionEvents(IDebuggerClientSession session) =>
            throw new NotImplementedException();
    }
}
