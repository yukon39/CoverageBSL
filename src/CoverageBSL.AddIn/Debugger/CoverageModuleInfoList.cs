using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using ScriptEngine.HostedScript.Library.Json;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System.Collections;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageModuleInfoList", typeAlias: "СписокПокрытияМодулей")]
    public class CoverageModuleInfoList : AutoContext<CoverageModuleInfoList>, ICollectionContext, IEnumerable<IValue>
    {
        private readonly List<CoverageModuleInfoContextClass> moduleInfos = new List<CoverageModuleInfoContextClass>();

        public CoverageModuleInfoList(List<PerformanceInfoMain> performanceInfos)
            => performanceInfos.ForEach(x => ProcessPerformanceInfo(x));

        public CoverageModuleInfoList() { }

        private void ProcessPerformanceInfo(PerformanceInfoMain info)
            => info.ModuleData.ForEach(x => ProcessPerformanceInfoModule(x));

        private void ProcessPerformanceInfoModule(PerformanceInfoModule module)
            => moduleInfos.Add(new CoverageModuleInfoContextClass(module));

        [ContextMethod("Count", "Количество")]
        public int Count()
            => moduleInfos.Count;

        [ContextMethod("Add", "Добавить")]
        public void Add(CoverageModuleInfoContextClass item)
            => moduleInfos.Add(item);

        [ContextMethod("SerializeJSON", "СериализоватьJSON")]
        public void SerializeJson(JSONWriter writer)
        {
            writer.WriteStartArray();

            moduleInfos.ForEach(x => x.SerializeJson(writer));

            writer.WriteEndArray();
        }

        public IEnumerator<IValue> GetEnumerator()
            => moduleInfos.GetEnumerator();

        public CollectionEnumerator GetManagedIterator()
            => new CollectionEnumerator(GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
