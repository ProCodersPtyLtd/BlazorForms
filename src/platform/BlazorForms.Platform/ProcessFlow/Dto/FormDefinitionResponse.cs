using BlazorForms.Platform;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BlazorForms.Platform.ProcessFlow.Dto
{
    public class FormDefinitionResponse
    {
        public Collection<FieldDetails> Fields { get; set; }
        public Collection<FieldDisplayDetails> DisplayProperties { get; set; }
    }
}
