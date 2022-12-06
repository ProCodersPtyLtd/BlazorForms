using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Proxyma.Model
{
    public class ProxyPropertyBag
    {
        public string JsonPath { get; set; }
        public string Name { get; set; }
        public object Parent { get; set; }
        public object Model { get; set; }
        public int? Index { get; set; }
        public object Key { get; set; }
    }
}
