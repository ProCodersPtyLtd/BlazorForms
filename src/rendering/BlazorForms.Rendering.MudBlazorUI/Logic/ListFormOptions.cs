using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.MudBlazorUI.Components
{
    public class ListFormOptions : FormOptions
    {
        public DataOptimization DataOptimization { get; set; }
        //public bool Virtualize { get; set; }
        public bool ShowSearch { get; set; }
        public string? ToolBarCaption { get; set; }
        //public bool ShowFilters { get; set; }
        public bool ShowSorting { get; set; }
        //public bool ShowPagination { get; set; }
        public int PageSize { get; set; } = 10;
        public int[]? PageSizeOptions { get; set; } = new int[] { 10, 50, 100 };

        public bool UseReloadServerData()
        {
            return DataOptimization == DataOptimization.ServerPagination || ShowSearch || ShowSorting;
        }
    }

    public enum DataOptimization
    {
        Virtualization,
        //VirtualPagination, // Virtual pagination is not working
        ServerPagination,
        None,
    }
}
