using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Model
{
    public class BoardColumn
    {
		public string Id { get; set; }
		public string Name { get; set; }
	}
    
    public class BoardCard
	{
        public IFlowContext Context { get; set; }       
        public string RefId { get; set; }       
        public string Selector { get; set; }  
        //public state State { get; set; }  
        public object Model { get; set; }  

        public string Title { get; set; }       
        public string Details { get; set; }       
        public int Order { get; set; }       
	}
}
