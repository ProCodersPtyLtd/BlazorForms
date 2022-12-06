using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.CodeGeneration
{
    public class EditFlowGenerator : IModelCentricCodeGenerator
    {
        public ModelCentricGenerationResult GetCode(ModelCentricGenerationParams p)
        {
            var result = new ModelCentricGenerationResult();
            var modelName = p.ModelType.Name;
            var className = p.ModelType.Name.Replace(CodeConst.ModelSuffix, CodeConst.EditFlowSuffix);
            result.FileName = className;
            var formName = $"{CodeConst.FormPrefix}{p.ModelType.Name.Replace(CodeConst.ModelSuffix, "")}";

            var sb = new StringBuilder();
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
using BlazorForms.Platform.Shared.Attributes;");

            sb.AppendLine($"using {p.ModelType.Namespace};");
            sb.AppendLine();
            sb.AppendLine($"namespace {p.ModelType.Namespace.ReplaceEnd(CodeConst.ModelNamespaceEnd, "")}");
            sb.AppendLine("{");
            sb.AppendLine($"    [Flow(nameof({className}))]");
            sb.AppendLine($"    public class {className} : FluentFlowBase<{modelName}>");
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
            sb.AppendLine("        public override void Define()");
            sb.AppendLine("        {");
            sb.AppendLine("            this");
            sb.AppendLine("                .Begin()");
            sb.AppendLine("                .Next(LoadData)");
            sb.AppendLine($"                .NextForm(typeof({formName}))");
            sb.AppendLine("                .Next(SaveAsync)");
            sb.AppendLine("                .End();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private async Task LoadData()");
            sb.AppendLine("        {");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private async Task SaveAsync()");
            sb.AppendLine("        {");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            result.Code = sb.ToString();
            return result;
        }
    }
}
