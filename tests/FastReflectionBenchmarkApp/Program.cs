using BenchmarkDotNet.Running;
using FastReflectionBenchmarkApp;

BenchmarkRunner.Run<ReflectionBenchmarks>();
//string s = ReflectionUsage.GetValuePropertyDictionary();
//s = ReflectionUsage.GetValueNestedObjectDictionary();
//var s = ReflectionUsage.NestedEmitterFastGet();
Console.ReadLine();