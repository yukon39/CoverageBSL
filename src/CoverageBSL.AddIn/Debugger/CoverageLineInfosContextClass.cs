using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageLineInfoList", typeAlias: "СписокСтрокПокрытия")]
    public class CoverageLineInfosContextClass : AutoCollectionContext<CoverageLineInfosContextClass, IValue>
    {
        private readonly List<CoverageLineInfoWrapper> lineInfos = new List<CoverageLineInfoWrapper>();

        public CoverageLineInfosContextClass(List<PerformanceInfoLine> lineInfos)
            => lineInfos.ForEach(x => this.lineInfos.Add(new CoverageLineInfoWrapper(x)));

        public CoverageLineInfosContextClass() { }

        [ContextMethod("Count", "Количество")]
        public override int Count()
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

        public override IEnumerator<IValue> GetEnumerator()
            => lineInfos.GetEnumerator();

        public CollectionEnumerator GetManagedIterator()
            => new CollectionEnumerator(GetEnumerator());
    }
}
