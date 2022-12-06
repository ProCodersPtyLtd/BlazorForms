using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReflectionBenchmarkApp
{
    [MemoryDiagnoser]
    public class ReflectionBenchmarks
    {
        // 3.5 ns
        [Benchmark]
        public string Direct() => ReflectionUsage.SimpleGet();

        // 9.1 ns
        [Benchmark]
        public string GetJsonPathStraightEmitterGet() => ReflectionUsage.GetJsonPathStraightEmitterGet();

        // 4.7 ns
        [Benchmark]
        public void GetJsonPathStraightEmitterSet() => ReflectionUsage.GetJsonPathStraightEmitterSet();

        // 222 ns
        //[Benchmark]
        //public string JsonPathNavigator() => ReflectionUsage.ReflectionGet();

        //[Benchmark]
        //public string JsonPathNavigatorFast() => ReflectionUsage.JsonPathNavigatorFastGet();

        //[Benchmark]
        //public string FastReflectionDirect() => ReflectionUsage.FastReflectionDirect();

        // 58 ns
        //[Benchmark]
        //public string FastReflectionStringKey() => ReflectionUsage.FastReflectionStringKey();

        //[Benchmark]
        //public string FastReflectionTupleKey() => ReflectionUsage.FastReflectionTupleKey();

        // 34 ns
        //[Benchmark]
        //public string FastReflectionNestedDictionary() => ReflectionUsage.FastReflectionNestedDictionary();

        // 18 ns
        //[Benchmark]
        //public string FastReflectionGetValueTypeDictionary() => ReflectionUsage.FastReflectionGetValueTypeDictionary();

        // 9 ns
        //[Benchmark]
        //public string FastReflectionNestedEmitterFastGet() => ReflectionUsage.NestedEmitterFastGet();

        // 42 ns
        //[Benchmark]
        //public string FastReflectionGetValuePropertyDictionary() => ReflectionUsage.FastReflectionGetValuePropertyDictionary();

        // 29 ns
        //[Benchmark]
        //public void FastReflectionSetValueNestedDictionary() => ReflectionUsage.FastReflectionSetValueNestedDictionary();

        //[Benchmark]
        //public string GetValueNestedObjectDictionary() => ReflectionUsage.GetValueNestedObjectDictionary();
    }
}
