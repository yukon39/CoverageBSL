using com.github.yukon39.CoverageBSL.AddIn.Utils;
using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using OneScript.Contexts;
using OneScript.StandardLibrary.Json;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    [ContextClass(typeName: "CoverageModuleInfoList", typeAlias: "СписокПокрытияМодулей")]
    public class CoverageModuleInfoList : AutoCollectionContext<CoverageModuleInfoList, IValue>
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
        public override int Count()
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

        [ContextMethod("DeserializeJSON", "ДесериализоватьJSON")]
        public void DeserializeJson(JSONReader reader)
        {
            JsonReaderHelper.ReadExpected(reader, JSONValueTypeEnum.ArrayStart);

            while (reader.Read())
            {
                if (reader.CurrentValueType == JSONValueTypeEnum.ArrayEnd)
                {
                    return;
                }

                var moduleInfo = CoverageModuleInfoContextClass.ScriptConstructor();
                moduleInfo.DeserializeJson(reader);
                Add(moduleInfo);
            }
        }

        public override IEnumerator<IValue> GetEnumerator()
            => moduleInfos.GetEnumerator();

        public CollectionEnumerator GetManagedIterator()
            => new CollectionEnumerator(GetEnumerator());
    }
}
