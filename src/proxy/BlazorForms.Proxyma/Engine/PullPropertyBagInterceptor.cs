using Castle.DynamicProxy;
using BlazorForms.Proxyma.Model;
using BlazorForms.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace BlazorForms.Proxyma
{
    public class PullPropertyBagInterceptor : IProxymaInterceptor, IInterceptor
    {
        protected readonly IProxyGenerator _proxyGenerator;
        private readonly Dictionary<int, ProxyPropertyBag> _propertyBags = new();
        private readonly HashSet<object> _proxiedObjects = new();
        private readonly Action<string, string, object> _changingProperty;
        private readonly List<string> _log = new();
        private readonly List<ProxyOperation> _proxyOperations = new();
        protected readonly HashSet<Type> _typeScope;
        private bool _reverted = false;
        internal object _sourceModel;

        public PullPropertyBagInterceptor(IProxyGenerator proxyGenerator, HashSet<Type> typeScope, Action<string, string, object> changingProperty)
        {
            _proxyGenerator = proxyGenerator;
            _typeScope = typeScope;
            RegisterSystemTypes(_typeScope);
            _changingProperty = changingProperty;
        }

        private void RegisterSystemTypes(HashSet<Type> typeScope)
        {
            typeScope.Set(typeof(DynamicRecordset));
        }

        #region classes
        public enum ProxyOperationType { Property, Array, List, Expando, Dictionary }

        public class ProxyOperation
        {
            public ProxyOperationType OperationType { get; set; }
            public object TargetObject { get; set; }
            public object ProxyObject { get; set; }
            public List<object> TargetList { get; set; } = new();
            public object InvocationTarget { get; set; }
            public string MethodName { get; set; }
        }
        #endregion

        public string GetJsonPath(ProxyPropertyBag obj)
        {
            if (obj.Name == null)
            {
                return "$";
            }

            ProxyPropertyBag current = obj;
            var path = "";

            do
            {
                string ind = current.Index != null ? $"[{current.Index}]" : "";
                path = $".{current.Name}{ind}{path}";

                var hash = current.Parent.GetHashCode();
                current = _propertyBags[hash];
            } while (current?.Parent != null);

            path = $"${path}";
            return path;
        }

        public void Intercept(IInvocation invocation)
        {
            // on demand
            if (_reverted)
            {
                return;
            }

            if (invocation.MethodInvocationTarget.Name == "GetHashCode")
            {
                invocation.Proceed();
                return;
            }

            var hash = invocation.Proxy.GetHashCode();

            if (!_propertyBags.ContainsKey(hash))
            {
                _propertyBags[hash] = new ProxyPropertyBag();
            }

            var methodName = invocation.Method.Name;


            if (methodName.StartsWith("get_"))
            {
                var nestedObject = invocation.Method.Invoke(invocation.InvocationTarget, invocation.Arguments);

                if (nestedObject == null)
                {
                    // ToDo: should we check nestedObject type here?
                    // SetProxyProperty(invocation, methodName, null);
                }
                else if (nestedObject.GetType() == typeof(ExpandoObject))
                {
                    SetupExpando(invocation, methodName, nestedObject as ExpandoObject);
                }
                else if (_typeScope.Contains(nestedObject.GetType()))
                {
                    SetProxyProperty(invocation, methodName, nestedObject);
                }
                else if (nestedObject.GetType().IsGenericType && nestedObject.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    SetProxyDictionary(invocation, nestedObject);
                }
                else if (nestedObject.GetType().IsArray && _typeScope.Any(t => t.Name == nestedObject.GetType().Name.Replace("[]", "")))
                {
                    SetProxyArray(invocation, nestedObject);
                }
                else if (nestedObject.GetType().IsGenericType && typeof(IEnumerable).IsAssignableFrom(nestedObject.GetType())
                            && nestedObject.GetType().GetGenericArguments().Any(a => _typeScope.Contains(a)))
                {
                    SetProxyList(invocation, nestedObject);
                }
            }

            if (methodName.StartsWith("set_"))
            {
                _log.Add($"{methodName} {invocation.Arguments[0]}");

                var prop = invocation.Method.Name.Replace("set_", "");
                var obj = _propertyBags[hash];
                var jsonPath = GetJsonPath(obj);
                jsonPath = $"{jsonPath}.{prop}";
                _changingProperty?.Invoke(jsonPath, prop, invocation.Arguments[0]);
            }

            invocation.Proceed();
        }

        #region reverts
        public object RevertBack()
        {
            _reverted = true;

            foreach (var operation in _proxyOperations)
            {
                switch (operation.OperationType)
                {
                    case ProxyOperationType.Property:
                        RevertProperty(operation);
                        break;

                    case ProxyOperationType.List:
                        RevertList(operation);
                        break;

                    case ProxyOperationType.Array:
                        RevertArray(operation);
                        break;

                    case ProxyOperationType.Dictionary:
                        RevertDictionary(operation);
                        break;

                    case ProxyOperationType.Expando:
                        RevertExpando(operation);
                        break;
                }
            }

            return _sourceModel;
        }

        private static void RevertExpando(ProxyOperation operation)
        {
            (operation.ProxyObject as ExpandoObject).CopyTo(operation.TargetObject as ExpandoObject);
            var method = operation.InvocationTarget.GetType().GetMethod(operation.MethodName);
            method.Invoke(operation.InvocationTarget, new object[] { operation.TargetObject });
        }

        private static void RevertDictionary(ProxyOperation operation)
        {
            var list = operation.TargetObject as IDictionary;
            var keys = (list.Keys as IEnumerable<string>).ToList();
            int n = 0;

            foreach (var key in keys)
            {
                if (list[key] is ExpandoObject)
                {
                    (list[key] as ExpandoObject).CopyTo(operation.TargetList[n] as ExpandoObject);
                }

                list[key] = operation.TargetList[n];
                n++;
            }
        }
        private static void RevertArray(ProxyOperation operation)
        {
            var list = operation.TargetObject as Array;
            int n = 0;

            foreach (var item in list)
            {
                if (operation.TargetList[n] != null)
                {
                    list.SetValue(operation.TargetList[n], n);
                }

                n++;
            }
        }

        private static void RevertList(ProxyOperation operation)
        {
            var list = operation.TargetObject as IList;

            for (int n = 0; n < list.Count; n++)
            {
                if (operation.TargetList[n] != null)
                {
                    list[n] = operation.TargetList[n];
                }
            }
        }

        private static void RevertProperty(ProxyOperation operation)
        {
            var method = operation.InvocationTarget.GetType().GetMethod(operation.MethodName);
            method.Invoke(operation.InvocationTarget, new object[] { operation.TargetObject });
        }

        #endregion

        private void SetupExpando(IInvocation invocation, string methodName, ExpandoObject nestedObject)
        {
            var nestedHash = nestedObject.GetHashCode();
            // if we already created proxy (subscribed) then ignore
            if (_proxiedObjects.Contains(nestedObject) || _propertyBags.ContainsKey(nestedHash))
            {
                return;
            }

            _proxiedObjects.Add(nestedObject);

            var proxyObject = nestedObject.Clone();
            ((INotifyPropertyChanged)proxyObject).PropertyChanged += PullPropertyBagInterceptor_PropertyChanged;
            AddPropertyBag(proxyObject, nestedObject, methodName.Replace("get_", ""), invocation.Proxy);

            var setter = methodName.Replace("get_", "set_");
            var method = invocation.InvocationTarget.GetType().GetMethod(setter);
            method.Invoke(invocation.InvocationTarget, new object[] { proxyObject });

            _proxyOperations.Add(new ProxyOperation
            {
                OperationType = ProxyOperationType.Expando,
                TargetObject = nestedObject,
                ProxyObject = proxyObject,
                InvocationTarget = invocation.InvocationTarget,
                MethodName = method.Name,
            });
        }

        private void SetProxyProperty(IInvocation invocation, string methodName, object nestedObject)
        {
            var proxyObject = _proxyGenerator.CreateClassProxyWithTarget(nestedObject.GetType(), nestedObject, this);
            AddPropertyBag(proxyObject, nestedObject, methodName.Replace("get_", ""), invocation.Proxy);

            var setter = methodName.Replace("get_", "set_");
            var method = invocation.InvocationTarget.GetType().GetMethod(setter);
            method.Invoke(invocation.InvocationTarget, new object[] { proxyObject });

            _proxyOperations.Add(new ProxyOperation
            {
                OperationType = ProxyOperationType.Property,
                TargetObject = nestedObject,
                InvocationTarget = invocation.InvocationTarget,
                MethodName = method.Name,
            });
        }

        private void PullPropertyBagInterceptor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var senderHash = sender.GetHashCode();
            var obj = _propertyBags[senderHash];
            var jsonPath = GetJsonPath(obj);
            jsonPath = $"{jsonPath}.{e.PropertyName}";
            var value = (sender as ExpandoObject).GetValue(e.PropertyName);
            _changingProperty?.Invoke(jsonPath, e.PropertyName, value);
        }

        private void AddPropertyBag(object proxyObject, object model, string name, object parent, int? index = null, object key = null)
        {
            var proxyHash = proxyObject.GetHashCode();

            _propertyBags.TryAdd(proxyHash, new ProxyPropertyBag());
            if (_propertyBags.TryGetValue(proxyHash, out var bag))
            {
                bag.Model = model;
                bag.Name = name;
                bag.Parent = parent;
                bag.Index = index;
                bag.Key = key;
            }
        }
        private void SetProxyDictionary(IInvocation invocation, object nestedObject)
        {
            // check that proxy already created for the InvocationTarget
            if (_proxiedObjects.Contains(nestedObject))
            {
                return;
            }

            _proxiedObjects.Add(nestedObject);

            var list = nestedObject as IDictionary;

            var operation = new ProxyOperation
            {
                OperationType = ProxyOperationType.Dictionary,
                TargetObject = nestedObject,
            };

            var keys = (list.Keys as IEnumerable<string>).ToList();

            foreach(var key in keys)
            {
                var item = list[key];

                if (item.GetType() == typeof(ExpandoObject))
                {
                    operation.TargetList.Add(item);
                    var proxyObject = (item as ExpandoObject).Clone();
                    ((INotifyPropertyChanged)proxyObject).PropertyChanged += PullPropertyBagInterceptor_PropertyChanged;
                    list[key] = proxyObject;
                    AddPropertyBag(proxyObject, item, $"{invocation.Method.Name.Replace("get_", "")}['{key}']", invocation.Proxy);
                }
                else if (_typeScope.Contains(item?.GetType()))
                {
                    operation.TargetList.Add(item);
                    var proxyObject = _proxyGenerator.CreateClassProxyWithTarget(item.GetType(), item, this);
                    list[key] = proxyObject;
                    AddPropertyBag(proxyObject, item, $"{invocation.Method.Name.Replace("get_", "")}['{key}']", invocation.Proxy, key: key);
                }
                else
                {
                    operation.TargetList.Add(null);
                }
            }

            _proxyOperations.Add(operation);
        }

        private void SetProxyList(IInvocation invocation, object nestedObject)
        {
            var list = nestedObject as IList;

            var operation = new ProxyOperation
            {
                OperationType = ProxyOperationType.List,
                TargetObject = nestedObject,
            };

            for (int n = 0; n < list.Count; n++)
            {
                var item = list[n];

                if (item.GetType() == typeof(ExpandoObject))
                {
                    operation.TargetList.Add(item);
                    var proxyObject = (item as ExpandoObject).Clone();
                    ((INotifyPropertyChanged)proxyObject).PropertyChanged += PullPropertyBagInterceptor_PropertyChanged;
                    list[n] = proxyObject;
                    AddPropertyBag(proxyObject, item, $"{invocation.Method.Name.Replace("get_", "")}[{n}]", invocation.Proxy);
                }
                else if (_typeScope.Contains(item?.GetType()))
                {
                    operation.TargetList.Add(item);
                    var proxyObject = _proxyGenerator.CreateClassProxyWithTarget(item.GetType(), item, this);
                    list[n] = proxyObject;
                    AddPropertyBag(proxyObject, item, invocation.Method.Name.Replace("get_", ""), invocation.Proxy, n);
                }
                else
                {
                    operation.TargetList.Add(null);
                }
            }

            _proxyOperations.Add(operation);
        }

        private void SetProxyArray(IInvocation invocation, object nestedObject)
        {
            var list = nestedObject as Array;
            int n = 0;

            var operation = new ProxyOperation
            {
                OperationType = ProxyOperationType.Array,
                TargetObject = nestedObject,
            };

            foreach (var item in list)
            {

                if (_typeScope.Contains(item.GetType()))
                {
                    operation.TargetList.Add(item);
                    var proxyObject = _proxyGenerator.CreateClassProxyWithTarget(item.GetType(), item, this);
                    list.SetValue(proxyObject, n);
                    AddPropertyBag(proxyObject, item, invocation.Method.Name.Replace("get_", ""), invocation.Proxy, n);
                }
                else
                {
                    operation.TargetList.Add(null);
                }

                n++;
            }

            _proxyOperations.Add(operation);
        }
    }
}
