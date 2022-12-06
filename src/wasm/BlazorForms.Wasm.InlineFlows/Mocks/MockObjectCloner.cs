﻿using Newtonsoft.Json;
using BlazorForms.Flows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Wasm.InlineFlows
{
    public class MockObjectCloner : IObjectCloner
    {
        public async Task<T> CloneObject<T>(T source)
        {
            throw new NotImplementedException();
        }
    }
}
