using System;
using System.Collections.Generic;
using BlazorForms.Shared;

namespace BlazorForms.Forms.Definitions.FluentForms.Validation;

public class RuleVirtualPropertyValidation
{
    public static void Validate<TEntity>(IEnumerable<DataField> fields)
    {
        foreach (var field in fields)
        {
            if (field.Rules.Count <= 0 || string.IsNullOrWhiteSpace(field.BindingProperty) || field.BindingProperty == ModelBinding.FormLevelBinding)
            {
                continue;
            }

            JsonPathHelper.Evaluate<TEntity>(field.BindingProperty, context =>
            {
                if (context.IsRoot || context.IsCurrent)
                {
                    return;
                }

                var propertyInfo = context.PropertyInfo!;
                    
                if (propertyInfo.GetMethod is null || !propertyInfo.GetMethod.IsPublic ||
                    propertyInfo.GetMethod.IsAbstract || !propertyInfo.GetMethod.IsVirtual)
                {
                    throw new NotSupportedException($"Property '{propertyInfo.Name}' of type '{propertyInfo.DeclaringType?.FullName}' referenced by selector {field.BindingProperty} must define a public virtual getter");
                }

                if (propertyInfo.SetMethod is null || !propertyInfo.SetMethod.IsPublic ||
                    propertyInfo.SetMethod.IsAbstract || !propertyInfo.SetMethod.IsVirtual)
                {
                    throw new NotSupportedException(
                        $"Property '{propertyInfo.Name}' of type '{propertyInfo.DeclaringType?.FullName}' referenced by selector {field.BindingProperty} must define a public virtual setter");
                }
            });
        }
    }
}