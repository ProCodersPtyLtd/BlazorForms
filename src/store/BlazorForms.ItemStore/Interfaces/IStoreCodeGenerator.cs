using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.ItemStore
{
    public interface IStoreCodeGenerator
    {
        void GenerateStoreModel();
        void GenerateDomainModel();
        void GenerateStoreToDomainMapping();
        void GenerateDomainToStoreMapping();
    }
}
