using BlazorForms.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using BlazorForms.Shared;
using System.Collections.ObjectModel;

namespace BlazorForms.Forms.Definitions.JsonForms
{
    public static class JsonFormConverter
    {
        public static StoreForm ToJsonForm(FormDetails form)
        {
            var data = new StoreForm
            {
                Caption = form.Caption,
                DisplayName = form.DisplayName,
                Name = form.ProcessTaskTypeFullName,
                ChildProcessName = form.ChildProcessTypeFullName,
            };

            foreach (var field in form.Fields)
            {
                var newControl = new StoreFormField
                {
                    Name = field.Name,
                    Group = field.Group,
                    Label = field.Caption ?? "",
                    ControlType = field.ControlType,

                    BindingProperty = field.Binding.Binding,
                    TableBindingProperty = field.Binding.TableBinding,
                    BindingControlType = field.Binding.BindingControlType,

                    Hidden = !field.DisplayProperties.Visible,
                    Required = field.DisplayProperties.Required,
                    ReadOnly = field.DisplayProperties.Disabled == true,
                    Highlighted = field.DisplayProperties.Highlighted,
                    Password = field.DisplayProperties.Password,
                    Hint = field.DisplayProperties.Hint,
                    NoCaption = field.DisplayProperties.NoCaption,

                    PrimaryKey = field.DisplayProperties.IsPrimaryKey == true,
                };

                data.Fields.Add(newControl);
            }
                
            return data;
        }

        public static FormDetails FromJsonForm(StoreForm form)
        {
            var details = new FormDetails
            {
                ProcessTaskTypeFullName = form.Name,
                DisplayName = form.DisplayName,
                ChildProcessTypeFullName = form.ChildProcessName,
                Fields = new List<FieldControlDetails>(),
            };

            // ToDo: implement form.Access
            //if (form.Access != null)
            //{
            //    details.Access = new FormAccessDetails { OnlyAssignee = form.Access.OnlyAssignee, Roles = form.Access.Roles };
            //    details.Access.CustomRule = new FormFlowRuleDetails { FormRuleCode = form.Access.CustomRule.Name };
            //}
            //else
            //{
            //    details.Access = new FormAccessDetails();
            //}

            int order = 0;

            // Add field one by one
            foreach (var field in form.Fields)
            {
                var newControl = new FieldControlDetails
                {
                    Name = field.Name,
                    Group = field.Group,
                    Caption = field.Label ?? "",
                    ControlType = field.ControlType,

                    // new binding concept
                    Binding = new FieldBinding 
                    { 
                        Binding = field.BindingProperty, 
                        TableBinding = field.TableBindingProperty, 
                        BindingControlType = field.BindingControlType,
                        BindingType = ResolveBindingType(field),
                    },
                };

                // CustomComponent
                if (newControl.ControlType == typeof(CustomComponent<>).Name)
                {
                    throw new NotImplementedException("Json forms custom components still not implemented");
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
                    //FilterType = field.FilterType,
                    //FilterRefField = field.FilterRefField,

                    Binding = newControl.Binding,

                    IsPrimaryKey = field.PrimaryKey
                };

                if (field.PrimaryKey)
                {
                    details.PkColumn = field.Name;
                }

                // rules
                FillRules(newControl, field);

                AddControl(newControl);
            }

            // No buttoms for now
            // Buttons
            //var buttons = form.GetButtons();

            //foreach (var button in buttons)
            //{
            //    var binding = new FieldBinding
            //    {
            //        Binding = GetBindingByActionType(button.Action),
            //        BindingType = FieldBindingType.ActionButton,
            //    };

            //    var newControl = new FieldControlDetails
            //    {
            //        Name = button.Action.ToString(),
            //        //Group = groupAttr?.Name ?? defAttr?.Group,
            //        Caption = button.Text ?? button.Action.ToString(),
            //        //ControlType = field.ControlType?.Name,
            //        ActionLink = button.LinkText,
            //        Binding = binding,
            //    };

            //    newControl.DisplayProperties = new FormDisplayDetails
            //    {
            //        Name = newControl.Name,
            //        Binding = newControl.Binding,
            //        Hint = button.Hint,
            //        Caption = newControl.Caption,
            //        Visible = true,
            //        Disabled = false,
            //    };

            //    AddControl(newControl);
            //}

            // No buttoms for now
            // Navigation buttons
            //var refButtons = form.GetButtonNavigations();

            //if (refButtons.Any())
            //{
            //    var refBinding = new FieldBinding
            //    {
            //        Binding = ModelBinding.FlowReferenceButtonsBinding,
            //        BindingType = FieldBindingType.FlowReferenceButtons,
            //        ContextMenuActions = refButtons.Select(a => a.GetAction()).ToList()
            //    };

            //    var newControl = new FieldControlDetails { Binding = refBinding };
            //    AddControl(newControl);
            //    var items = GetContextMenuActionItems(refBinding, FieldBindingType.FlowReferenceButtonsItem);
            //    details.Fields.AddRange(items);
            //}

            // No Context menu for now
            // Context menu
            //var contextButtons = form.GetContextLinks();

            //if (contextButtons.Any())
            //{
            //    var menuBinding = new FieldBinding
            //    {
            //        TableBinding = form.ItemsPath, //JsonPathHelper.ReplaceLambdaVar(items.Body.ToString()),
            //        BindingType = FieldBindingType.TableColumnContextMenu,
            //        ContextMenuActions = contextButtons.Select(a => GetAction(a)).ToList()
            //    };

            //    var menuControl = new FieldControlDetails { Binding = menuBinding };
            //    AddControl(menuControl);
            //    var items = GetContextMenuActionItems(menuBinding, FieldBindingType.ListFormContextMenuItem);
            //    details.Fields.AddRange(items);
            //}

            return details;

            void AddControl(FieldControlDetails ctrl)
            {
                ctrl.Order = order;
                order += 10;
                details.Fields.Add(ctrl);
            }

            void FillRules(FieldControlDetails ctrl, StoreFormField field)
            {
                // ToDo: decide what to do with rules code
                //ctrl.FlowRules = new Collection<FormFlowRuleDetails>(field.Rules.Select(r => new FormFlowRuleDetails
                //{
                //    // RuleCode can be resolved in Flow Provider
                //    FormRuleType = r.RuleType.FullName,
                //    FormRuleTriggerType = r.Trigger.GetDescription(),
                //    IsOuterProperty = r.IsOuterProperty,
                //}).ToList());
            }
        }

        private static FieldBindingType ResolveBindingType(StoreFormField field)
        {
            switch (field.BindingControlType)
            {
                case "TableBindingControlType":
                    return FieldBindingType.Table;
                case "TableColumnBindingControlType":
                    return FieldBindingType.TableColumn;
                default:
                    return FieldBindingType.SingleField;
            }
        }

        public static FormDetails FromJsonForm(string json)
        {
            var data = JsonSerializer.Deserialize<StoreForm>(json);
            return FromJsonForm(data);
        }
    }
}
