using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform;
using BlazorForms.Rendering.Interfaces;
using BlazorForms.Rendering.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Shared.FastReflection;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Net.WebSockets;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using BlazorForms.Rendering.Model;

namespace BlazorForms.Rendering
{
    public class FormViewModel<T> : FormViewModel, IFormViewModel<T> where T : class, IFlowModel
    {
        public T Model { get { return ModelUntyped as T; } }

        public FormViewModel(ILogger<FormViewModel> logger, IFlowRunProvider flowRunProvider, IDynamicFieldValidator dynamicFieldValidator,
           IJsonPathNavigator jsonPathNavigator, IModelNavigator modelNavi, IReflectionProvider reflectionProvider, NavigationManager navigationManager, 
           IModelBindingNavigator modelBindingNavigator)
                :base(logger, flowRunProvider, dynamicFieldValidator, jsonPathNavigator, modelNavi, reflectionProvider, navigationManager,
                     modelBindingNavigator)
        { 
        }
    }
    public class FormViewModel : RenderingViewModel, IFormViewModel
    {
        private Dictionary<string, FieldControlDetails> _rowFields = new Dictionary<string, FieldControlDetails>();

        protected readonly ILogger _logger;
        protected readonly IDynamicFieldValidator _dynamicFieldValidator;
        protected readonly IReflectionProvider _reflectionProvider;
        protected readonly NavigationManager _navigationManager;
        protected readonly IModelNavigator _modelNavi;
        protected readonly IModelBindingNavigator _modelBindingNavigator;

        public IFlowModel? ModelUntyped { get; protected set; }
        public IFlowParams? Params { get; protected set; }
        public IFlowContextNoModel? Context { get; protected set; }
        public string? RefId { get { return Context?.RefId; } }
        public FormDetails? FormData { get; private set; }
        public FormSettingsViewState FormSettings { get; private set; } = new FormSettingsViewState();
        public FlowParamsGeneric? FormParameters { get; private set; }
        public IEnumerable<IGrouping<string, FieldControlDetails>>? FieldsGrouped { get; private set; }
        public Dictionary<string, List<FieldControlDetails>>? Tables { get; private set; }
        public Dictionary<string, List<FieldControlDetails>>? Repeaters { get; private set; }
        public Dictionary<string, List<FieldControlDetails>>? Lists { get; private set; } = new();
        public IEnumerable<RuleExecutionResult>? Validations { get; set; }
        public IJsonPathNavigator? PathNavi { get; private set; }
        public bool FormAccessDenied { get; private set; }
        public string? FormAssignedUser { get; private set; }
        public IEnumerable<FieldControlDetails>? ActionFields { get; private set; }
        public FieldControlDetails? SubmitAction { get; private set; }
        public string? SubmitActionName { get; private set; }
        public FieldControlDetails? RejectAction { get; private set; }
        public string? RejectActionName { get; private set; }
        public string? SaveActionName { get; private set; }

        public FormViewModel(ILogger<FormViewModel> logger, IFlowRunProvider flowRunProvider, IDynamicFieldValidator dynamicFieldValidator,
            IJsonPathNavigator jsonPathNavigator, IModelNavigator modelNavi, IReflectionProvider reflectionProvider, NavigationManager navigationManager,
            IModelBindingNavigator modelBindingNavigator) :
            base(flowRunProvider)
        {
            _logger = logger;
            _dynamicFieldValidator = dynamicFieldValidator;
            PathNavi = jsonPathNavigator;
            _reflectionProvider = reflectionProvider;
            _modelNavi = modelNavi;
            _navigationManager = navigationManager;
            _modelBindingNavigator = modelBindingNavigator;
        }

        public async Task InitiateFlow(string flowName, string refId, string pk)
        {
            if (string.IsNullOrEmpty(flowName))
            {
                throw new Exception("Flow name must be supplied");
            }

            var flowParams = new FlowParamsGeneric { ItemId = pk };
            flowParams["BaseUri"] = _navigationManager.BaseUri;
            Params = flowParams;

            if (FormSettings.AllowFlowStorage)
            {
                // if refId is null - new flow will be created
                var ctx = await _flowRunProvider.ExecuteFlow(flowName, refId, flowParams);
                Context = ctx.GetClientContext();
                ModelUntyped = ctx.Model;
            }
            else
            {
                var startCtx = new ClientKeptContext { FlowName = flowName };
                var ctx = await _flowRunProvider.ExecuteClientKeptContextFlow(startCtx, null, flowParams);
                Context = ctx.GetClientContext();
                ModelUntyped = ctx.Model;
            }

            await ReloadFormData();
        }

