using BlazorForms.ItemStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public interface IStoreObjectResolver
    {
        string ResolveField(StoreFieldReference field);
    }
}
