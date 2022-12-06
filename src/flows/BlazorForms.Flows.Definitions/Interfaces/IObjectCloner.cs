using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public interface IObjectCloner
    {
        Task<T> CloneObject<T>(T source);
    }
}
