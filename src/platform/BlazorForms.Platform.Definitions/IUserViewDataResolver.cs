using System;
using System.Collections.Generic;
using System.Text;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;

namespace BlazorForms.Platform
{
    public interface IUserViewDataResolver
    {
        string[,] ResolveData(FormDetails formDetails, IFlowModel model, ILogStreamer logStreamer);
        string[,] ResolveData(string tableName, IEnumerable<FieldControlDetails> columns, IFlowModel model);
    }
}
