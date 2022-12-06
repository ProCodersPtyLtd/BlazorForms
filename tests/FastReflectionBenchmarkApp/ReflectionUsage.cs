using BenchmarkDotNet.Columns;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReflectionBenchmarkApp
{
    internal class ReflectionUsage
    {
        private static readonly Model _model;

        static JsonPathNavigator _nav = new JsonPathNavigator();
        static JsonPathNavigatorFastEmitted _fast = new JsonPathNavigatorFastEmitted();
        static FastReflectionPrototype _fastReflection = new FastReflectionPrototype();
        static Func<object, object> _netsedEmitter;

        static ReflectionUsage()
        {
            _model = new Model();
            _model.Name = "Abba";
            _model.Nested.Value = "Java";
            _netsedEmitter = FastReflectionPrototype.GetNestedEmitter(_model, "$.Nested.Value").CreateDelegate();
        }

        public static string SimpleGet()
        {
            //return _model.Name;
            return _model.Nested.Value;
        }

        static Func<object, object> _getter = FastReflectionPrototype.GetJsonPathStraightEmitterGet(typeof(Model), "$.Nested.Value").CreateDelegate();
        public static string GetJsonPathStraightEmitterGet()
        {
            //return _model.Name;
            return _getter(_model).AsString();
        }

        static Action<object, object> _setter = FastReflectionPrototype.GetJsonPathStraightEmitterSet(typeof(Model), "$.Nested.Value").CreateDelegate();
        public static void GetJsonPathStraightEmitterSet()
        {
            //return _model.Name;
            _setter(_model, "Zebr");
        }

        public static string NestedEmitterFastGet()
        {
            return _netsedEmitter(_model).AsString();
        }

        public static string ReflectionGet()
        {
            return _nav.GetValue(_model, "$.Name").AsString();
        }


        public static string JsonPathNavigatorFastGet()
        {
            return _fast.GetValue(_model, "$.Name").AsString();
        }

        public static string FastReflectionDirect()
        {
            return _fastReflection.GetValueMaxSpeed(_model, "Name").AsString();
        }

        public static string FastReflectionStringKey()
        {
            return _fastReflection.GetValueStringKey(_model, "Name").AsString();
        }

        public static string FastReflectionTupleKey()
        {
            return _fastReflection.GetValueTupleKey(_model, "Name").AsString();
        }

        public static string FastReflectionNestedDictionary()
        {
            return _fastReflection.GetValueNestedDictionary(_model, "Name").AsString();
        }

        static Type _type = typeof(Model);
        static Dictionary<string, Func<object, object>> _pps = new Dictionary<string, Func<object, object>>();
        static int _code = "Name".GetHashCode();

        public static string FastReflectionGetValueTypeDictionary()
        {
            return _fastReflection.GetValueTypeDictionary(_model, "Name", _pps).AsString();
        }

        public static string FastReflectionGetValuePropertyDictionary()
        {
            return _fastReflection.GetValuePropertyDictionary(_model, "Name").AsString();
        }

        public static void FastReflectionSetValueNestedDictionary()
        {
            _fastReflection.SetValueNestedDictionary(_model, "Name", "qq23");
        }

        public static string GetValueNestedObjectDictionary()
        {
            return _fastReflection.GetValueNestedObjectDictionary(_model, "Name").AsString();
        }

        class Model
        {
            public string Name { get; set; }
            public NestedModel Nested { get; set; } = new NestedModel();
        }

        class NestedModel
        {
            public string Value { get; set; }
        }
    }
}
