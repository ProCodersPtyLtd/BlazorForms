using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions.Abstractions;

public interface IFormRulesCollection<TModel> 
    where TModel : IFlowModel
{
    IEnumerable<Func<IFlowModel, Task<bool>>> Rules { get; }
}