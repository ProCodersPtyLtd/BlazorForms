using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public interface IFormDefinitionParser
    {
        FormDetails Parse(Type formType);
    }
}
