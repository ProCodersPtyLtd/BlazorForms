﻿using BlazorForms.Rendering.MudBlazorUI.Components;
using MudBlazor;

namespace CrmLightDemoApp
{
    public static class GlobalSettings
    {
        public static EditFormOptions EditFormOptions = new EditFormOptions
        {
            MudBlazorProvidersDefined = true,
            Variant = Variant.Filled,
            DateFormat = "dd/MM/yyyy",
        };

        public static ListFormOptions ListFormOptions = new ListFormOptions
        {
            MudBlazorProvidersDefined = true,
            Variant = Variant.Filled,
            DateFormat = "dd/MM/yyyy",
        };
    }
}
