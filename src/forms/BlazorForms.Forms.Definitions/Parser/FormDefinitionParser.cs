using BlazorForms.Shared;
using BlazorForms.Shared.FastReflection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace BlazorForms.Forms
{
    public enum BindingType
    {
        Simple,
        TableColumn,
        TableCount,
        Repeater,
        RepeaterColumn
    }
    public class FormDefinitionParser : IFormDefinitionParser
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IFastReflectionProvider _fastReflectionProvider;

        public FormDefinitionParser(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _fastReflectionProvider = _serviceProvider.GetRequiredService<IFastReflectionProvider>();
        }

        public FormDetails Parse(Type formType)
        {
            if (typeof(IModelDefinitionForm).IsAssignableFrom(formType))
            {
                return ParseFluentForm(formType);
            }

            var formAttr = formType.GetCustomAttributes().FirstOrDefault(attr => attr.GetType() == typeof(FormAttribute)) as FormAttribute;
            var details = new FormDetails { DisplayName = formAttr?.Name, ChildProcessTypeFullName = formAttr?.ChildProcess?.FullName };

            if (formType.GetCustomAttributes().FirstOrDefault(attr => attr.GetType() == typeof(AllowAccessAttribute)) is AllowAccessAttribute formAccessAttr)
            {
                details.Access = new FormAccessDetails { OnlyAssignee = formAccessAttr.OnlyAssignee, Roles = formAccessAttr.Roles };
                details.Access.CustomRule = new FormFlowRuleDetails { FormRuleCode = formAccessAttr.CustomRule.Name };
            }
            else
            {
                details.Access = new FormAccessDetails(); //Maybe should fill properties
            }

            var parameters = TypeHelper.GetConstructorParameters(_serviceProvider, formType);
            var form = Activator.CreateInstance(formType, parameters) as IFormDefinition;

            details.ProcessTaskTypeFullName = form?.ProcessTaskTypeFullName ?? formType.FullName;
            details.Name = form?.Name ?? formType.Name;
            var includeTypes = new List<Type> { formType };
            while (includeTypes.Last().BaseType.GetCustomAttributes().Any(attr => attr.GetType() == typeof(BaseFormAttribute)))
            {
                includeTypes.Add(includeTypes.Last().BaseType);
            }

            var excludeFields = includeTypes.Last().BaseType.GetProperties().Select(p => p.Name);
            var fields = formType.GetProperties().Where( f => !excludeFields.Contains(f.Name));
            details.Fields = new List<FieldControlDetails>();

            //var getBindingType = new Func<TableColumnBindingControlType, TableCountBindingControlType, RepeaterBindingControlType, string>((a, b, c) =>
            //{
            //    if (c != null)
            //    {
            //        return BindingType.Repeater.ToString();
            //    }
            //    if (a != null)
            //    {
            //        return BindingType.TableColumn.ToString();
            //    }
            //    if (b != null)
            //    {
            //        return BindingType.TableCount.ToString();
            //    }
            //    return BindingType.Simple.ToString();
            //});

            int order = 0;

            foreach(var field in fields)
            {
                var defAttr = field.GetCustomAttributes().FirstOrDefault(attr => attr.GetType() == typeof(FormComponentAttribute)) as FormComponentAttribute;
                var groupAttr = field.GetCustomAttributes().FirstOrDefault(attr => attr.GetType() == typeof(ComponentGroupAttribute)) as ComponentGroupAttribute;
                var fieldValue = field.GetValue(form);

                // read as FieldBinding
                var fieldBinding = fieldValue as FieldBinding;

                var control = fieldValue as IBindingControlType;
                var listControl = fieldValue as ListBindingControlType;
                var repeaterControl = fieldValue as RepeaterBindingControlType;
                var tableCountControl = fieldValue as TableCountBindingControlType;
                var selectableListControl = fieldValue as SelectableListBindingControlType;

                var modelTableBinding = fieldValue is TableColumnBindingControlType tableColumnControl
                    ? tableColumnControl.TableBinding
                    : tableCountControl?.TableBinding;

                modelTableBinding ??= repeaterControl?.TableBinding;

                var newControl = new FieldControlDetails
                {
                    Name = field.Name,
                    Group = groupAttr?.Name ?? defAttr?.Group,
                    Caption = defAttr?.Caption ?? "",
                    ControlType = defAttr?.FormComponentType?.Name,

                    // new binding concept
                    Binding = fieldBinding,
                };

                if (newControl.ControlType == typeof(CustomComponent<>).Name)
                {
                    var ct = defAttr.FormComponentType.GetGenericArguments()[0];
                    var c = Activator.CreateInstance(ct) as IFormComponent;
                    newControl.ControlType = $"{typeof(CustomComponent<>).Name} {c.GetFullName()}";
                }

                if(field.GetCustomAttributes().FirstOrDefault(attr => attr.GetType() == typeof(DisplayAttribute)) is DisplayAttribute displayAttr)
                {
                    newControl.DisplayProperties = new FormDisplayDetails
                    {
                        Caption = displayAttr.Caption,
                        Visible = displayAttr.Visible,
                        Required = displayAttr.Required,
                        Disabled = displayAttr.Disabled,
                        Highlighted = displayAttr.Highlighted,
                        Password = displayAttr.Password,
                        Hint = displayAttr.Hint,
                        NoCaption = displayAttr.NoCaption,
                        Name = field.Name,
                        FilterType = displayAttr.FilterType,
                        FilterRefField = displayAttr.FilterRefField,

                        // new binding concept
                        Binding = fieldBinding,

                        //ModelBinding = newControl.ModelBinding ?? newControl.ModelTableBinding,
                        IsPrimaryKey = displayAttr?.IsPrimaryKey
                    };

                    if(displayAttr.IsPrimaryKey)
                    {
                        details.PkColumn = field.Name;
                    }
                }

                newControl.Order = order;
                order += 10;
                details.Fields.Add(newControl);
            }

            return details;
        }

        private FormDetails ParseFluentForm(Type formType)
        {
            var form = Activator.CreateInstance(formType) as IModelDefinitionForm;

            var details = new FormDetails 
            {
                ProcessTaskTypeFullName = formType.FullName,
                DisplayName = form.DisplayName, 
                ChildProcessTypeFullName = form.ChildProcess?.FullName,
                Fields = new List<FieldControlDetails>(),
            };

            if (form.Access != null)
            {
                details.Access = new FormAccessDetails { OnlyAssignee = form.Access.OnlyAssignee, Roles = form.Access.Roles };
                details.Access.CustomRule = new FormFlowRuleDetails { FormRuleCode = form.Access.CustomRule.Name };
            }
            else
            {
                details.Access = new FormAccessDetails();
            }

            var fields = form.GetDetailsFields();
            int order = 0;

            // ToDo: should be moved to form.GetDetailsFields()
            // Add Form as a field with rules
            //var formControl = new FieldControlDetails
            //{
            //    Name = details.DisplayName,
            //    Caption = "",
            //    ControlType = ControlType.Form.ToString(),

            //    // new binding concept
            //    Binding = new FieldBinding
            //    {
            //        Binding = ModelBinding.FormLevelBinding,
            //        BindingType = FieldBindingType.Form,
            //    },
            //};

            ////FillRules(formControl, form);
            //AddControl(formControl);

            // Add field one by one
            foreach (var field in fields)
            {
                // Set FastReflection delegates
                UpdateFastReflectionDelegates(field.Binding, form.GetDetailsType());

                var newControl = new FieldControlDetails
                {
                    Name = field.Name,
                    Group = field.Group,
                    Caption = field.Label ?? "",
                    ControlType = field.ControlType?.Name ?? field.ControlTypeName,

                    // new binding concept
                    Binding = field.Binding,
                };

                // CustomComponent
                if (newControl.ControlType == typeof(CustomComponent<>).Name)
                {
                    throw new NotImplementedException("Fluent forms custom components still not implemented");
                    //var ct = defAttr.FormComponentType.GetGenericArguments()[0];
                    //var c = Activator.CreateInstance(ct) as IFormComponent;
                    //newControl.ControlType = $"{typeof(CustomComponent<>).Name} {c.GetFullName()}";
                }

                // DisplayAttribute
                newControl.DisplayProperties = new FormDisplayDetails
                {
                    Caption = field.Label,
                    Visible = !field.Hidden,
                    Required = field.Required,
                    Disabled = field.ReadOnly == true,
                    Highlighted = field.Highlighted,
                    Password = field.Password,
                    Hint = field.Hint,
                    NoCaption = field.NoCaption,
                    Name = field.Name,
                    FilterType = field.FilterType,
                    FilterRefField = field.FilterRefField,
                    Format= field.Format,
                    IsUnique = field.Unique,

                    // new binding concept
                    Binding = field.Binding,

                    //ModelBinding = newControl.ModelBinding ?? newControl.ModelTableBinding,
                    IsPrimaryKey = field.PrimaryKey
                };

                if (field.PrimaryKey)
                {
                    details.PkColumn = field.Name;
                }

                // rules
                FillRules(newControl, field);
                //newControl.FlowRules = new Collection<FormFlowRuleDetails>(field.Rules.Select(r => new FormFlowRuleDetails
                //{
                //    // RuleCode can be resolved in Flow Provider
                //    FormRuleType = r.RuleType.FullName,
                //    FormRuleTriggerType = r.Trigger.GetDescription(),
                //    IsOuterProperty = r.IsOuterProperty,
                //}).ToList());

                AddControl(newControl);
            }

            // Buttons
            var buttons = form.GetButtons();

            foreach (var button in buttons)
            {
                var binding = new FieldBinding
                {
                    Binding = GetBindingByActionType(button.Action),
                    BindingType = FieldBindingType.ActionButton,
                };

                var newControl = new FieldControlDetails
                {
                    Name = button.Action.ToString(),
                    //Group = groupAttr?.Name ?? defAttr?.Group,
                    Caption = button.Text ?? button.Action.ToString(),
                    //ControlType = field.ControlType?.Name,
                    ActionLink = button.LinkText,
                    Binding = binding,
                };

                newControl.DisplayProperties = new FormDisplayDetails
                {
                    Name = newControl.Name,
                    Binding = newControl.Binding,
                    Hint = button.Hint,
                    Caption = newControl.Caption,
                    Visible = true,
                    Disabled = false,
                };

                AddControl(newControl);
            }

            // Navigation buttons
            var refButtons = form.GetButtonNavigations();

            if (refButtons.Any())
            {
                var refBinding = new FieldBinding
                {
                    Binding = ModelBinding.FlowReferenceButtonsBinding,
                    BindingType = FieldBindingType.FlowReferenceButtons,
                    ContextMenuActions = refButtons.Select(a => a.GetAction()).ToList()
                };

                var newControl = new FieldControlDetails { Binding = refBinding };
                AddControl(newControl);
                var items = GetContextMenuActionItems(refBinding, FieldBindingType.FlowReferenceButtonsItem);
                details.Fields.AddRange(items);
            }

            // Context menu
            var contextButtons = form.GetContextLinks();

            if (contextButtons.Any())
            {
                var menuBinding = new FieldBinding
                {
                    TableBinding = form.ItemsPath, //JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
                    BindingType = FieldBindingType.TableColumnContextMenu,
                    ContextMenuActions = contextButtons.Select(a => GetAction(a)).ToList()
                };

                var menuControl = new FieldControlDetails { Binding = menuBinding };
                AddControl(menuControl);
                var items = GetContextMenuActionItems(menuBinding, FieldBindingType.ListFormContextMenuItem);
                details.Fields.AddRange(items);
            }
                            
            return details;

            void AddControl(FieldControlDetails ctrl)
            {
                ctrl.Order = order;
                order += 10;
                details.Fields.Add(ctrl);
            }

            void FillRules(FieldControlDetails ctrl, DataField field)
            {
                ctrl.FlowRules = new Collection<FormFlowRuleDetails>(field.Rules.Select(r => new FormFlowRuleDetails
                {
                    // RuleCode can be resolved in Flow Provider
                    FormRuleType = r.RuleType.FullName,
                    FormRuleTriggerType = r.Trigger.GetDescription(),
                    IsOuterProperty = r.IsOuterProperty,
                }).ToList());
            }
        }

        private void UpdateFastReflectionDelegates(FieldBinding binding, Type modelType)
        {
            // populate binding fast reflection delegates
            _fastReflectionProvider.UpdateBindingFastReflection(binding, modelType);
        }

        private List<FieldControlDetails> GetContextMenuActionItems(FieldBinding refBinding, FieldBindingType fieldBindingType)
        {
            var result = new List<FieldControlDetails>();
            int order = 0;

            foreach (var a in refBinding.ContextMenuActions)
            {
                var binding = new FieldBinding
                {
                    Binding = GenerateContextMenuActionBinding(refBinding, a),
                    BindingType = fieldBindingType,
                };

                var newControl = new FieldControlDetails { Binding = binding, ControlType = ControlType.ActionMenuItem.ToString() };

                newControl.DisplayProperties = new FormDisplayDetails
                {
                    Name = newControl.Name,
                    Binding = newControl.Binding,
                    //Hint = a.Hint,
                    Caption = newControl.Caption,
                    Visible = true,
                    Disabled = false,
                };

                newControl.Order = order;
                order += 10;
                result.Add(newControl);
            }

            return result;
        }

        private string GenerateContextMenuActionBinding(FieldBinding refBinding, BindingFlowAction a)
        {
            var main = refBinding.Binding ?? ModelBinding.ListFormContextMenuBinding;
            var result = $"{main}.{a.Name}";
            return result;
        }

        private BindingFlowAction GetAction(ActionRouteLink a)
        {
            var result = new BindingFlowAction 
            { 
                Name = a.Text,
                NavigationFormat = a.LinkText,
                IsNavigation = a.IsNavigation,
                FlowFullName = a.FlowType?.FullName,
                Operation = a.Operation
            };

            return result;
        }

        private static string GetBindingByActionType(ButtonActionTypes actionType)
        {
            return actionType switch
            {
                ButtonActionTypes.Reject => ModelBinding.RejectButtonBinding,
                ButtonActionTypes.Submit => ModelBinding.SubmitButtonBinding,
                ButtonActionTypes.Save => ModelBinding.SaveButtonBinding,
                ButtonActionTypes.Close => ModelBinding.CloseButtonBinding,
                ButtonActionTypes.Cancel => ModelBinding.CloseButtonBinding,
                ButtonActionTypes.CloseFinish => ModelBinding.CloseFinishButtonBinding,
                ButtonActionTypes.SubmitClose => ModelBinding.SubmitCloseButtonBinding,
                ButtonActionTypes.Delete => ModelBinding.DeleteButtonBinding,
                ButtonActionTypes.Edit => ModelBinding.EditButtonBinding,
                ButtonActionTypes.Custom => ModelBinding.CustomButtonBinding,
                _ => throw new Exception($"ActionType {actionType} binding is not found"),
            };
        }
    }
}
