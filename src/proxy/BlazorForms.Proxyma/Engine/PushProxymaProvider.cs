using Castle.DynamicProxy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazorForms.Proxyma
{
    public class PushProxymaProvider : ProxymaProviderBase
    {
        public static readonly string ProxyEngineType = "push";

        public PushProxymaProvider(IProxyGenerator proxyGenerator, HashSet<Type> modelTypes) : base(proxyGenerator, modelTypes)
        {
        }

        public override IProxymaInterceptor CreateModelProxyInterceptor(Action<string, string, object> changingProperty)
        {
            return new PushPropertyBagInterceptor(changingProperty);
        }

        public override T GetModelProxy<T>(T source, Action<string, string, object> changingProperty)
        {
            var interceptor = CreateModelProxyInterceptor(changingProperty) as PushPropertyBagInterceptor;
            T result = ConvertModelHierarchyToProxyTyped(source, interceptor) as T;
            return result;
        }

        public override T GetProxyModel<T>(T source)
        {
            var result = ConvertProxyHierarchyToModel(source) as T;
            return result;
        }

        public override T CreateModelProxyObject<T>(T source, IProxymaInterceptor interceptor)
        {
            var proxy = _proxyGenerator.CreateClassProxyWithTarget(source.GetType(), new Type[] { typeof(IProxyPropertyBagStore) }, source, new IInterceptor[] { interceptor as IInterceptor }) as T;
            var store = proxy as IProxyPropertyBagStore;
            store.PropertyBag.Model = source;
            return proxy;
        }

        #region private
        private object ConvertModelHierarchyToProxyTyped(object source, PushPropertyBagInterceptor interceptor)
        {
            // all nested into source objects (and all nested into nested, etc.) should be substituted by proxies
            var scope = new List<NestedPropertyModel>();
            var sourceProperty = new NestedPropertyModel { Object = source };
            scope.Add(sourceProperty);

            // generate proxies
            for (int i = 0; i < scope.Count; i++)
            {
                var current = scope[i];

                if (current.IsArray)
                {
                    var list = current.Object as Array;
                    int n = 0;

                    foreach (var item in list)
                    {
                        var itemProperty = new NestedPropertyModel { Object = item, Index = n, Parent = current.Parent, Name = current.Name, IsItem = true };
                        itemProperty.Proxy = CreateModelProxyObject(itemProperty.Object, interceptor);
                        scope.Add(itemProperty);
                        n++;
                    }

                    continue;
                }
                else if (current.IsGenericEnumerable)
                {
                    var list = current.Object as IEnumerable<object>;
                    int n = 0;

                    foreach (var item in list)
                    {
                        var itemProperty = new NestedPropertyModel { Object = item, Index = n, Parent = current.Parent, Name = current.Name, IsItem = true };
                        itemProperty.Proxy = CreateModelProxyObject(itemProperty.Object, interceptor);
                        scope.Add(itemProperty);
                        n++;
                    }

                    continue;
                }
                else
                {
                    current.Proxy = CreateModelProxyObject(current.Object, interceptor);
                }

                var nested = GetNestedNotEmptyProperties(current);
                scope.AddRange(nested);
            }

            // set Proxy for list and array items and set tracker properties
            for (int i = scope.Count - 1; i > 0; i--)
            {
                var current = scope[i];
                var parentRecord = scope.FirstOrDefault(r => r.Object == current.Parent);

                // substitute model property to its proxy
                if (current.IsItem)
                {
                    continue;
                }
                else if (current.IsArray)
                {
                    // find all elements in scope in populate array by proxy
                    var list = current.Object as Array;
                    int n = 0;

                    foreach (var item in list)
                    {
                        var scopeItem = scope.Single(s => s.IsItem && s.Index == n && s.Name == current.Name);
                        list.SetValue(scopeItem.Proxy, n);

                        var tracker = scopeItem.Proxy as IProxyPropertyBagStore;
                        tracker.PropertyBag.Parent = parentRecord.Proxy as IProxyPropertyBagStore;
                        tracker.PropertyBag.Name = current.Name;
                        tracker.PropertyBag.Index = n;

                        n++;
                    }
                }
                else if (current.IsGenericEnumerable)
                {
                    var list = current.Object as IList;

                    if (list == null)
                    {
                        throw new NotImplementedException("Only IList collections supported");
                    }

                    for (int n = 0; n < list.Count; n++)
                    {
                        var scopeItem = scope.Single(s => s.IsItem && s.Index == n && s.Name == current.Name);
                        list[n] = scopeItem.Proxy;

                        var tracker = scopeItem.Proxy as IProxyPropertyBagStore;
                        tracker.PropertyBag.Parent = parentRecord.Proxy as IProxyPropertyBagStore;
                        tracker.PropertyBag.Name = current.Name;
                        tracker.PropertyBag.Index = n;
                    }
                }
                else
                {
                    parentRecord.Object.GetType().GetProperty(current.Name).SetValue(parentRecord.Object, current.Proxy);
                    var tracker = current.Proxy as IProxyPropertyBagStore;

                    if (tracker != null)
                    {
                        tracker.PropertyBag.Parent = parentRecord.Proxy as IProxyPropertyBagStore;
                        tracker.PropertyBag.Name = current.Name;
                    }
                }
            }

            // set __JsonPath
            foreach (var current in scope)
            {
                if (current.IsItem)
                {
                }
                else if (current.IsArray)
                {
                    var list = current.Object as Array;

                    foreach (IProxyPropertyBagStore tracker in list)
                    {
                        tracker.PropertyBag.JsonPath = GetJsonPath(tracker, tracker.PropertyBag.Index);
                    }
                }
                else if (current.IsGenericEnumerable)
                {
                    var list = current.Object as IList;

                    foreach (IProxyPropertyBagStore tracker in list)
                    {
                        tracker.PropertyBag.JsonPath = GetJsonPath(tracker, tracker.PropertyBag.Index);
                    }
                }
                else
                {
                    var tracker = current.Proxy as IProxyPropertyBagStore;
                    tracker.PropertyBag.JsonPath = GetJsonPath(tracker);
                }
            }

            return scope[0].Proxy;
        }

        private T ConvertProxyHierarchyToModel<T>(T source) where T : class
        {
            var scope = new List<NestedPropertyProxy>();
            var sourceProperty = new NestedPropertyProxy { Object = source };
            scope.Add(sourceProperty);

            for (int i = 0; i < scope.Count; i++)
            {
                var current = scope[i];

                if (current.IsArray)
                {
                    current.Model = current.Object;
                    var list = current.Object as Array;
                    int n = 0;

                    foreach (var item in list)
                    {
                        var itemProperty = new NestedPropertyProxy { Object = item, Index = n, Parent = current.Parent, Name = current.Name, IsItem = true };
                        dynamic proxyItem = item;
                        itemProperty.Model = proxyItem.PropertyBag.Model;
                        scope.Add(itemProperty);
                        n++;
                    }

                    continue;
                }

                if (current.IsGenericEnumerable)
                {
                    current.Model = current.Object;
                    var list = current.Object as IList;

                    for (int n = 0; n < list.Count; n++)
                    {
                        var item = list[n];
                        var itemProperty = new NestedPropertyProxy { Object = item, Index = n, Parent = current.Parent, Name = current.Name, IsItem = true };
                        dynamic proxyItem = item;
                        itemProperty.Model = proxyItem.PropertyBag.Model;
                        scope.Add(itemProperty);
                    }

                    continue;
                }

                dynamic proxy = current.Object;
                current.Model = proxy.PropertyBag.Model;
                var nested = GetNestedNotEmptyProperties(current);
                scope.AddRange(nested);
            }

            for (int i = scope.Count - 1; i > 0; i--)
            {
                var current = scope[i];

                if (current.IsItem)
                {
                    continue;
                }

                if (current.IsArray)
                {
                    var list = current.Object as Array;

                    for (int n = 0; n < list.Length; n++)
                    {
                        var scopeItem = scope.Single(s => s.IsItem && s.Index == n && s.Name == current.Name);
                        list.SetValue(scopeItem.Model, n);
                    }

                    continue;
                }

                if (current.IsGenericEnumerable)
                {
                    var list = current.Object as IList;

                    for (int n = 0; n < list.Count; n++)
                    {
                        var scopeItem = scope.Single(s => s.IsItem && s.Index == n && s.Name == current.Name);
                        list[n] = scopeItem.Model;
                    }

                    continue;
                }

                var parentRecord = scope.FirstOrDefault(r => r.Object == current.Parent);
                parentRecord.Model.GetType().GetProperty(current.Name).SetValue(parentRecord.Model, current.Model);
            }

            return scope[0].Model as T;
        }

        #endregion

        #region support classes

        public class NestedProperty
        {
            public string Name { get; set; }
            public object Object { get; set; }
            public object Parent { get; set; }
            public bool IsArray { get; set; }
            public bool IsGenericEnumerable { get; set; }
            public int Index { get; internal set; }
            public bool IsItem { get; set; }
        }

        public class NestedPropertyModel : NestedProperty
        {
            public object Proxy { get; set; }
        }

        public class NestedPropertyProxy : NestedProperty
        {
            public object Model { get; set; }
        }

        public string GetJsonPath(IProxyPropertyBagStore obj, int? index = null)
        {
            if (obj.PropertyBag.Name == null)
            {
                return "$";
            }

            IProxyPropertyBagStore current = obj;
            var path = "";

            do
            {
                string ind = current.PropertyBag.Index != null ? $"[{current.PropertyBag.Index}]" : "";
                path = $".{current.PropertyBag.Name}{ind}{path}";
                current = current.PropertyBag.Parent as IProxyPropertyBagStore;
            } while (current?.PropertyBag?.Parent != null);

            path = $"${path}";
            return path;
        }

        public IEnumerable<T> GetNestedNotEmptyProperties<T>(T source) where T : NestedProperty, new()
        {
            var nestedVirtualProperties = source.Object.GetType().GetProperties().Where(p => p.GetMethod?.IsVirtual == true && p.SetMethod?.IsVirtual == true);
            var nestedProxyMappedArrays = nestedVirtualProperties.Where(p => p.PropertyType.IsArray && IsProxyScopeType(p.PropertyType.GetElementType()));

            var nestedProxyMappedGenericEnumerables = nestedVirtualProperties.Where(p => p.PropertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(p.PropertyType)
                && p.PropertyType.GetGenericArguments().Any(a => IsProxyScopeType(a)));

            var nestedProxyMappedProperties = nestedVirtualProperties.Where(p => IsProxyScopeType(p.PropertyType));

            var notNullNested = nestedProxyMappedProperties.Select(p => new T { Name = p.Name, Object = p.GetValue(source.Object), Parent = source.Object }).Where(p => p.Object != null)
                .Union(nestedProxyMappedArrays.Select(p => new T { Name = p.Name, Object = p.GetValue(source.Object), Parent = source.Object, IsArray = true })
                    .Where(p => p.Object != null && (p.Object as Array).Length > 0))
                .Union(nestedProxyMappedGenericEnumerables.Select(p => new T { Name = p.Name, Object = p.GetValue(source.Object), Parent = source.Object, IsGenericEnumerable = true })
                    .Where(p => p.Object != null && (p.Object as IEnumerable<object>).Any()));

            return notNullNested.ToList().AsEnumerable();
        }







        #endregion

    }
}
