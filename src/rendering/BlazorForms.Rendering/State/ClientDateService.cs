using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace BlazorForms.Rendering
{
    public class ClientDateService : IClientDateService
    {

        private IClientBrowserService _clientBrowserService;
        private DateTimeFormatInfo _dateFormat = CultureInfo.CurrentCulture.DateTimeFormat;
        private TimeZoneInfo _timeZoneInfo = default;

        public ClientDateService(IClientBrowserService clientBrowserService)
        {
            _clientBrowserService = clientBrowserService;
        }

        public async ValueTask<DateTime?> GetLocalDateTime(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return dateTime;
            }
            var browserLanguage = await _clientBrowserService.GetBrowserLanguage();
            var culture = CultureInfo.GetCultureInfoByIetfLanguageTag(browserLanguage);
            _dateFormat = culture != null ? culture.DateTimeFormat : _dateFormat;
            _timeZoneInfo = await _clientBrowserService.GetLocalDateTimezone();

            if (dateTime != null && dateTime.Value.Kind == DateTimeKind.Utc)
            {
                dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime.Value, _timeZoneInfo);
            }            

            return dateTime;
        }
    }
}