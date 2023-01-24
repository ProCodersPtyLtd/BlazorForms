using BlazorForms.FlowRules;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Rendering.Model;
using BlazorForms.Rendering.Types;
using BlazorForms.Rendering.ViewModels;

namespace BlazorForms.Rendering.Interfaces
{
    public interface IFormViewModel<T> : IFormViewModel where T : class, IFlowModel
    {
        T Model { get; }
    }

    public interface IFormViewModel : IRenderingViewModel
    {
        IFlowModel? ModelUntyped { get; }
        IFlowParams? Params { get; }
        IFlowContextNoModel? Context { get; }
        string? RefId { get; }
        FormDetails? FormData { get; }
        FormSettingsViewState FormSettings { get; }
        FlowParamsGeneric? FormParameters { get; }
        IEnumerable<IGrouping<string, FieldControlDetails>>? FieldsGrouped { get; }
        Dictionary<string, List<FieldControlDetails>>? Tables { get; }
        Dictionary<string, List<FieldControlDetails>>? Repeaters { get; }
        Dictionary<string, List<FieldControlDetails>>? Lists { get; }
        IEnumerable<RuleExecutionResult>? Validations { get; set; }
        LayoutFormParams? LayoutParams { get; }
        IJsonPathNavigator? PathNavi { get; }
        bool FormAccessDenied { get; }
        string? FormAssignedUser { get; }
        IEnumerable<FieldControlDetails>? ActionFields { get; }
        FieldControlDetails? SubmitAction { get; }
        string? SubmitActionName { get; }
        FieldControlDetails? RejectAction { get; }
        string? RejectActionName { get; }
        string? SaveActionName { get; }
        string? ExceptionMessage { get; }
        string? ExceptionStackTrace { get; }
        string? ExceptionType { get; }

        // main flow api
        Task InitiateFlow(string flowName, string refId, string pk);
        Task FinishFlow(string refId, string binding = null);
        Task ReloadFormData();
        List<RuleExecutionResult> CheckUniqueValidationRules(string tableBinding);
        Task<RuleEngineExecutionResult> TriggerRules(string formName, FieldBinding modelBinding, FormRuleTriggers? trigger = null, int rowIndex = 0);
        //Task<RuleEngineExecutionResult> TriggerFormLoadRulesRules();
        Task SaveForm(string actionBinding = null, string operationName = null);
        Task SubmitForm(string binding = null, string operationName = null);
        Task RejectForm(string binding = null, string operationName = null);
        Task LoadFlowDefaultForm(string refId);
        Task ApplyFormData(FormDetails form, IFlowModel model);
        List<FormConfirmationData> GetAvailableConfirmations(ConfirmType confirmType, string? binding = null);
        void RefreshValidations(FieldControlDetails field);
        IEnumerable<RuleExecutionResult> GetValidations(FieldControlDetails field);

        // track user input changes
        void RegisterChildControlViewModel(ControlViewModel child);
        void UnregisterChildControlViewModel(ControlViewModel child);
        void SetInputChanged(bool changed = true);
        void IgnoreInputChanged();
        void RestoreInputChanged();
        bool InputChangedIgnored { get; }

        // useful api
        List<SelectableListItem> GetSelectableListData(FieldControlDetails field);
        FieldControlDetails GetRowField(FieldControlDetails template, int row);
        FieldControlDetails GetFieldByName(string name);
        IEnumerable<RuleExecutionResult> GetDynamicFieldValidations();
        Task<bool> CheckFormUserAccess(FormDetails form, UserViewAccessInformation accessInfo, IFlowModel model, FlowParamsGeneric flowParams);
        void ClearRowFields();

        // ModelNavi
        [Obsolete]
        object ModelNaviGetValueObject(string modelBinding);
        [Obsolete]
        string ModelNaviGetValue(string modelBinding);
        [Obsolete]
        object ModelNaviGetValue(string tableBinding, int rowIndex, string modelBinding);
        [Obsolete]
        void ModelNaviSetValue(string modelBinding, object val);
        [Obsolete]
        void ModelNaviSetValue(string tableBinding, int rowIndex, string modelBinding, object val);
        [Obsolete]
        IEnumerable<object> ModelNaviGetItems(string itemsBinding);

        // FastReflection
        object FieldGetValue(object model, FieldBinding binding);
        object FieldGetNameValue(object model, FieldBinding binding);
        object FieldGetIdValue(object model, FieldBinding binding);
        IEnumerable<object> FieldGetItemsValue(object model, FieldBinding binding);
        IEnumerable<object> FieldGetItemsValue(object model, string modelBinding);
        IEnumerable<object> FieldGetTableValue(object model, FieldBinding binding);
        object FieldGetRowValue(object model, FieldBinding binding, int rowIndex);
        void FieldSetValue(object model, FieldBinding binding, object value);
    }
}
