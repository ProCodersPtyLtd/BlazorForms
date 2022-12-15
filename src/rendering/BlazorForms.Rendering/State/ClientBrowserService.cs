using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorForms.Platform.Definitions.Shared;

namespace BlazorForms.Rendering
{
    // ToDo: this is default implementation pointing to MaterialBlazor - you need to implement similar service in each new rendering lib
    public class ClientBrowserService : IClientBrowserService
    {
        //private readonly IJSRuntime _jsRuntime;
        private readonly NavigationManager _navigationManager;
        private TimeSpan? _userOffset;
        private TimeZoneInfo? _userTimeZone;
        private string? _windowOrigin;

        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        public ClientBrowserService(IJSRuntime jsRuntime, NavigationManager navigationManager)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/BlazorForms.Rendering.MaterialBlazor/timeZone.js").AsTask());

            //_jsRuntime = jsRuntime;
            _navigationManager = navigationManager;
        }

        public async ValueTask<TimeSpan> GetLocalDateTimeOffset()
        {
            if (_userOffset == null)
            {
                //int offsetInMinutes = await _jsRuntime.InvokeAsync<int>("blazorGetTimezoneOffset");
                var module = await _moduleTask.Value;
                int offsetInMinutes = await module.InvokeAsync<int>("blazorGetTimezoneOffset");
                _userOffset = TimeSpan.FromMinutes(-offsetInMinutes);
            }

            return _userOffset ?? default;
        }

        public async ValueTask<TimeZoneInfo> GetLocalDateTimezone()
        {
            if (_userTimeZone == null)
            {
                //var timezoneName = await _jsRuntime.InvokeAsync<string>("blazorGetTimezone");
                var module = await _moduleTask.Value;
                var timezoneName = await module.InvokeAsync<string>("blazorGetTimezone");
                // i.e. Australia/Sydney       
                var windowsTimeZone = TimeZoneFormats.OlsonToWindowsTimeZones[timezoneName];
                
                var tzi = TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZone);
                    
                return tzi;
            }

            return _userTimeZone ?? default;
        }

        public async ValueTask<string> GetBrowserLanguage()
        {
            try
            {
                //return await _jsRuntime.InvokeAsync<string>("blazorNavigatorLanguage");
                var module = await _moduleTask.Value;
                return await module.InvokeAsync<string>("blazorNavigatorLanguage");
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.ToString());
                return null;
            }
        }

        public async ValueTask<string> GetWindowOrigin()
        {
            try
            {
                return string.IsNullOrEmpty(_windowOrigin)
                    // ? (_windowOrigin = await _jsRuntime.InvokeAsync<string>("blazorWindowLocationOrigin"))
                    ? (_windowOrigin = _navigationManager.BaseUri)
                    : _windowOrigin;
            }
            catch (Exception ex)
            {
                await Console.Error.WriteLineAsync(ex.ToString());
                return null;
            }
        }
    }
}