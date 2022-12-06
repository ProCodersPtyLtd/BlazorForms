using System;
using System.Threading.Tasks;

namespace BlazorForms.Rendering
{
    public interface IClientDateService
    {
        ValueTask<DateTime?> GetLocalDateTime(DateTime? dateTime);
    }
}