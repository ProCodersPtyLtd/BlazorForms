using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared.Extensions;
using System.Collections.Generic;

namespace BlazorForms.Platform
{
    public class UserViewAccessInformation
    {
        public string AssignedUser { get; set; }
        public string AdminUser { get; set; }
        public string AssignedTeam { get; set; }
    }

    public interface IUserViewModel
    {
        string ModelFullName { get; set; }

        string RefId { get; set; }
        FormDetails UserViewDetails { get; set; }
        UserViewAccessInformation AccessInfo { get; set; }

        List<string> RawDataList { get; set; }
        int RawDataWidth { get; set; }
        QueryOptions QueryOptions { get; set; }

        IFlowModel GetModel();
        FlowParamsGeneric GetParams();
        void SetModel(IFlowModel model, FlowParamsGeneric flowParams);
    }

    public class UserViewModel<T> : IUserViewModel
        where T: class
    {
        public string ModelFullName { get; set; }

        public string RefId { get; set; }
        public FormDetails UserViewDetails { get; set; }
        public UserViewAccessInformation AccessInfo { get; set; }
        public List<string> RawDataList { get; set; }
        public int RawDataWidth { get; set; }
        public QueryOptions QueryOptions { get; set; }
        public T Model { get; set; }
        public FlowParamsGeneric Params { get; set; }

        public IFlowModel GetModel()
        {
            return Model as IFlowModel;
        }

        public FlowParamsGeneric GetParams()
        {
            return Params;
        }

        public void SetModel(IFlowModel model, FlowParamsGeneric flowParams)
        {
            Params = flowParams;
            Model = model as T;
            ModelFullName = Model?.GetType()?.FullName;
        }
    }
}
