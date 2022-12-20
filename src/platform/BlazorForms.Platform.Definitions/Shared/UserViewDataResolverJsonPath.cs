using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Definitions.Shared
{
    public class UserViewDataResolverJsonPath : IUserViewDataResolver
    {
        public string[,] ResolveData(FormDetails formDetails, IFlowModel model, ILogStreamer logStreamer)
        {
            throw new NotImplementedException();
        }

        public string[,] ResolveData(string tableName, IEnumerable<FieldControlDetails> columns, IFlowModel model)
        {
            throw new NotImplementedException();
        }
    }
}
