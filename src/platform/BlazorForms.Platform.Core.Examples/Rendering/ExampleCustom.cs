using Pc.Framework.Libs.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pc.Platz
{
    public class ExampleCustom : IFormComponent
    {
        public string GetFullName()
        {
            // full name of class of your razor component
            // razor component namespace depends on it location
            return typeof(Pc.Platform.Core.Examples.Rendering.ExampleCustomComponent).FullName;
        }
    }
}
