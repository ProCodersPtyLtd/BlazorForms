using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Interfaces
{
    public interface IListFormViewModel : IRenderingViewModel
    {
        string[][]? ListData { get; }
        List<FieldControlDetails>? Columns { get; }
        List<FieldControlDetails>? VisibleColumns { get; }
        FormDetails? FormData { get; }
        QueryOptions? QueryOptions { get; }
        IFlowModel? Model { get; }
        string? ExceptionMessage { get; }
        string? ExceptionStackTrace { get; }
        string? ExceptionType { get; }
        Exception? ExecutionException { get; }

        List<string>? ReferenceButtonActions { get; }
        Dictionary<string, BindingFlowAction>? ReferenceButtonActionsDictionary { get; }

        List<string>? MainMenuActions { get; }
        Dictionary<string, BindingFlowAction>? MainMenuActionsDictionary { get; }

        List<string>? ContextMenuActions { get; }
        Dictionary<string, BindingFlowAction>? ContextMenuActionsDictionary { get; }
        Dictionary<string, FilterObject>? FieldFilters { get; }

        Task NavigateActionFlow(string action);
        Task NavigateReferenceButtonActionFlow(string action);
        Task LoadListForm(string flowType, string pK, FlowParamsGeneric flowParams, bool clear = false);
        Task ContextMenuClicking(string pk);

        // filters
        void SetFilterValue(FieldControlDetails field, string filtervalue, FieldFilterPositionType? fieldFilterPositionType = null);
        DateTime? GetDateFilter(FieldControlDetails field, FieldFilterPositionType? fieldFilterPositionType = null);
        decimal? GetDecimalFilter(FieldControlDetails field, FieldFilterPositionType? position = null);
        string GetFilter(FieldControlDetails field);
    }
}
