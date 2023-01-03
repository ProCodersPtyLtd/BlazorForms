using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Model
{
    public class BoardColumn
    {
		public string Name { get; set; }
	}
    
    public class BoardCard
	{
        public string FlowRunId { get; set; }       
        public string Selector { get; set; }  
        public object Model { get; set; }  

        public string Title { get; set; }       
	}
}
