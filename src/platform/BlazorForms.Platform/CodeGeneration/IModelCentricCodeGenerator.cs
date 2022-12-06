using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Platform.CodeGeneration
{
    public class ModelCentricGenerationParams
    {
        public Type ModelType { get; set; }
    }
    public class ModelCentricGenerationResult
    {
        public string Code { get; set; }
        public string FileName { get; set; }
    }

    public interface IModelCentricCodeGenerator
    {
        ModelCentricGenerationResult GetCode(ModelCentricGenerationParams p);
    }
}
