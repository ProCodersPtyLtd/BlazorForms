using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Forms
{
    public class CustomComponent<T> : IFormComponent
      where T : class, IFormComponent
    {
        public string GetFullName()
        {
            throw new NotImplementedException();
        }
    }
}
