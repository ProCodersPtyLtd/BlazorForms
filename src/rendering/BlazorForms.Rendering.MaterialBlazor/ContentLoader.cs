using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.MaterialBlazor
{
    public class ContentLoader
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        public ContentLoader(IJSRuntime jsRuntime)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/BlazorForms.Rendering.MaterialBlazor/Components/MatBlazorComponent.razor.js").AsTask());
        }

        public async Task LoadContentAsync()
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("includeJsCss");
        }
    }
}
