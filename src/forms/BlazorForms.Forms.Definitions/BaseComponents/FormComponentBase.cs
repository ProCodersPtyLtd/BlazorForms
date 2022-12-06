using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public class FormComponentBase : IFormComponent
    {
        public virtual string GetFullName()
        {
            throw new NotImplementedException();
        }
    }
}
