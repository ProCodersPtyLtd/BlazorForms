using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BlazorForms.Forms
{
    public interface IFormDefinition
    {
        string Name { get; }
        string ProcessTaskTypeFullName { get; }
        void SetModel(object model);
    }

    public interface IFormDefinition<TSource> : IFormDefinition
      where TSource : class
    {
        TSource Model { get; set; }
    }
}
