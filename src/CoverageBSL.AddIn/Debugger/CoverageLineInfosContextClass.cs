using com.github.yukon39.DebugBSL.debugger.debugMeasure;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System.Collections;
using System.Collections.Generic;

namespace com.github.yukon39.CoverageBSL.AddIn.Debugger
{
    class CoverageLineInfosContextClass : AutoContext<CoverageLineInfosContextClass>, ICollectionContext, IEnumerable<IValue>
    {
        private readonly List<CoverageLineInfoWrapper> lineInfos = new List<CoverageLineInfoWrapper>();

        public CoverageLineInfosContextClass(List<PerformanceInfoLine> lineInfos) =>
            lineInfos.ForEach(x => this.lineInfos.Add(new CoverageLineInfoWrapper(x)));

        [ContextMethod("Count", "Количество")]
        public int Count()
            => lineInfos.Count;

        [ContextMethod("Add", "Добавить")]
        public void Add(CoverageLineInfoWrapper item)
            => lineInfos.Add(item);

        public IEnumerator<IValue> GetEnumerator()
            => lineInfos.GetEnumerator();

        public CollectionEnumerator GetManagedIterator()
            => new CollectionEnumerator(GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
