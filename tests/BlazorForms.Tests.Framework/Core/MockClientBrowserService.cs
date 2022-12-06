using BlazorForms.Rendering;
using System;
using System.Threading.Tasks;

namespace BlazorForms.Tests.Framework.Core
{
    internal class MockClientBrowserService : IClientBrowserService
    {
        public ValueTask<string> GetBrowserLanguage()
        {
            throw new NotImplementedException();
        }

        public ValueTask<TimeSpan> GetLocalDateTimeOffset()
        {
            throw new NotImplementedException();
        }

        public ValueTask<TimeZoneInfo> GetLocalDateTimezone()
        {
            throw new NotImplementedException();
        }

        public ValueTask<string> GetWindowOrigin()
        {
            return new ValueTask<string>("https://localhost");
        }
    }
}