using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System.Collections.Generic;

#if NET48
using ScriptEngine.HostedScript.Library.Json;
using System.Collections;
#else
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
#endif

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageLineInfoList", typeAlias: "СписокСтрокПокрытия")]
#if NET48
    public class CoverageLineInfosContextClass : AutoContext<CoverageLineInfosContextClass>, ICollectionContext, IEnumerable<IValue>
#else
    public class CoverageLineInfosContextClass : AutoCollectionContext<CoverageLineInfosContextClass, IValue>
#endif
    {
        private readonly List<CoverageLineInfoWrapper> lineInfos = new List<CoverageLineInfoWrapper>();

        public CoverageLineInfosContextClass(List<PerformanceInfoLine> lineInfos)
            => lineInfos.ForEach(x => this.lineInfos.Add(new CoverageLineInfoWrapper(x)));

        public CoverageLineInfosContextClass() { }

        [ContextMethod("Count", "Количество")]
#if NET48
        public int Count()
#else
        public override int Count()
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

#if NET48
        public IEnumerator<IValue> GetEnumerator()
#else
        public override IEnumerator<IValue> GetEnumerator()
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
