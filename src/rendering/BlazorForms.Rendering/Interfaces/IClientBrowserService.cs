using System;
using System.Threading.Tasks;

namespace BlazorForms.Rendering
{
    public interface IClientBrowserService
    {
        ValueTask<string> GetBrowserLanguage();
        ValueTask<TimeSpan> GetLocalDateTimeOffset();
        ValueTask<TimeZoneInfo> GetLocalDateTimezone();
        ValueTask<string> GetWindowOrigin();
    }
}