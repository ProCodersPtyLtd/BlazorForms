using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BlazorForms.Forms
{
    public interface IModelDefinitionForm 
    {
        
        Type GetDetailsType();
        IEnumerable<DataField> GetDetailsFields();
        IEnumerable<DialogButtonDetails> GetButtons();
        IEnumerable<IBindingFlowReference> GetButtonNavigations();
        IEnumerable<ActionRouteLink> GetContextLinks();

        string DisplayName { get; }
        Type ChildProcess { get; }
        FormAllowAccess Access { get; }
        string ItemsPath { get; }
    }
}
