using BlazorForms.Flows.Definitions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Model
{
    public class FlowBoardColumn
    {
		public string Id { get; set; }
		public string Name { get; set; }
	}
    
    public class FlowBoardCard
	{
        public IFlowContext Context { get; set; }       
        public string RefId { get; set; }       
        //public state State { get; set; }  
        public object ModelUntyped { get; set; }  

        public string State { get; set; }  
        public string Title { get; set; }       
        public string Details { get; set; }       
        public int Order { get; set; }       
	}

	public class FlowBoardCard<T> : FlowBoardCard
		where T: class
    {
        public T Model { get { return ModelUntyped as T; } set { ModelUntyped = value; } }
    }
}
