using Castle.Components.DictionaryAdapter;
using BlazorForms.Shared;
using BlazorForms.Shared.DataStructures;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlazorForms.Platform.CodeGeneration
{
    public class FormGenerator : IModelCentricCodeGenerator
    {
        public ModelCentricGenerationResult GetCode(ModelCentricGenerationParams p)
        {
            var result = new ModelCentricGenerationResult();
            var modelName = p.ModelType.Name;
            var className = $"{CodeConst.FormPrefix}{p.ModelType.Name.Replace(CodeConst.ModelSuffix, "")}";
            result.FileName = className;

            var sb = new StringBuilder();
            var sb2 = new StringBuilder();
            sb.AppendLine(@"using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Logs;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Platform.Shared.Attributes;
using BlazorForms.FlowRules;");

            sb.AppendLine($"using {p.ModelType.Namespace};");
            sb.AppendLine();
            sb.AppendLine($"namespace {p.ModelType.Namespace.ReplaceEnd(CodeConst.ModelNamespaceEnd, "")}");
            sb.AppendLine("{");
            sb.AppendLine($"    [Form(\"{p.ModelType.Name.Replace(CodeConst.ModelSuffix, "").SplitWords()}\")]");
            sb.AppendLine($"    public class {className} : FlowTaskDefinitionBase<{modelName}>");
            sb.AppendLine("    {");

            var props = TypeHelper.GetNestedPublicVirtualProperties(p.ModelType);

            foreach (var prop in props)
            {
                var propName = prop.Property.Name;

                if (prop.Parent?.IsList == true && prop.Parent.Property.Name.EndsWith(CodeConst.RefSuffix))
                {
                    continue;
                }

                if (prop.IsList)
                {
                    if (prop.Property.Name.EndsWith(CodeConst.RefSuffix))
                    {
                        continue;
                    }

                    string ruleName = $"{className}{prop.Property.Name}{CodeConst.SelectedRuleSuffix}";
                    string ruleAddName = $"{className}{prop.Property.Name}{CodeConst.AddRuleSuffix}";
                    sb.AppendLine();
                    sb.AppendLine($"        // Table for {prop.GetPath()}");
                    sb.AppendLine();
                    sb.AppendLine($"        [FlowRule(typeof({ruleName}), FormRuleTriggers.Changed)]");
                    sb.AppendLine($"        [FlowRule(typeof({ruleAddName}), FormRuleTriggers.ItemAdded)]");
                    sb.AppendLine($"        [FormComponent(typeof(Repeater))]");
                    sb.AppendLine($"        // [FormComponent(typeof(Table))]");
                    sb.AppendLine($"        [Display(\"{prop.Property.Name.SplitWords()}\")]");
                    sb.AppendLine($"        // public object {propName} => Table(t => t.{prop.GetPath()});");
                    sb.AppendLine($"        public object {propName} => Repeater(t => t.{prop.GetPath()});");

                    sb2.AppendLine();
                    sb2.AppendLine($"    public class {ruleName} : FlowRuleAsyncBase<{modelName}>");
                    sb2.AppendLine("    {");
                    sb2.AppendLine($"        public override string RuleCode => nameof({ruleName});");
                    sb2.AppendLine();
                    sb2.AppendLine($"        public override async Task Execute({modelName} model)");
                    sb2.AppendLine("        {");
                    sb2.AppendLine($"            // model.SelectedMemberIndex = RunParams.RowIndex;");
                    sb2.AppendLine("        }");
                    sb2.AppendLine("    }");
                    sb2.AppendLine();

                    sb2.AppendLine($"    public class {ruleAddName} : FlowRuleAsyncBase<{modelName}>");
                    sb2.AppendLine("    {");
                    sb2.AppendLine($"        public override string RuleCode => nameof({ruleAddName});");
                    sb2.AppendLine();
                    sb2.AppendLine($"        public override async Task Execute({modelName} model)");
                    sb2.AppendLine("        {");
                    sb2.AppendLine("            //model.Roles[RunParams.RowIndex].HourlyRate = new Money{ Currency = model.Project.BaseCurrencySearch };");
                    sb2.AppendLine("        }");
                    sb2.AppendLine("    }");
                }
                else if (prop.Property.PropertyType.IsSimple() || prop.Property.PropertyType == typeof(Money))
                {
                    sb.AppendLine();
                    WriteControlByProperty(sb, prop, props);
                }
            }

            sb.AppendLine();
            sb.AppendLine("        [Display(\"Cancel\")]");
            sb.AppendLine("        public object CancelButton => ActionButton(ActionType.Close);");
            sb.AppendLine("        [Display(\"Submit\")]");
            sb.AppendLine("        public object SubmitButton => ActionButton(ActionType.Submit);");
            sb.AppendLine("    }");

            if (sb2.Length > 0)
            {
                sb.AppendLine(sb2.ToString());
            }

            sb.AppendLine("}");

            result.Code = sb.ToString();
            return result;
        }

        private void WriteControlByProperty(StringBuilder sb, ModelProperty prop, List<ModelProperty> props)
        {
            var p = prop.Property;
            var isColumn = (prop.Parent?.IsList == true);

            if (p.Name.EndsWith(CodeConst.SearchSuffix))
            {
                var refList = FindRefList(prop, props);

                if (refList != null)
                {
                    if (isColumn)
                    {
                        sb.AppendLine($"        // Warning: Autocomplete inside Repeater is not supported yet, for property {prop.GetPath()}");
                        sb.AppendLine($"        public object {prop.GetPath().Replace(".", "")} {{get; set;}}");
                    }
                    else
                    {
                        sb.AppendLine($"        [FormComponent(typeof(Autocomplete))]");
                        sb.AppendLine($"        [Display(\"{p.Name.Replace(CodeConst.ModelSuffix, "").SplitWords()}\")]");
                        sb.AppendLine($"        public object {prop.GetPath().Replace(".", "")} => EditWithOptions(a => a.{prop.GetPath()}, e => e.{refList.List.GetPath()}, m => m.{refList.Id});");
                    }
                }
                else
                {
                    sb.AppendLine($"        // Warning: Cannot find Reference List for property {prop.GetPath()}");
                }
            }
            else if (p.Name.EndsWith(CodeConst.IdSuffix) || p.Name.EndsWith(CodeConst.CodeSuffix))
            {
                var refList = FindRefList(prop, props);

                if (refList != null)
                {
                    sb.AppendLine($"        [FormComponent(typeof(DropDown))]");

                    if (isColumn)
                    {
                        sb.AppendLine($"        [Display(\"{p.Name.Replace(CodeConst.ModelSuffix, "").SplitWords()}\")]");
                        sb.AppendLine($"        public object {prop.GetPath().Replace(".", "")} => TableColumnSingleSelect(t => t.{prop.Parent.GetPath()}, c => c.{prop.GetPath()}, p => p.{refList.List.GetPath()}, m => m.{refList.Id}, m => m.{refList.Value});");
                    }
                    else
                    {
                        sb.AppendLine($"        [Display(\"{p.Name.Replace(CodeConst.ModelSuffix, "").SplitWords()}\")]");
                        sb.AppendLine($"        public object {prop.GetPath().Replace(".", "")} => SingleSelect(c => c.{prop.GetPath()}, p => p.{refList.List.GetPath()}, m => m.{refList.Id}, m => m.{refList.Value});");
                    }
                }
                else
                {
                    sb.AppendLine($"        // Warning: Cannot find Reference List for property {prop.GetPath()}");
                }
            }
            else if (p.PropertyType == typeof(int) || p.PropertyType == typeof(int?)
                || p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?)
                || p.PropertyType == typeof(string)
                || p.PropertyType == typeof(float) || p.PropertyType == typeof(float?)
                || p.PropertyType == typeof(double) || p.PropertyType == typeof(double?))
            {
                sb.AppendLine($"        [FormComponent(typeof(TextEdit))]");
            }
            else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
            {
                sb.AppendLine($"        [FormComponent(typeof(DateEdit))]");
            }
            else if (p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?))
            {
                sb.AppendLine($"        [FormComponent(typeof(Checkbox))]");
            }
            else if (p.PropertyType == typeof(Money))
            {
                sb.AppendLine($"        [FormComponent(typeof(MoneyEdit))]");
            }
            else if (p.Name.EndsWith(CodeConst.PercentSuffix))
            {
                sb.AppendLine($"        [FormComponent(typeof(PercentEdit))]");
            }
            else 
            {
                sb.AppendLine($"        [FormComponent(typeof(TextEdit))]");
            }

            if (!p.Name.EndsWith(CodeConst.SearchSuffix) && !(p.Name.EndsWith(CodeConst.IdSuffix) || p.Name.EndsWith(CodeConst.CodeSuffix)))
            {
                sb.AppendLine($"        [Display(\"{p.Name.Replace(CodeConst.ModelSuffix, "").SplitWords()}\")]");

                if (isColumn)
                {
                    sb.AppendLine($"        public object {prop.GetPath().Replace(".", "")} => TableColumn(t => t.{prop.Parent.GetPath()}, c => c.{prop.GetPath()});");
                }
                else
                {
                    sb.AppendLine($"        public object {prop.GetPath().Replace(".", "")} => ModelProp(c => c.{prop.GetPath()});");
                }
            }
        }

        public class RefListDetails
        {
            public ModelProperty List { get; set; }
            public string Id { get; set; }
            public string Value { get; set; }
        }

        public static RefListDetails FindRefList(ModelProperty prop, List<ModelProperty> props)
        {
            var result = new RefListDetails();
            var words = prop.Property.Name.SplitWords().Split(' ');
            var refs = props.Where(p => p.IsList && p.Property.Name.EndsWith(CodeConst.RefSuffix)).Select(p => new { List = p, Count = p.Property.Name.SplitWords().Split(' ').Intersect(words).Count() });
            
            if (!refs.Any())
            {
                return null;
            }

            var max = refs.Max(r => r.Count);

            if (max == 0)
            {
                return null;
            }

            result.List = refs.FirstOrDefault(r => r.Count == max)?.List;
            var lps = TypeHelper.GetPublicVirtualProperties(result.List.Property.PropertyType.GetGenericArguments()[0]);
            
            result.Id = lps.FirstOrDefault(p => p.Property.Name.EndsWith(CodeConst.IdSuffix) || p.Property.Name.EndsWith(CodeConst.CodeSuffix) 
                                                || p.Property.Name.EndsWith(CodeConst.ShortNameSuffix))?.Property.Name;

            if (result.Id == null && lps.Any())
            {
                result.Id = lps[0].Property.Name;
            }

            result.Value = lps.LastOrDefault(p => p.Property.Name == CodeConst.Name || p.Property.Name == CodeConst.DisplayName
                                                || p.Property.Name.Contains(CodeConst.Value))?.Property.Name;

            if ((result.Value == null || result.Value == result.Id) && lps.Any())
            {
                result.Value = lps.Last().Property.Name;
            }

            return result;
        }
    }
}
