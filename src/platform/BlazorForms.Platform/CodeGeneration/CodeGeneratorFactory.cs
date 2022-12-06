using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.CodeGeneration
{
    public static class CodeGeneratorFactory
    {
        public static IEnumerable<IModelCentricCodeGenerator> GetGeneratorsByType(Type t)
        {
            var result = new List<IModelCentricCodeGenerator>();

            if (t.Name.EndsWith(CodeConst.ListModelSuffix))
            {
                result.Add(new ListFlowGenerator());
            }
            else if (t.Name.EndsWith(CodeConst.ModelSuffix))
            {
                result.Add(new EditFlowGenerator());
                result.Add(new FormGenerator());
            }

            return result;
        }
    }
}
