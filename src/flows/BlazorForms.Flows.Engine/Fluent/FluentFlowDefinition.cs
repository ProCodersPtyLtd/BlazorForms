using BlazorForms.Flows.Definitions;
using BlazorForms.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public static class FluentFlowDefinition
    {
        public static F Begin<F>(this F flow) where F : class, IFluentFlow { RegisterStart(flow.Tasks, null); return flow; }
        public static F Begin<F>(this F flow, Func<Task> action) where F : class, IFluentFlow { RegisterStart(flow.Tasks, action); return flow; }
        public static F End<F>(this F flow, Func<Task> action) where F : class, IFluentFlow { RegisterFinish(flow.Tasks, action); return flow; }
        public static F End<F>(this F flow) where F : class, IFluentFlow { RegisterFinish(flow.Tasks, null); return flow; }
        public static F Next<F>(this F flow, Func<Task> action) where F : class, IFluentFlow { RegisterTask(flow.Tasks, action.Method.Name, action); return flow; }
        public static F Next<F>(this F flow, Action action) where F : class, IFluentFlow { RegisterTask(flow.Tasks, action.Method.Name, action); return flow; }
        public static F NextForm<F>(this F flow, Type formType, Func<Task> action) where F : class, IFluentFlow { RegisterFormTask(flow.Tasks, formType.Name, formType, action); return flow; }
        
        public static F NextForm<F>(this F flow, Type formType) where F : class, IFluentFlow { RegisterFormTask(flow.Tasks, formType.Name, formType); return flow; }
        public static F NextForm<F>(this F flow, string formName) where F : class, IFluentFlow { RegisterFormTask(flow.Tasks, formName, formName); return flow; }
        
        public static void ListForm<M>(this IFluentFlow flow, Type formType, Func<QueryOptions , Task<M>> callBack) where M : class, IFlowModel 
        { 
            RegisterListFormTask(flow.Tasks, formType.Name, formType, callBack); 
        }

        public static void ListForm<M>(this IFluentFlow flow, Type formType, Func<QueryOptions , Task<M>> callBack, bool preloadTableData) where M : class, IFlowModel 
        { 
            RegisterListFormTask(flow.Tasks, formType.Name, formType, callBack, preloadTableData); 
        }

        public static F Goto<F>(this F flow, string label) where F : class, IFluentFlow { RegisterGoto(flow.Tasks, label); return flow; }
        public static F Label<F>(this F flow, string label) where F : class, IFluentFlow { RegisterLabel(flow.Tasks, label); return flow; }
        public static F GotoIf<F>(this F flow, string label, Func<bool> condition) where F : class, IFluentFlow { RegisterGotoIf(flow.Tasks, label, condition); return flow; }
        public static F If<F>(this F flow, Func<bool> condition) where F : class, IFluentFlow { RegisterIf(flow.Tasks, condition); return flow; }
        public static F Else<F>(this F flow) where F : class, IFluentFlow { RegisterTask(flow.Tasks, TaskDefTypes.Else, "Else", null); return flow; }
        public static F EndIf<F>(this F flow) where F : class, IFluentFlow { RegisterTask(flow.Tasks, TaskDefTypes.EndIf, "EndIf", null); return flow; }
        public static F Wait<F>(this F flow, Func<bool> condition) where F : class, IFluentFlow { RegisterWait(flow.Tasks, condition); return flow; }

        private static void RegisterTask(List<TaskDef> tasks, TaskDefTypes type, string name, Func<Task> action)
        {
            tasks.Add(new TaskDef { Action = action, Name = name, Type = type });
        }

        private static void RegisterWait(List<TaskDef> tasks, Func<bool> condition)
        {
            tasks.Add(new TaskDef { Name = "Wait", Type = TaskDefTypes.Wait, Condition = condition });
        }

        private static void RegisterIf(List<TaskDef> tasks, Func<bool> condition)
        {
            tasks.Add(new TaskDef { Name = "If", Type = TaskDefTypes.If, Condition = condition });
        }

        private static void RegisterGoto(List<TaskDef> tasks, string name)
        {
            // the second pass will update GotoIndex
            tasks.Add(new TaskDef { Name = name, Type = TaskDefTypes.Goto, GotoIndex = -1 });
        }

        private static void RegisterGotoIf(List<TaskDef> tasks, string name, Func<bool> condition)
        {
            // the second pass will update GotoIndex
            tasks.Add(new TaskDef { Name = name, Type = TaskDefTypes.GotoIf, GotoIndex = -1, Condition = condition });
        }

        private static void RegisterStart(List<TaskDef> tasks, Func<Task> action)
        {
            RegisterTask(tasks, TaskDefTypes.Begin, "Start", action);
        }

        private static void RegisterFinish(List<TaskDef> tasks, Func<Task> action)
        {
            RegisterTask(tasks, TaskDefTypes.End, "Finish", action);
        }

        private static void RegisterFormTask(List<TaskDef> tasks, string name, Type formType)
        {
            tasks.Add(new TaskDef { Name = name, Type = TaskDefTypes.Form, FormType = formType });
        }
        private static void RegisterFormTask(List<TaskDef> tasks, string name, string formTypeName)
        {
            tasks.Add(new TaskDef { Name = name, Type = TaskDefTypes.Form, FormTypeName = formTypeName });
        }

        private static void RegisterListFormTask<M>(List<TaskDef> tasks, string name, Type formType, Func<QueryOptions, Task<M>> callBack, bool preloadTableData = false) where M : class, IFlowModel
        {
            tasks.Add(new TaskDef { Name = name, Type = TaskDefTypes.Form, FormType = formType, CallbackTask = callBack.Method.Name, PreloadTableData = preloadTableData });
        }
        private static void RegisterListFormTask<M>(List<TaskDef> tasks, string name, Type formType, Func<dynamic, Task<M>> callBack) where M : class, IFlowModel
        {
            tasks.Add(new TaskDef { Name = name, Type = TaskDefTypes.Form, FormType = formType, CallbackTask = callBack.Method.Name });
        }

        private static void RegisterTask(List<TaskDef> tasks, string name, Func<Task> action)
        {
            RegisterTask(tasks, TaskDefTypes.Task, name, action);
        }

        private static void RegisterTask(List<TaskDef> tasks, string name, Action action)
        {
            tasks.Add(new TaskDef { NonAsyncAction = action, Name = name, Type = TaskDefTypes.Task });
        }
        
        private static void RegisterFormTask(List<TaskDef> tasks, string name, Type formType, Func<Task> action)
        {
            tasks.Add(new TaskDef { Name = name, Type = TaskDefTypes.Form, FormType = formType, Action = action });
        }

        private static void RegisterLabel(List<TaskDef> tasks, string name)
        {
            RegisterTask(tasks, TaskDefTypes.Label, name, null);
        }
    }
}
