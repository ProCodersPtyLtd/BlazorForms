using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazorForms.Flows.Definitions;

namespace BlazorFormsDemoModels.Models
{
    public class NavigationModel1 : FlowModelBase
    {
        public string? WelcomeText { get; set; }
        public string? UserName { get; set; }
        public bool Continue { get; set; }
    }
}
