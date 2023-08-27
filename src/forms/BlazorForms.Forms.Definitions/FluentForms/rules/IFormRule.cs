#nullable enable
using System.Threading.Tasks;

namespace BlazorForms.Forms.Definitions.FluentForms.Rules;

public interface IFormRule<TModel>
    where TModel : class
{
    IFormRule<TModel>? SetNext(IFormRule<TModel>? rule);
    Task<bool> Handle(TModel? model);
}

public abstract class FormRuleBase<TModel>: IFormRule<TModel> where TModel : class
{
    private IFormRule<TModel>? _nextRule;

    public virtual async Task<bool> Handle(TModel? requestModel)
    {
        return _nextRule == null || await _nextRule.Handle(requestModel);
    }

    public IFormRule<TModel>? SetNext(IFormRule<TModel>? rule)
    {
        _nextRule = rule;
        return rule;
    }

}
