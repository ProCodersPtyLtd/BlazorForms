using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using BlazorForms.FlowRules;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms.Platform.ProcessFlow;

namespace BlazorForms.Platform.Crm.Business.Artel
{
    [Form("Artel Project Dashboard")]
    public class FormArtelProjectDashboard : FlowTaskDefinitionBase<ArtelProjectDashboardModel>
    {
        private const string PersonalGroup = "Personal Statistics";

        [FormComponent(typeof(Label))]
        [Display("Project")]
        public object NameControl => ModelProp(m => m.Project.Name);

        [FormComponent(typeof(MoneyEdit))]
        [Display("Project Total Value", Disabled = true)]
        public object TotalValueControl => ModelProp(m => m.Project.Statistics.TotalValue);

        [FormComponent(typeof(TextEdit))]
        [Display("Project Total Shares Issued", Disabled = true)]
        public object TotalSharesControl => ModelProp(m => m.Project.Statistics.TotalSharesIssued);

        [FormComponent(typeof(MoneyEdit))]
        [Display("Project Share Price", Disabled = true)]
        public object SharePriceControl => ModelProp(m => m.Project.InitialSharePrice);

        [FormComponent(typeof(TextEdit))]
        [Display("Total time submitted in timesheets", Disabled = true)]
        public object HoursControl => ModelProp(m => m.TotalTimeSubmitted);

        [FormComponent(typeof(MoneyEdit))]
        [Display("Project Balance", Disabled = true)]
        public object BalanceControl => ModelProp(m => m.Project.Statistics.CurrentBalance);

        [FlowRule(typeof(ProjectSettingsRule), FormRuleTriggers.Changed)]
        [FormComponent(typeof(Button))]
        [Display("Project Settings")]
        public object ProjectSettings => ActionButton("$.ActionButtons.ProjectSettings", ActionType.Custom);

        // Team table
        [FlowRule(typeof(MemberSelectedRule), FormRuleTriggers.Changed)]
        [FormComponent(typeof(Table))]
        //[FormComponent(typeof(Repeater))]
        [Display("Project Team")]
        public IFieldBinding Members => Table(t => t.Members);
        //public IFieldBinding Members => Repeater(t => t.Members, new RepeaterParameters(){ IsFixedList = true});

        [Display("Id", Visible = false, IsPrimaryKey = true)]
        public object KeyId => TableColumn(t => t.Members, c => c.EntityId);

        [FormComponent(typeof(Label))]
        [Display("Name")]
        public IFieldBinding MemberControl => TableColumn(t => t.Members, m => m.FullName);

        [FormComponent(typeof(TextEdit))]
        [Display("Email", Disabled = true)]
        public IFieldBinding EmailControl => TableColumn(t => t.Members, m => m.Email);

        [FormComponent(typeof(TextEdit))]
        [Display("Role", Disabled = true)]
        public IFieldBinding RoleControl => TableColumn(t => t.Members, m => m.AssignedRole);

        [FormComponent(typeof(Checkbox))]
        [Display("Is Admin", Disabled = true)]
        public IFieldBinding AdminControl => TableColumn(t => t.Members, m => m.IsAdmin);

        [FlowRule(typeof(TeamSettingsRule), FormRuleTriggers.Changed)]
        [FormComponent(typeof(Button))]
        [Display("Team Settings")]
        public object TeamSettings => ActionButton("$.ActionButtons.TeamSettings", ActionType.Custom);

        [FormComponent(typeof(Label))]
        [Display("Personal Statistics")]
        public IFieldBinding PersonalTitleControl => ModelProp(m => m.PersonalStatName);

        [FormComponent(typeof(TextEdit))]
        [Display("Contribution effort submitted", Disabled = true)]
        public IFieldBinding EffortControl => ModelProp(m => m.PersonalStatEffortSubmitted);

        [FormComponent(typeof(TextEdit))]
        [Display("Total shares balance", Disabled = true)]
        public IFieldBinding PersonalSharesControl => ModelProp(m => m.PersonalStatShares);

        [FormComponent(typeof(MoneyEdit))]
        [Display("Total salary paid up to date", Disabled = true)]
        public IFieldBinding PersonalSalaryControl => ModelProp(m => m.PersonalStatTotalPaid);

        [FormComponent(typeof(MoneyEdit))]
        [Display("Project account outstanding balance", Disabled = true)]
        public IFieldBinding PersonalBalanceControl => ModelProp(m => m.PersonalStatOutstandingBalance);
    }

    public class ProjectSettingsRule : FlowRuleAsyncBase<ArtelProjectDashboardModel>
    {
        public override string RuleCode => "APD-BTN-1";
        private readonly NavigationManager _navigation;

        public ProjectSettingsRule(NavigationManager navigation)
        {
            _navigation = navigation;
        }

        public override async Task Execute(ArtelProjectDashboardModel model)
        {
            //string path = $"start-flow-form-generic/{typeof(ArtelProjectSettingsFlow).FullName}/{model.Project.Id}";
            string path = $"start-flow-form-generic/{typeof(ArtelProjectSettingsFlow).FullName}/{155}";
            _navigation.NavigateTo(path, true);
        }
    }

    public class MemberSelectedRule : FlowRuleAsyncBase<ArtelProjectDashboardModel>
    {
        public override string RuleCode => "APD-TBL-1";

        public override async Task Execute(ArtelProjectDashboardModel model)
        {
            model.SelectedMemberIndex = RunParams.RowIndex;
        }
    }

    public class TeamSettingsRule : FlowRuleAsyncBase<ArtelProjectDashboardModel>
    {
        public override string RuleCode => "APD-BTN-2";
        private readonly NavigationManager _navigation;

        public TeamSettingsRule(NavigationManager navigation)
        {
            _navigation = navigation;
        }

        public override async Task Execute(ArtelProjectDashboardModel model)
        {
            //string path = $"artel-project-team/{typeof(ArtelTeamListFlow).FullName}/{model.Project.Id}";
            string path = $"artel-project-team/{model.Project.Id}";
            _navigation.NavigateTo(path, true);
        }
    }
}
