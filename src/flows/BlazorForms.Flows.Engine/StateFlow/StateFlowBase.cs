﻿using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine.StateFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public interface IStateFlow : IFlow
    {
        List<StateDef> States { get; }
        List<TransitionDef> Transitions { get; }
        void Define();
        void Parse();
        void SetFlowContext(IFlowContext context);
        string AssignedUser { get; set; }
        string AssignedRole { get; set; }
    }

    public abstract class StateFlowBase : IStateFlow
    {
        public virtual string AssignedUser
        {
            get
            {
                return Context.AssignedUser;
            }
            set
            {
                Context.AssignedUser = value;
            }
        }

        public virtual string AssignedRole
        {
            get
            {
                return Context.AssignedTeam;
            }
            set
            {
                Context.AssignedTeam = value;
            }
        }

        public status Status
        {
            get
            {
                return new status(Context.StatusMessage);
            }
            set
            {
                Context.StatusMessage = value.Value;
            }
        }

        public List<StateDef> States { get; protected set; } = new List<StateDef>();
        public List<TransitionDef> Transitions { get; protected set; } = new List<TransitionDef>();
        public IFlowContext Context { get; set; }
        public virtual FlowParamsGeneric Params { get; set; }

        public FlowSettings Settings { get; private set; } = new FlowSettings { StoreModel = FlowExecutionStoreModel.NoStoreTillStop };

        public StateFlowBase()
        {
            PrepopulateFlowObjects();
        }

        public abstract void Define();

        private void PrepopulateFlowObjects()
        {
            var objectFields = GetType().GetFields().Where(f => typeof(StateFlowObject).IsAssignableFrom(f.FieldType));

            foreach (var field in objectFields)
            {
                if (field.GetValue(this) == null)
                {
                    // initialize flow object fields values by field names
                    var obj = Activator.CreateInstance(field.FieldType, field.Name) as StateFlowObject;
                    field.SetValue(this, obj);
                }
            }
        }

        public void Parse()
        {
            // just populate States and Transitions
            Define();
        }

        public abstract IFlowModel GetModel();

        public abstract void SetModel(IFlowModel model);

        public void SetParams(FlowParamsGeneric p)
        {
            Params = p;
        }

        public abstract IFlowContext CreateContext();

        public IFlowContext GetCurrentContext()
        {
            return Context;
        }

        public void SetFlowContext(IFlowContext context)
        {
            Context = context;
        }
        public virtual string CreateRefId()
        {
            return Guid.NewGuid().ToString();
        }

    }

    public abstract class StateFlowBase<M> : StateFlowBase
        where M : class, IFlowModel
    {
        public virtual M Model { get; set; }

        protected SpanTransitionTrigger DaySpanTrigger(int days)
        {
            return new SpanTransitionTrigger(new DaySpan(days));
        }

        protected SpanFromChangeTransitionTrigger DaySpanFromChangeTrigger(int days)
        {
            return new SpanFromChangeTransitionTrigger(new DaySpan(days));
        }

        public override IFlowContext CreateContext()
        {
            var context = new FlowContext
            {
                Model = Model,
                Params = Params,
                RefId = CreateRefId(),
            };
            return context;
        }

        public override IFlowModel GetModel()
        {
            return Model;
        }

        public override void SetModel(IFlowModel model)
        {
            Model = model as M;
        }
    }

    // aux types
    public class StateDef
    {
        public string State { get; set; }
        public bool IsEnd { get; internal set; }
    }

    public class TransitionDef
    {
        public string FromState { get; set; }
        public string ToState { get; set; }
        public TransitionTrigger Trigger { get; set; }
        public Func<TransitionTrigger> TriggerFunction { get; set; }
        public Action OnChanging { get; set; }

        public TransitionTrigger GetTrigger()
        {
            return Trigger ?? TriggerFunction();
        }

        public bool IsButtonTrigger()
        {
            var trigger = GetTrigger();
            return trigger is ButtonTransitionTrigger;
        }
    }
}
