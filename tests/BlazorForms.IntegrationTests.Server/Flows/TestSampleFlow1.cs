using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared.DataStructures;

namespace BlazorForms.Integration.Tests.Server.Flows
{
    public class TestSampleFlow1 : FluentFlowBase<TestSampleModel1>
    {
        public override void Define()
        {
            this
                .Begin(() => FlowStart())
                .Next(() => LoadData())
                .NextForm(typeof(TestSampleForm1))
                .Next(() => RefreshFormData())
                .NextForm(typeof(TestSampleForm2))
                .Next(() => SaveData())
                .End(() => FlowEnd());
        }

        public async Task FlowStart()
        {
        }

        public async Task LoadData()
        {
            Model.Name = "DeleteMe";
            Model.Amount = new Money(100m, "Tanga");
        }

        public async Task RefreshFormData()
        {
            Model.Name = "SecondForm";
        }
        public async Task SaveData()
        {
        }

        public async Task FlowEnd()
        {
            Model.Name = "FlowEnd";
        }
    }

    public class TestSampleModel1 : FlowModelBase
    {
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Money Amount { get; set; }
    }

    public class TestSampleForm1 : FormEditBase<TestSampleModel1>
    {
        protected override void Define(FormEntityTypeBuilder<TestSampleModel1> f)
        {
            f.Property(p => p.Id).IsPrimaryKey();
            f.Property(p => p.Name).IsRequired();
            f.Property(e => e.Amount).Control(typeof(MoneyEdit));

            f.Button("/", ButtonActionTypes.Submit);
            f.Button("/", ButtonActionTypes.Cancel, "Cancel Me");
        }
    }

    public class TestSampleForm2 : FormEditBase<TestSampleModel1>
    {
        protected override void Define(FormEntityTypeBuilder<TestSampleModel1> f)
        {
            f.Property(p => p.Id).IsReadOnly();
            f.Property(p => p.Name).IsRequired().IsReadOnly();
            f.Property(e => e.Amount).Control(typeof(MoneyEdit));

            f.Button("/", ButtonActionTypes.Submit);
            f.Button("/", ButtonActionTypes.Cancel, "Cancel Me");
        }
    }
}
