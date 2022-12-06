using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Platform.Tests.FluentForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Platform.Tests.Forms
{
    public class QuizFormTests
    {
        [Fact]
        public void QuizParseTest()
        { }
    }

    public class Quiz1Form : FormEditBase<Quiz1Model>
    {
        protected override void Define(FormEntityTypeBuilder<Quiz1Model> f)
        {
            //f.Property(p => p.Id).IsReadOnly().Control(ControlType.Label);
            //f.Property(p => p.Client.Id).IsRequired().IsReadOnly();
            //f.Property(e => e.ClientId).Dropdown<TestClient>().Set(c => c.Id, c => c.Name).IsRequired().Label("Client").Rule(typeof(TestForm1NameChangedRule));

            f.Button("CustAddrCountList", ButtonActionTypes.Submit);
            f.Button("CustAddrCountList", ButtonActionTypes.Cancel, "Cancel Me");
        }
    }

    public class Quiz1Model : FlowModelBase
    {
        public virtual string? MainQuestion { get; set; }
        public virtual byte[] MainImage { get; set; }
        public virtual List<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
        public virtual int SelectedAnswerIndex { get; set; }
    }

    public class QuizAnswer
    {
        public virtual string? Text { get; set; }
        public virtual byte[] Image { get; set; }
    }
}
