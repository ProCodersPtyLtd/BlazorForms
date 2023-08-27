using BlazorForms.Forms.Definitions.FluentForms.Model;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;

namespace BlazorForms.Forms
{
    public interface IModelDefinitionForm 
    {
        
        Type GetDetailsType();
        IEnumerable<DataField> GetDetailsFields();
        IEnumerable<DialogButtonDetails> GetButtons();
        IEnumerable<ConfirmationDetails> GetConfirmations();
        IEnumerable<IBindingFlowReference> GetButtonNavigations();
        IEnumerable<ActionRouteLink> GetContextLinks();

        string DisplayName { get; }
        FormLayout Layout { get; }
        Type ChildProcess { get; }
        FormAllowAccess Access { get; }
        string ItemsPath { get; }
    }
}