        public async Task ApplyFormData(FormDetails form, IFlowModel model)
        {
            ModelUntyped = model;
            await Load(form);
        }

        public async Task ReloadFormData()
        {
            ClearData();

            if (Context?.ExecutionResult?.ExecutionException != null)
            {
                throw Context.ExecutionResult.ExecutionException;
            }

            if (Context?.ExecutionResult?.IsFormTask == false)
            {
                throw new Exception("The flow is not in a Form state");
            }

            var form = await _flowRunProvider.GetFormDetails(Context?.ExecutionResult?.FormId);

            if (!FormSettings.AllowAnonymousAccess)
            {
                await CheckAndSetUserAccessFlowContext(form);
            }

            if (FormSettings.AllowAnonymousAccess || !FormAccessDenied)
            {
                await Load(form);
            }
        }

        protected async Task Load(FormDetails form)
        {
            FormData = form;

            // ToDo: do we need to execute ExecuteFormLoadRules ? at the end of the method we execute TriggerRules(..., FormRuleTriggers.Loaded)
            // Execute FormRuleTriggers.Loaded rules
            var allFields = GetAllFields();
            //var ruleFields = GetFieldsWithRules(allFields);
            var ps = Context?.Params ?? (Params as FlowParamsGeneric);
            var ruleRequest = GetRuleRequest(form.ProcessTaskTypeFullName, null, null, 0, allFields, ps);
            var ruleResult = await _flowRunProvider.ExecuteFormLoadRules(ruleRequest, ModelUntyped);
            ModelUntyped = ruleResult.Model as IFlowModel;
            _modelNavi.SetModel(ModelUntyped);

            // Check Rule validations and display properties
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
                        target.Binding = disp.Binding.CopyWithKey();
                        target.Name = disp.Name;
                        target.Hint = disp.Hint;
                        target.Visible = disp.Visible;
                    }
                }
            }

            Validations = ruleResult.Validations.AsEnumerable();

            // populate fields
            ActionFields = FormData.Fields
                .Where(f => f.Binding.BindingType == FieldBindingType.ActionButton && f.Binding.ActionType != ActionType.Custom);

            RejectAction = ActionFields.FirstOrDefault(f => f.Binding.ActionType == ActionType.Reject);
            RejectActionName = RejectAction == null ? "" : RejectAction.DisplayProperties.Caption;

            SubmitAction = ActionFields.FirstOrDefault(f => f.Binding.ActionType == ActionType.Submit);
            SubmitActionName = SubmitAction == null ? "Submit" : SubmitAction.DisplayProperties.Caption;

            var saveAction = ActionFields.FirstOrDefault(f => f.Binding.ActionType == ActionType.Save);
            SaveActionName = saveAction == null ? "Save" : saveAction.DisplayProperties.Caption;

            FieldsGrouped = FormData.Fields.Where(f => f.IsRenderedField).Except(ActionFields).GroupBy(g => g.Group);

            Tables = FormData.Fields.Where(f => f.Binding.BindingType == FieldBindingType.TableColumn || f.Binding.BindingType == FieldBindingType.TableColumnSingleSelect)
                .GroupBy(g => g.Binding.TableBinding).ToDictionary(d => d.Key, d => d.ToList());

            var repeaters = FormData.Fields.Where(f => f?.DisplayProperties?.Visible == true && f.Binding.BindingType == FieldBindingType.Repeater);

            Repeaters = FormData.Fields
                .Where(f => f?.DisplayProperties?.Visible == true && repeaters.Any(x => x.Binding.TableBinding == f.Binding.TableBinding) &&
                    (f.Binding.BindingType == FieldBindingType.TableColumn || f.Binding.BindingType == FieldBindingType.TableColumnSingleSelect))
                .GroupBy(g => g.Binding.TableBinding).ToDictionary(d => d.Key, d => d.ToList());

            var lists = FormData.Fields.Where(f => f?.DisplayProperties?.Visible == true && f.Binding.BindingType == FieldBindingType.List);

            Lists = FormData.Fields
                .Where(f => f?.DisplayProperties?.Visible == true && lists.Any(x => x.Binding.TableBinding == f.Binding.TableBinding) &&
                    (f.Binding.BindingType == FieldBindingType.ListCard))
                .GroupBy(g => g.Binding.TableBinding).ToDictionary(d => d.Key, d => d.ToList());

            var ListSets = FormData.Fields
                .Where(f => f?.DisplayProperties?.Visible == true && lists.Any(x => x.Binding.TableBinding == f.Binding.TableBinding) &&
                    (f.Binding.BindingType == FieldBindingType.ListCard))
                .GroupBy(g => g.Binding.TableBinding).ToDictionary(d => d.Key, d => FieldSetControlDetails.FindAllSets(d));

            //Validations = new List<RuleExecutionResult>();

            // We already executed ExecuteFormLoadRules which triggers the same rules (FormRuleTriggers.Loaded)
            //await TriggerRules(form.ProcessTaskTypeFullName, null, FormRuleTriggers.Loaded);
        }

        public static RuleExecutionRequest GetRuleRequest(string formName, FieldBinding modelBinding, FormRuleTriggers? trigger, int rowIndex,
            IEnumerable<FieldControlDetails> allFields, FlowParamsGeneric ps)
        {
            var formDisplayProperties = allFields.Where(f => f.ControlType != "Table").Select(f => f?.DisplayProperties).Where(f => f != null).Select(d => new FieldDisplayDetails
            {
                Caption = d.Caption,
                Disabled = d.Disabled,
                Highlighted = d.Highlighted,
                Hint = d.Hint,
                Binding = d.Binding.CopyWithKey(),
                Name = d.Name,
                Required = d.Required,
                Visible = d.Visible
            }).ToArray();

            var ruleFields = allFields.Select(f => new FieldDetails
            {
                Binding = f.Binding.CopyWithKey(),
                Rules = new Collection<RuleDetails>(f.FlowRules.Select(r => new RuleDetails
                {
                    FullName = r.FormRuleType,
                    RuleCode = r.FormRuleCode,
                    RuleTriggerType = Enum.Parse<FormRuleTriggers>(r.FormRuleTriggerType),
                    IsOuterProperty = r.IsOuterProperty
                }).ToList())
            });

            var request = new RuleExecutionRequest
            {
                ProcessTaskTypeFullName = formName,
                Fields = new Collection<FieldDetails>(ruleFields.ToList())
,
                DisplayProperties = formDisplayProperties,
                FlowParams = ps,
                Trigger = trigger,
                RuleCode = null,
                FieldBinding = modelBinding?.CopyWithKey(),
                RowIndex = rowIndex
            };
            return request;
        }

        protected void ClearData()
        {
            SetInputChanged(false);
            ActionFields = new FieldControlDetails[0];
            _modelNavi.SetModel(ModelUntyped);
            _rowFields = new Dictionary<string, FieldControlDetails>();
            Validations = new List<RuleExecutionResult>();
        }

        public FieldControlDetails GetRowField(FieldControlDetails template, int row)
        {
            var binding = template.Binding.GetResolvedKey(row);

            if (!_rowFields.ContainsKey(binding))
            {
                var newField = CloneField(template); 
                newField.Binding.ResolveKey(new FieldBindingArgs { RowIndex = row });
                newField.DisplayProperties.Binding.ResolveKey(new FieldBindingArgs { RowIndex = row });
                _rowFields[binding] = newField;
            }

            return _rowFields[binding];
        }

        private FieldControlDetails CloneField(FieldControlDetails template)
        {
            var f = _reflectionProvider.CloneObject(template);
            f.Binding.FastReflectionGetter = template.Binding?.FastReflectionGetter;
            f.Binding.FastReflectionNameGetter = template.Binding?.FastReflectionNameGetter;
            f.Binding.FastReflectionIdGetter = template.Binding?.FastReflectionIdGetter;
            f.Binding.FastReflectionItemsGetter = template.Binding?.FastReflectionItemsGetter;
            f.Binding.FastReflectionTableGetter = template.Binding?.FastReflectionTableGetter;
            f.Binding.FastReflectionSetter = template.Binding?.FastReflectionSetter;

            return f;
        }

        public void ClearRowFields()
        {
            _rowFields = new Dictionary<string, FieldControlDetails>();
        }

        private IEnumerable<FieldControlDetails> GetAllFields()
        {
            var result = new List<FieldControlDetails>(FormData.Fields);
            result.AddRange(_rowFields.Values);
            return result;
        }

        protected List<FieldControlDetails> GetFieldsWithRules(IEnumerable<FieldControlDetails> fields)
        {
            var result = fields
                .Where(f => f.FlowRules.Any())
                .GroupBy(p => p.Binding.TemplateKey)
                  .Select(g => g.First())
                  .ToList();

            return result;
        }

        private async Task CheckAndSetUserAccessFlowContext(FormDetails form)
        {
            var context = new FlowContext(Context, ModelUntyped);

            if (await _flowRunProvider.CheckFormAccess(form?.Access, context))
            {
                FormAccessDenied = false;
                return;
            }

            FormAssignedUser = context.AssignedUser;
            FormAccessDenied = true;
            return;
        }

        public async Task FinishFlow(string refId, string binding = null)
        {
            await _flowRunProvider.FinishFlow(refId, ModelUntyped, binding);
        }

        public FieldControlDetails GetFieldByName(string name)
        {
            return FormData.Fields.FirstOrDefault(f => f.Name == name);
        }

        


        public async Task LoadFlowDefaultForm(string refId)
        {
            ClearData();
            var view = await _flowRunProvider.GetFlowDefaultReadonlyView(refId);
            var form = view.UserViewDetails;
            ModelUntyped = view.GetModel();
            Params = view.GetParams();
            await Load(form);
        }

        public async Task RejectForm(string binding = null, string operationName = null)
        {
            var context = await _flowRunProvider.RejectForm(Context.RefId, ModelUntyped, binding, operationName);
            Context = context;
            PopulateException(context);
        }

        public async Task SaveForm(string actionBinding = null, string operationName = null)
        {
            await _flowRunProvider.SaveForm(Context.RefId, ModelUntyped, actionBinding, operationName);
            SetInputChanged(false);
        }

        public async Task SubmitForm(string binding = null, string operationName = null)
        {
            IFlowContext ctx;

            if (FormSettings.AllowFlowStorage)
            {
                ctx = await _flowRunProvider.SubmitForm(Context.RefId, ModelUntyped, binding, operationName);
            }
            else
            {
                ctx = await _flowRunProvider.SubmitClientKeptContextFlowForm(Context.GetClientContext(), ModelUntyped, Params as FlowParamsGeneric, binding);
            }

            SetInputChanged(false);
            Context = ctx.GetClientContext();
            ModelUntyped = ctx.Model;

            PopulateException(ctx);
        }

        //public async Task<RuleEngineExecutionResult> TriggerFormLoadRulesRules()
        //{
        //    if (!Context.ExecutionResult.IsFormTask)
        //    {
        //        throw new Exception("The flow is not in a Form state");
        //    }

        //    var allFields = GetFieldsWithRules(GetAllFields());
        //    var ps = Context?.Params ?? (Params as FlowParamsGeneric);
        //    var ruleRequest = GetRuleRequest(Context.ExecutionResult.FormId, null, null, 0, allFields, ps);
        //    return await _flowRunProvider.ExecuteFormLoadRules(ruleRequest, ModelUntyped);
        //}
                
        public async Task<RuleEngineExecutionResult> TriggerRules(string formName, FieldBinding modelBinding, FormRuleTriggers? trigger = null, 
            int rowIndex = 0)
        {
            // Clear validations for this binding
            if (modelBinding != null)
            {
                Validations = Validations.Where(v => v.AffectedField != modelBinding?.ResolvedBinding).ToList();
            }

            var allFields = GetAllFields();
            var field = allFields.FirstOrDefault(f => f.Binding.Key == modelBinding?.Key);

            if (field?.FlowRules?.Any() == true || trigger != null)
            {
                var ps = Context?.Params ?? (Params as FlowParamsGeneric);
                // we should supply all fields with display properties, in other case some rule can modify display property partially and
                // not filled (not supplied initially) display property data can be lost
                var request = GetRuleRequest(formName, modelBinding, trigger, rowIndex, allFields, ps);
                //var request = GetRuleRequest(formName, modelBinding, trigger, rowIndex, GetFieldsWithRules(allFields), ps);

                var ruleResult = await _flowRunProvider.TriggerRule(request, ModelUntyped);
                ModelUntyped = ruleResult.Model as IFlowModel;
                _modelNavi.SetModel(ModelUntyped);

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
                            target.Binding = disp.Binding.CopyWithKey();
                            target.Name = disp.Name;
                            target.Hint = disp.Hint;
                            target.Visible = disp.Visible;
                        }
                    }
                }

                //Validations = ruleResult.Validations.AsEnumerable();
                Validations = Validations.Union(ruleResult.Validations);
                return ruleResult;
            }

            return null;
        }

        private IEnumerable<RuleExecutionResult> TriggerUniqueRules()
        {
            var result = new List<RuleExecutionResult>();

            if (Repeaters.Any())
            {
                foreach (var r in Repeaters.Keys)
                {
                    var errors = CheckUniqueValidationRules(r);
                    result.AddRange(errors);
                }
            }

            return result;
        }

        /// <summary>
        /// Validates every field including all Repeater rows
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RuleExecutionResult> GetDynamicFieldValidations()
        {
            // Clear all validations first
            Validations = new List<RuleExecutionResult>();

            var allFields = GetAllFields().ToList();

            allFields = allFields.Where(x => !string.IsNullOrWhiteSpace(x.Binding.Binding) && x.Binding.IsResolved &&
                    x.Binding.BindingType != FieldBindingType.ActionButton).ToList();

            var result = new List<RuleExecutionResult>();
            var uniqueValueValidations = TriggerUniqueRules();
            result.AddRange(uniqueValueValidations);

            foreach (var x in allFields)
            {
                //var r = _dynamicFieldValidator.Validate(x, PathNavi.GetValue(ModelUntyped, x.Binding.Key));
                var r = _dynamicFieldValidator.Validate(x, PathNavi.GetValue(ModelUntyped, x.Binding.Key), ModelUntyped);

                if (r != null)
                {
                    result.AddRange(r);
                }
            }

            // ToDo: why Select doesn't iterate through all list items???
            //var result = allFields.Select(x => _dynamicFieldValidator.Validate(x, PathNavi.GetValueStringKey(ModelUntyped, x.Binding.Key))).
            //    Where(x => x != null);
            
            return result;
        }

        public List<SelectableListItem> GetSelectableListData(FieldControlDetails field)
        {
            if (ModelUntyped != null)
            {
                var items = PathNavi.GetValue(ModelUntyped, field.Binding.Binding) as IEnumerable<SelectableListItem>;

                if (items != null)
                {
                    return items.ToList();
                }
            }


            return null;
        }

        public async Task<bool> CheckFormUserAccess(FormDetails form, UserViewAccessInformation accessInfo, IFlowModel model, FlowParamsGeneric flowParams)
        {
            if (await _flowRunProvider.CheckFormAccess(form?.Access, accessInfo, model, flowParams))
            {
                FormAccessDenied = false;
                return true;
            }

            FormAssignedUser = accessInfo.AssignedUser;
            FormAccessDenied = true;
            return false;
        }

        public List<RuleExecutionResult> CheckUniqueValidationRules(string tableBinding)
        {
            var errors = new List<RuleExecutionResult>();

            if (!string.IsNullOrWhiteSpace(tableBinding))
            {
                // clean all validations fo this type
                var message = "This field should be unique";
                Validations = Validations.Where(v => v.ValidationMessage != message);
                var columns = Repeaters[tableBinding];
                var uniqueColumns = columns.Where(c => c.DisplayProperties.IsUnique);
                var list = FieldGetItemsValue(ModelUntyped, tableBinding) as IEnumerable<object>;

                foreach (var f in uniqueColumns)
                {
                    var values = list.Select(x => FieldGetValue(x, f.Binding)).ToList();

                    for (int i = 0; i < values.Count(); i++)
                    {
                        if (values.Where(v => v?.ToString() == values[i]?.ToString()).Count() > 1)
                        {
                            //f.Binding.ResolveKey(new FieldBindingArgs { RowIndex = i });

                            var validation = new RuleExecutionResult
                            {
                                AffectedField = f.Binding.GetResolvedKey(i),
                                RuleCode = f.DisplayProperties.Caption,
                                ValidationMessage = message,
                                ValidationResult = RuleValidationResult.Error
                            };

                            errors.Add(validation);
                        }
                    }
                }

                if (errors.Any())
                {
                    Validations = Validations.Union(errors);
                }
            }

            return errors;
        }

        // ToDo: try to move all calls from _modelNavi to _modelBindingNavigator
        // Model Navigation
        [Obsolete]
        public object ModelNaviGetValueObject(string modelBinding)
        {
            _modelNavi.SetModel(ModelUntyped);
            return _modelNavi.GetValueObject(modelBinding);
        }

        [Obsolete]
        public object ModelNaviGetValue(string tableBinding, int rowIndex, string modelBinding)
        {
            _modelNavi.SetModel(ModelUntyped);
            return _modelNavi.GetValue(tableBinding, rowIndex, modelBinding);
        }

        [Obsolete]
        public string ModelNaviGetValue(string modelBinding)
        {
            _modelNavi.SetModel(ModelUntyped);
            return _modelNavi.GetValue(modelBinding);
        }

        [Obsolete]
        public void ModelNaviSetValue(string modelBinding, object val)
        {
            _modelNavi.SetModel(ModelUntyped);
            _modelNavi.SetValue(modelBinding, val);
        }

        [Obsolete]
        public void ModelNaviSetValue(string tableBinding, int rowIndex, string modelBinding, object val)
        {
            _modelNavi.SetModel(ModelUntyped);
            _modelNavi.SetValue(tableBinding, rowIndex, modelBinding, val);
        }

        [Obsolete]
        public IEnumerable<object> ModelNaviGetItems(string itemsBinding)
        {
            _modelNavi.SetModel(ModelUntyped);
            return _modelNavi.GetItems(itemsBinding);
        }

        // FastReflection
        public object FieldGetValue(object model, FieldBinding binding)
        {
            return _modelBindingNavigator.GetValue(model, binding);
        }

        public object FieldGetNameValue(object model, FieldBinding binding)
        {
            return _modelBindingNavigator.GetNameValue(model, binding);
        }

        public object FieldGetIdValue(object model, FieldBinding binding)
        {
            return _modelBindingNavigator.GetIdValue(model, binding);
        }

        public IEnumerable<object> FieldGetItemsValue(object model, FieldBinding binding)
        {
            return _modelBindingNavigator.GetItems(model, binding);
        }

        public IEnumerable<object> FieldGetItemsValue(object model, string modelBinding)
        {
            return _modelBindingNavigator.GetItems(model, modelBinding);
        }

        public IEnumerable<object> FieldGetTableValue(object model, FieldBinding binding)
        {
            return _modelBindingNavigator.GetTable(model, binding);
        }

        public object FieldGetRowValue(object model, FieldBinding binding, int rowIndex)
        {
            return _modelBindingNavigator.GetRowValue(model, binding, rowIndex);
        }


        public void FieldSetValue(object model, FieldBinding binding, object value)
        {
            _modelBindingNavigator.SetValue(model, binding, value);
        }

        public List<FormConfirmationData> GetAvailableConfirmations(ConfirmType confirmType, string? binding = null)
        {
            var result =new List<FormConfirmationData>();

            var list = GetAllFields().Where(f => f.DisplayProperties?.Confirmations != null)
                .SelectMany(f => f.DisplayProperties.Confirmations).Select(c => new FormConfirmationData(c));

            if (binding == null)
            {
                // Form Confirmations
                if (_changed && confirmType == ConfirmType.ChangesWillBeLost)
                {
                    result.AddRange(list.Where(c => c.Type == ConfirmType.ChangesWillBeLost));
                }
                
                result.AddRange(list.Where(c => c.Type == confirmType && c.Type != ConfirmType.ChangesWillBeLost));
            }

            return result;
        }

        protected bool _changed;
        protected bool _ignoreChanged;

        public void SetInputChanged(bool changed = true)
        {
            if (!_ignoreChanged && changed)
            {
                _changed = true;
                return;
            }

            _changed = changed;
        }

        public void IgnoreInputChanged()
        {
            _ignoreChanged = true;
        }

        public void RestoreInputChanged()
        {
            _ignoreChanged = false;
        }

        public void RefreshValidations(FieldControlDetails field)
        {
            Validations = Validations.Where(v => v.AffectedField != field.Binding.ResolvedBinding).ToList();

            var r = _dynamicFieldValidator.Validate(field, PathNavi.GetValue(ModelUntyped, field.Binding.Key), ModelUntyped);

            if (r != null)
            {
                Validations = Validations.Union(r);
            }
        }

        public IEnumerable<RuleExecutionResult> GetValidations(FieldControlDetails field)
        {
            return Validations.Where(v => v.AffectedField == field.Binding.ResolvedBinding);
        }
    }
}
