using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System.Collections.Generic;

#if NET5_0_OR_GREATER
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
#else
using ScriptEngine.HostedScript.Library.Json;
using System.Collections;
#endif

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageLineInfoList", typeAlias: "СписокСтрокПокрытия")]
#if NET5_0_OR_GREATER
    public class CoverageLineInfosContextClass : AutoCollectionContext<CoverageLineInfosContextClass, IValue>
#else
    public class CoverageLineInfosContextClass : AutoContext<CoverageLineInfosContextClass>, ICollectionContext, IEnumerable<IValue>
#endif
    {
        private readonly List<CoverageLineInfoWrapper> lineInfos = new List<CoverageLineInfoWrapper>();

        public CoverageLineInfosContextClass(List<PerformanceInfoLine> lineInfos)
            => lineInfos.ForEach(x => this.lineInfos.Add(new CoverageLineInfoWrapper(x)));

        public CoverageLineInfosContextClass() { }

        [ContextMethod("Count", "Количество")]
#if NET5_0_OR_GREATER
        public override int Count()
#else
        public int Count()
#endif
            => lineInfos.Count;

        [ContextMethod("Add", "Добавить")]
        public void Add(CoverageLineInfoWrapper item)
            => lineInfos.Add(item);

        [ContextMethod("SerializeJSON", "СериализоватьJSON")]
        public void SerializeJson(JSONWriter writer)
        {
            writer.WriteStartArray();

            lineInfos.ForEach(x => x.SerializeJson(writer));

            writer.WriteEndArray();
        }

#if NET5_0_OR_GREATER
        public override IEnumerator<IValue> GetEnumerator()
#else
        public IEnumerator<IValue> GetEnumerator()
#endif
            => lineInfos.GetEnumerator();

        public CollectionEnumerator GetManagedIterator()
            => new CollectionEnumerator(GetEnumerator());

#if NET48
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
#endif
    }
}
