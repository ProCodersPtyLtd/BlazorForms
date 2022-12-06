using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public class FluentFlowDetails
    {
        public List<TaskDef> Tasks { get; set; }
    }

    public class TaskDef
    {
        public string Name { get; set; }
        public TaskDefTypes Type { get; set; }
        public Type FormType { get; set; }
        public string FormTypeName { get; set; }
        public Func<Task> Action { get; set; }
        public Action NonAsyncAction { get; set; }
        public Func<bool> Condition { get; set; }
        public int GotoIndex { get; set; }
        public string CallbackTask { get; set; }
        public bool PreloadTableData { get; set; }
   }

    public enum TaskDefTypes
    {
        Begin,
        End,
        Task,
        Form,
        Label,
        Goto,
        GotoIf,
        If,
        Else,
        EndIf,
        Wait = 51
    }
}
