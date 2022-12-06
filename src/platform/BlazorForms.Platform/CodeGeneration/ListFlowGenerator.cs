using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;

namespace BlazorForms.Platform.CodeGeneration
{
    public class ListFlowGenerator : IModelCentricCodeGenerator
    {
        public ModelCentricGenerationResult GetCode(ModelCentricGenerationParams p)
        {
            var result = new ModelCentricGenerationResult();
            var modelName = p.ModelType.Name;
            var className = p.ModelType.Name.Replace(CodeConst.ListModelSuffix, CodeConst.ListFlowSuffix);
            result.FileName = className;

            var sb = new StringBuilder();
            sb.AppendLine(@"using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Logs;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Platform.Shared.Attributes;");

            sb.AppendLine($"using {p.ModelType.Namespace};");
            sb.AppendLine();
            sb.AppendLine($"namespace {p.ModelType.Namespace.ReplaceEnd(CodeConst.ModelNamespaceEnd, "")}");
            sb.AppendLine("{");
            sb.AppendLine($"    [Flow(nameof({className}))]");
            sb.AppendLine($"    public class {className} : FlowSingleTaskDefinitionBase<{modelName}, FlowParamsGeneric>");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly ILogger _logger;");
            sb.AppendLine("        private readonly ILogStreamer _logStreamer;");
            sb.AppendLine("        // private readonly IArtelService _service;");
            sb.AppendLine();
            sb.AppendLine($"        public {className}(ILogger<{className}> logger, ILogStreamer logStreamer");
            sb.AppendLine("            // , IArtelService service;");
            sb.AppendLine("            )");
            sb.AppendLine("        {");
            sb.AppendLine("            _logger = logger; ");
            sb.AppendLine("            _logStreamer = logStreamer; ");
            sb.AppendLine("            // _service = service; ");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public override async Task ExecuteFlow()");
            sb.AppendLine("        {");
            sb.AppendLine("            await UserView(ViewDataCallbackTask);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [ResolveTableData]");
            sb.AppendLine("        [Time]");
            sb.AppendLine($"        public async Task<{modelName}> ViewDataCallbackTask(queryOptions queryOptions)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = new {modelName}();");
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                // result.Projects = await _service.GetProjects(queryOptions queryOptions);");
            sb.AppendLine("                return result;");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception exc)");
            sb.AppendLine("            {");
            sb.AppendLine("                _logStreamer.TrackException(new Exception(\"ViewDataCallbackTask failed\", exc));");
            sb.AppendLine("                _logger.LogError(exc, \"ViewDataCallbackTask failed\");");
            sb.AppendLine("                throw exc;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");

            var props = TypeHelper.GetNestedPublicVirtualProperties(p.ModelType);
            var listProp = props.FirstOrDefault(m => m.IsList);
            var pkFound = false;

            foreach (var prop in props)
            {
                var propName = prop.Property.Name;

                if (propName == CodeConst.IdProperty || propName == CodeConst.EnitytIdProperty)
                {
                    // only one Primary Key supported
                    if (!pkFound)
                    {
                        pkFound = true;
                        sb.AppendLine();
                        sb.AppendLine($"        [Display(\"{propName}\", Visible = false, IsPrimaryKey = true)]");
                        sb.AppendLine($"        public object {propName} => TableColumn(t => t.{listProp.GetPath()}, c => c.{prop.GetPath()});");
                    }
                }
                else if (prop.Property.PropertyType.IsSimple())
                {
                    sb.AppendLine();
                    sb.AppendLine($"        [Display(\"{propName}\")]");
                    sb.AppendLine($"        public object {prop.GetPath().Replace(".", "")} => TableColumn(t => t.{listProp.GetPath()}, c => c.{prop.GetPath()});");
                }
            }

            var editClassName = p.ModelType.Name.Replace(CodeConst.ListModelSuffix, CodeConst.EditFlowSuffix);

            // Context menu
            sb.AppendLine();
            sb.AppendLine($"        public object Menu => TableColumnContextMenu(t => t.{listProp.GetPath()}");
            sb.AppendLine($"            , new BindingFlowNavigationReference(\"Edit Page\", $\"start-flow-form-generic/{{typeof({editClassName}).FullName}}/{{{{0}}}}\", FlowReferenceOperation.Edit)");
            sb.AppendLine($"            , new BindingFlowReference(\"Edit Dialog\", typeof({editClassName}), FlowReferenceOperation.Edit)");
            sb.AppendLine($"            , new BindingFlowReference(\"Delete\", typeof({editClassName}), FlowReferenceOperation.Delete)");
            sb.AppendLine($"            ");
            sb.AppendLine($"        );");

            // Add button
            sb.AppendLine();
            sb.AppendLine($"        public object RefButtons => FlowReferenceButtons(");
            sb.AppendLine($"            new BindingFlowReference(\"Start new dialog\", typeof({editClassName}), FlowReferenceOperation.DialogForm)");
            sb.AppendLine($"            , new BindingFlowNavigationReference(\"Start new page\", $\"start-flow-form-generic/{{typeof({editClassName}).FullName}}/0\")");
            sb.AppendLine($"        );");

            sb.AppendLine("    }");
            sb.AppendLine("}");

            result.Code = sb.ToString();
            return result;
        }
    }
}
