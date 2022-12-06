using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using BlazorForms.FlowRules;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform;
using BlazorForms.Platform.Definitions.Shared;
using BlazorForms.Rendering.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering
{
    public class ListFormViewModel : RenderingViewModel, IListFormViewModel
    {
        private readonly ILogger _logger;
        private readonly IAuthState _authState;
        private readonly NavigationManager _navigationManager;

        public string[][]? ListData { get; private set; }
        public List<FieldControlDetails>? Columns { get; private set; }
        public List<FieldControlDetails>? VisibleColumns { get; private set; }
        public FormDetails? FormData { get; private set; }
        public QueryOptions? QueryOptions { get; private set; }
        public IFlowModel? Model { get; private set; }
        public FlowParamsGeneric? FlowParams { get; private set; }
        public Exception? ExecutionException { get; private set; }

        public IEnumerable<RuleExecutionResult>? Validations { get; set; }
        public List<string>? ReferenceButtonActions { get { return ReferenceButtonActionsDictionary?.Keys?.ToList(); } }
        public Dictionary<string, BindingFlowAction>? ReferenceButtonActionsDictionary { get; private set; }

        public List<string>? MainMenuActions { get { return MainMenuActionsDictionary?.Keys?.ToList(); } }
        public Dictionary<string, BindingFlowAction>? MainMenuActionsDictionary { get; private set; }

        public List<string>? ContextMenuActions { get { return ContextMenuActionsDictionary?.Keys?.ToList(); } }
        public Dictionary<string, BindingFlowAction>? ContextMenuActionsDictionary { get; private set; }
        public Dictionary<string, FilterObject>? FieldFilters { get; private set; } = new Dictionary<string, FilterObject>();

        public ListFormViewModel(ILogger<ListFormViewModel> logger, IAuthState authState, IFlowRunProvider flowRunProvider, NavigationManager navigationManager):
            base(flowRunProvider)
        {
            _logger = logger;
            _authState = authState;
            _navigationManager = navigationManager;
            
            ClearData();
        }

        public async Task LoadListForm(string flowType, string pK, FlowParamsGeneric flowParams, bool clear = false)
        {
            if (clear)
            {
                ClearData();
            }

            FlowParams = flowParams ?? FlowParams;

            QueryOptions.Filters = FieldFilters.Select(f => new QueryOptions.FieldFilter
            {
                BindingProperty = f.Key,
                FilterType = f.Value.FilterType,
                Filter = f.Value.FilterValue,
                Date = f.Value.Date,
                FromDate = f.Value.FromDate,
                ToDate = f.Value.ToDate,
                Number = f.Value.Number,
                GreaterThan = f.Value.GreaterThan,
                LessThan = f.Value.LessThan,
                Text = f.Value.Text
            }).ToList();

            var data = await _flowRunProvider.GetListFlowUserView(flowType, FlowParams, QueryOptions);
            FormData = data.UserViewDetails;
            Model = data.GetModel();

            // Execute FormRuleTriggers.Loaded rules
            var ps = flowParams ?? data.GetParams();
            var allFields = data.UserViewDetails.Fields;
            var ruleRequest = FormViewModel.GetRuleRequest(FormData.ProcessTaskTypeFullName, null, null, 0, allFields, ps);
            var ruleResult = await _flowRunProvider.ExecuteFormLoadRules(ruleRequest, Model);
            Model = ruleResult.Model as IFlowModel;

            // ToDo: Check Rule validations and display properties
            if (ruleResult.FieldsDisplayProperties != null)
            {
                foreach (var prop in ruleResult.FieldsDisplayProperties)
                {
                    var disp = prop.Value;
                    var target = allFields.FirstOrDefault(f => f.Binding.Key == prop.Key)?.DisplayProperties;

                    if (target != null)
                    {
                        target.Caption = disp.Caption;
                        target.Disabled = disp.Disabled;
                        target.Highlighted = disp.Highlighted;
                        target.Required = disp.Required;
                        // ToDo: do we need this?
                        target.Binding = disp.Binding;
                        target.Name = disp.Name;
                        target.Hint = disp.Hint;
                        target.Visible = disp.Visible;
                    }
                }
            }

            Validations = ruleResult.Validations.AsEnumerable();

            // Populates form display data
            Columns = allFields.Where(c => c.ControlType != ControlType.ActionMenuItem.ToString()).ToList();
            
            VisibleColumns = Columns.Where(c => c.IsListRenderedField
                && c.DisplayProperties != null && c.DisplayProperties.Visible != false).ToList();

            ListData = ConvertHelper.ConvertToJaggedArray(data.RawDataList, data.RawDataWidth);

            var contexMenuColumn = Columns.FirstOrDefault(c => c.Binding.BindingType == FieldBindingType.TableColumnContextMenu);
            PopulateContextMenuActions(contexMenuColumn);

            var mainMenuColumn = Columns.FirstOrDefault(c => c.Binding.BindingType == FieldBindingType.ListFormContextMenu);
            PopulateMainMenuActions(mainMenuColumn);

            var refButtons = Columns.FirstOrDefault(c => c.Binding.BindingType == FieldBindingType.FlowReferenceButtons);
            PopulateReferenceButtonActions(refButtons);
        }

        public async Task ContextMenuClicking(string pk)
        {
            // Trigger ContextMenuClicking rules
            var rowIndex = ListData[0].ToList().IndexOf(pk);
            await TriggerRules(FormData.ProcessTaskTypeFullName, null, FormRuleTriggers.ContextMenuClicking, rowIndex);

            // refresh visible context menu actions
            var contexMenuColumn = Columns.FirstOrDefault(c => c.Binding.BindingType == FieldBindingType.TableColumnContextMenu);
            PopulateContextMenuActions(contexMenuColumn);
        }

        private IEnumerable<FieldControlDetails> GetAllFields()
        {
            var result = new List<FieldControlDetails>(FormData.Fields);
            return result;
        }

        private async Task<RuleEngineExecutionResult> TriggerRules(string formName, FieldBinding modelBinding, FormRuleTriggers? trigger = null,
            int rowIndex = 0)
        {
            var allFields = GetAllFields();
            var field = allFields.FirstOrDefault(f => f.Binding.Key == modelBinding?.Key);

            if (field?.FlowRules?.Any() == true || trigger != null)
            {
                var ps = FlowParams;
                var request = FormViewModel.GetRuleRequest(formName, modelBinding, trigger, rowIndex, allFields, ps);

                var ruleResult = await _flowRunProvider.TriggerRule(request, Model);
                Model = ruleResult.Model as IFlowModel;

                if (ruleResult.FieldsDisplayProperties != null)
                {
                    foreach (var prop in ruleResult.FieldsDisplayProperties)
                    {
                        var disp = prop.Value;
                        var target = allFields.FirstOrDefault(f => f.Binding.Key == prop.Key)?.DisplayProperties;

                        if (target != null)
                        {
                            target.Caption = disp.Caption;
                            target.Disabled = disp.Disabled;
                            target.Highlighted = disp.Highlighted;
                            // ToDo: do we need this?
                            target.Required = disp.Required;
                            target.Binding = disp.Binding;
                            target.Name = disp.Name;
                            target.Hint = disp.Hint;
                            target.Visible = disp.Visible;
                        }
                    }
                }

                Validations = ruleResult.Validations.AsEnumerable();
                return ruleResult;
            }

            return null;
        }


        private void ClearData()
        {
            ListData = null;
            FormData = null;
            Model = null;
            Columns = null;
            VisibleColumns = null;

            QueryOptions = new QueryOptions
            {
                PageIndex = 0,
                PageSize = 1000,
                SortDirection = SortDirection.Desc,
                SortColumn = "Created",
                AllowFiltering = false,
                AllowPagination = true
            };

            FlowParams = new FlowParamsGeneric 
            { 
            };
        }

        private void PopulateReferenceButtonActions(FieldControlDetails actions)
        {
            ReferenceButtonActionsDictionary = null;

            if (actions == null)
            {
                return;
            }

            ReferenceButtonActionsDictionary = actions.Binding.ContextMenuActions
                .Where(a => !string.IsNullOrWhiteSpace(a.Name) && IsReferenceButtonVisible(a))
                .ToDictionary(a => a.Name, a => a);
        }

        private bool IsReferenceButtonVisible(BindingFlowAction a)
        {
            var binding = $"{ModelBinding.FlowReferenceButtonsBinding}.{a.Name}";
            var field = FormData.Fields.FirstOrDefault(f => f.Binding.Binding == binding);
            return field?.DisplayProperties?.Visible != false;
        }

        private void PopulateMainMenuActions(FieldControlDetails mainMenuColumn)
        {
            MainMenuActionsDictionary = null;

            if (mainMenuColumn == null)
            {
                return;
            }

            MainMenuActionsDictionary = mainMenuColumn.Binding.ContextMenuActions
                .Where(a => !string.IsNullOrWhiteSpace(a.Name) && IsMainMenuItemVisible(a))
                .ToDictionary(a => a.Name, a => a);
        }

        private bool IsMainMenuItemVisible(BindingFlowAction a)
        {
            var binding = $"{ModelBinding.ListFormMainMenuBinding}.{a.Name}";
            var field = FormData.Fields.FirstOrDefault(f => f.Binding.Binding == binding);
            return field?.DisplayProperties?.Visible != false;
        }

        private void PopulateContextMenuActions(FieldControlDetails contexMenuColumn)
        {
            ContextMenuActionsDictionary = null;

            if (contexMenuColumn == null)
            {
                return;
            }

            ContextMenuActionsDictionary = contexMenuColumn.Binding.ContextMenuActions
                .Where(a => !string.IsNullOrWhiteSpace(a.Name) && IsContextMenuItemVisible(a))
                .ToDictionary(a => a.Name, a => a);

            if (ContextMenuActionsDictionary.Values.Any(c => !string.IsNullOrEmpty(c.ActionsBinding)))
            {
                // ToDo: Extend context menu from Model for provided ActionsBinding
            }
        }

        private bool IsContextMenuItemVisible(BindingFlowAction a)
        {
            var binding = $"{ModelBinding.ListFormContextMenuBinding}.{a.Name}";
            var field = FormData.Fields.FirstOrDefault(f => f.Binding.Binding == binding);
            return field?.DisplayProperties?.Visible != false;
        }

        public async Task NavigateActionFlow(string action)
        {
            string flowId = "";
            string flowName = MainMenuActionsDictionary[action].FlowFullName;
            string user = await _authState.UserLogin();
            string path = string.Empty;

            if (MainMenuActionsDictionary[action].IsNavigation)
            {
                path = MainMenuActionsDictionary[action].NavigationFormat;
            }
            else
            {
                await Task.Run(async () =>
                {
                    var parameters = new FlowParamsGeneric { AssignedUser = user };
                    var context = await _flowRunProvider.ExecuteFlow(flowName, null, parameters);
                    PopulateException(context);
                    flowId = context.RefId;
                });

                path = $"flow-form-generic/{flowName}/{flowId}";
            }
            if (ExecutionException != null && ExceptionType != typeof(FlowStopException).Name)
            {
                throw ExecutionException;
            }
            else
            {
                _navigationManager.NavigateTo(path);
            }
        }

        public async Task NavigateReferenceButtonActionFlow(string action)
        {
            string flowId = "";
            var flowAction = ReferenceButtonActionsDictionary[action];
            string flowName = flowAction.FlowFullName;
            string user = await _authState.UserLogin();

            await Task.Run(async () =>
            {
                var parameters = new FlowParamsGeneric { AssignedUser = user, Operation = flowAction.Operation, Tag = flowAction.Tag };
                parameters[PlatformConstants.BaseUri] = _navigationManager.BaseUri;
                var context = await _flowRunProvider.ExecuteFlow(flowName, null, parameters, action);
                PopulateException(context);
                flowId = context.RefId;
            });
            if (ExecutionException != null && ExceptionType != typeof(FlowStopException).Name)
            {
                throw ExecutionException;
            }
            else
            {
                string path = $"flow-form-generic/{flowName}/{flowId}";
                _navigationManager.NavigateTo(path);
            }
        }

        // Filters
        public void SetFilterValue(FieldControlDetails field, string filtervalue, FieldFilterPositionType? fieldFilterPositionType = null)
        {
            if (FieldFilters.ContainsKey(field.Binding.Binding))
            {
                if (field.DisplayProperties.FilterType == FieldFilterType.DateExpressionRange)
                {
                    if (fieldFilterPositionType == FieldFilterPositionType.FirstControl)
                    {
                        FieldFilters[field.Binding.Binding].FromDate = filtervalue ?? null;
                    }
                    else
                    {
                        FieldFilters[field.Binding.Binding].ToDate = filtervalue ?? null;
                    }
                }
                else if (field.DisplayProperties.FilterType == FieldFilterType.DecimalRange)
                {
                    if (fieldFilterPositionType == FieldFilterPositionType.FirstControl)
                    {
                        FieldFilters[field.Binding.Binding].GreaterThan = filtervalue ?? null;
                    }
                    else
                    {
                        FieldFilters[field.Binding.Binding].LessThan = filtervalue ?? null;
                    }
                }
                else
                {
                    FieldFilters[field.Binding.Binding].FilterValue = filtervalue;
                }
            }
            else
            {
                var filterobject = new FilterObject { FilterValue = filtervalue, FilterType = field.DisplayProperties.FilterType };

                if (field.DisplayProperties.FilterType == FieldFilterType.DateExpressionRange)
                {
                    if (fieldFilterPositionType == FieldFilterPositionType.FirstControl)
                    {
                        filterobject.FromDate = filtervalue;
                    }
                    else
                    {
                        filterobject.ToDate = filtervalue;
                    }
                }
                else if (field.DisplayProperties.FilterType == FieldFilterType.DecimalRange)
                {
                    if (fieldFilterPositionType == FieldFilterPositionType.FirstControl)
                    {
                        filterobject.GreaterThan = filtervalue;
                    }
                    else
                    {
                        filterobject.LessThan = filtervalue;
                    }
                }
                FieldFilters.Add(field.Binding.Binding, filterobject);
            }
        }

        public DateTime? GetDateFilter(FieldControlDetails field, FieldFilterPositionType? fieldFilterPositionType = null)
        {
            if (FieldFilters.ContainsKey(field.Binding.Binding))
            {
                if (FieldFilters[field.Binding.Binding].FilterType == FieldFilterType.DateExpressionRange)
                {
                    if (fieldFilterPositionType == FieldFilterPositionType.FirstControl)
                    {
                        return FieldFilters[field.Binding.Binding].FromDate != null ? DateTime.ParseExact(FieldFilters[field.Binding.Binding].FromDate, PlatformConstants.BaseDateFormat, null) : null;
                    }
                    else
                    {
                        return FieldFilters[field.Binding.Binding].ToDate != null ? DateTime.ParseExact(FieldFilters[field.Binding.Binding].ToDate, PlatformConstants.BaseDateFormat, null) : null;
                    }
                }
                else
                {
                    return FieldFilters[field.Binding.Binding].FilterValue != null ? DateTime.ParseExact(FieldFilters[field.Binding.Binding].FilterValue, PlatformConstants.BaseDateFormat, null) : null;
                }
            }
            return null;
        }

        public decimal? GetDecimalFilter(FieldControlDetails field, FieldFilterPositionType? position = null)
        {
            if (FieldFilters.ContainsKey(field.Binding.Binding))
            {
                if (FieldFilters[field.Binding.Binding].FilterType == FieldFilterType.DecimalRange)
                {
                    if (position == FieldFilterPositionType.FirstControl)
                    {
                        return FieldFilters[field.Binding.Binding].GreaterThan?.AsDecimal();
                    }
                    else
                    {
                        return FieldFilters[field.Binding.Binding].LessThan?.AsDecimal();
                    }
                }
                else
                {
                    return FieldFilters[field.Binding.Binding].FilterValue.AsDecimal();
                }
            }
            return null;
        }

        public string GetFilter(FieldControlDetails field)
        {
            if (FieldFilters.ContainsKey(field.Binding.Binding))
            {
                return FieldFilters[field.Binding.Binding].FilterValue.AsString();
            }

            return null;
        }
    }
}
