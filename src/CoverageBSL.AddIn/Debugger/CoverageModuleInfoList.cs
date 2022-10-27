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
    [ContextClass(typeName: "CoverageModuleInfoList", typeAlias: "СписокПокрытияМодулей")]
#if NET5_0_OR_GREATER
    public class CoverageModuleInfoList : AutoCollectionContext<CoverageModuleInfoList, IValue>
#else
    public class CoverageModuleInfoList : AutoContext<CoverageModuleInfoList>, ICollectionContext, IEnumerable<IValue>
#endif
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
#if NET5_0_OR_GREATER
        public override int Count()
#else
        public int Count()
#endif
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

#if NET5_0_OR_GREATER
        public override IEnumerator<IValue> GetEnumerator()
#else
        public IEnumerator<IValue> GetEnumerator()
#endif
            => moduleInfos.GetEnumerator();

        public CollectionEnumerator GetManagedIterator()
            => new CollectionEnumerator(GetEnumerator());

#if NET48
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
#endif
    }
}
