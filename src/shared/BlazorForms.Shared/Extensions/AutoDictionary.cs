using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public class AutoDictionary<TKey, TValue> where TValue : class, new()
    {
        private Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get
            {
                if (!_dict.ContainsKey(key))
                {
                    var item = Activator.CreateInstance<TValue>();
                    _dict.Add(key, item);
                }

                return _dict[key];
            }
        }

        public Dictionary<TKey, TValue> Dictionary
        {
            get => _dict;

            set
            {
                _dict = value;
            }
        }
    }
}
