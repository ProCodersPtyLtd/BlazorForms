using BlazorForms.Flows.Definitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public abstract class FluentFlowBase<M> : IFluentFlow<M> where M : class, IFlowModel, new()
    {
        private ITaskExecutor _executionEngine;

        public List<TaskDef> Tasks { get; set; }

        public virtual M Model { get; set; }
        public virtual FlowParamsGeneric Params { get; set; }

        public bool FirstPass { get; private set; }

        protected string _refId;
        protected IFlowContext _flowContext;
        public ReadOnlyCollection<TaskExecutionValidationResult> TaskExecutionValidationIssues => throw new NotImplementedException();

        public FlowSettings Settings { get; private set; } = new FlowSettings { StoreModel = FlowExecutionStoreModel.FullNoHistory };

        public FluentFlowBase()
        {
            Model = new M();
            Params = new FlowParamsGeneric();
            Initialize(Settings);
        }

        public virtual Type GetModelType()
        {
            return typeof(M);
        }

        public virtual void Initialize(FlowSettings settings)
        { }

        public virtual IFlowContext CreateContext()
        {
            var context = new FlowContext
            {
                Model = Model,
                Params = Params,
                RefId = CreateRefId(),
            };
            return context;
        }

        public virtual IFlowModel GetModel()
        {
            return Model;
        }

        public virtual void SetModel(IFlowModel model)
        {
            Model = model as M;
        }

        public void SetParams(FlowParamsGeneric p)
        {
            Params = p as FlowParamsGeneric;
        }

        public abstract void Define();

        public List<TaskDef> Parse()
        {
            // Clear Tasks list
            Tasks = new List<TaskDef>();

            // 1st pass - create task list
            Define();

            // 2nd pass - goto indexes
            var labels = Tasks.Select((s, i) => new { s, i }).Where(t => t.s.Type == TaskDefTypes.Label).ToDictionary(x => x.s.Name, x => x.i);
            Tasks.Where(t => t.Type == TaskDefTypes.Goto || t.Type == TaskDefTypes.GotoIf).ToList().ForEach(t => t.GotoIndex = labels[t.Name]);

            // 3rd pass - if-else-endif indexes
            Stack<StackType> stack = new Stack<StackType>();

            for (int i = 0; i < Tasks.Count; i++)
            {
                var task = Tasks[i];

                if (task.Type == TaskDefTypes.If)
                {
                    stack.Push(new StackType { If = true, Index = i });
                }
                else if (task.Type == TaskDefTypes.Else)
                {
                    var st = stack.Pop();

                    if (st.If == false)
                    {
                        throw new Exception("Else must be used after If only");
                    }

                    int ifIndex = st.Index;
                    Tasks[ifIndex].GotoIndex = i + 1;
                    stack.Push(new StackType { If = false, Index = i });
                }
                else if (task.Type == TaskDefTypes.EndIf)
                {
                    var st = stack.Pop();

                    if (st.If == true)
                    {
                        // Else is missed - goto directly after EndIf
                        int ifIndex = st.Index;
                        Tasks[ifIndex].GotoIndex = i + 1;
                    }
                    else
                    {
                        // set Else goto index
                        int elseIndex = st.Index;
                        Tasks[elseIndex].GotoIndex = i + 1;
                    }
                }
            }

            for (int i = 0; i < Tasks.Count; i++)
            {
                Tasks[i].Index = i;
            }

            return Tasks;
        }

        public virtual async Task UserView(Type formType, Func<int, int, Task<M>> callback)
        {
            await _executionEngine.CallViewDataCallback(formType, callback.Method.Name);
        }

        // ToDo: all these NotImplementedException's break Liskov SOLID principle
        public Task ExecuteFlow()
        {
            throw new NotImplementedException();
        }

        public Task CallTask(Type taskType)
        {
            throw new NotImplementedException();
        }

        public Task UserInput(Type formType)
        {
            throw new NotImplementedException();
        }

        public Task UserInputReview(Type formType)
        {
            throw new NotImplementedException();
        }

        public void SetTaskExecutor(ITaskExecutor taskExecutor)
        {
            _executionEngine = taskExecutor;
        }

        public IFlowContext GetCurrentContext()
        {
            return _executionEngine.CurrentContext;
        }

        private class StackType
        {
            public bool If;
            public int Index;
        }

        public virtual void SetFlowRefId(string refId)
        {
            _refId = refId;
        }

        public virtual void SetFirstPass(bool firstPass)
        {
            FirstPass = firstPass;
        }

        public virtual void SetFlowContext(IFlowContext flowContext)
        {
            _flowContext = flowContext;
        }

        public virtual void UpdateCurrentContext(string statusMessage, string assignedUser, string adminUser, string assignedTeam)
        {
            _flowContext.StatusMessage = statusMessage;
            _flowContext.AssignedUser = assignedUser;
            _flowContext.AdminUser = adminUser;
            _flowContext.AssignedTeam = assignedTeam;
        }

        public virtual string CreateRefId()
        {
            return Guid.NewGuid().ToString();
        }
    }

}
