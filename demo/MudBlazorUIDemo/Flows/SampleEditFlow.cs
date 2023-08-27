using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Forms.Definitions.FluentForms.Rules;

namespace MudBlazorUIDemo.Flows
{
    public class SampleEditFlow : FluentFlowBase<SampleModel>
    {
        public override void Define()
        {
            this
                .Begin()
                .Next(LoadData)
                .NextForm(typeof(FormSampleEdit))
                .Next(SaveAsync)
                .End();
        }

        private async Task LoadData()
        {
            Model.AllPersons = (new PersonModel[]
            {
                new PersonModel { Id=1, FirstName = "Adi", LastName = "Puma" },
                new PersonModel { Id=2, FirstName = "Gary", LastName = "Gansler" },
            }).ToList();

            Model.CommentHistory = (new CommentHistoryItem[]
            {
                new CommentHistoryItem { Id=1, FirstName = "Adi", LastName = "Puma", Date = DateTime.Now },
                new CommentHistoryItem { Id=2, FirstName = "Gary", LastName = "Gansler", Date = DateTime.Now.AddDays(1) },
                new CommentHistoryItem { Id=3, FirstName = "Bary", LastName = "London", Date = DateTime.Now.AddDays(1).AddHours(1) },
                new CommentHistoryItem { Id=4, FirstName = "Semen", LastName = "Petrov", Date = DateTime.Now.AddDays(1).AddHours(2) },
                new CommentHistoryItem { Id=5, FirstName = "Crazy", LastName = "Man", Date = DateTime.Now.AddDays(1).AddHours(12) },
            }).ToList();

            Model.AllPersons.ForEach(p => p.FullName = $"{p.FirstName} {p.LastName}");
        }

        private async Task SaveAsync()
        { }
    }

    public class FormSampleEdit : FormEditBase<SampleModel>
    {
        
        protected override void Define(FormEntityTypeBuilder<SampleModel> f)
        {
            f.DisplayName = "Sample layout form";
            f.Layout = FormLayout.TwoColumns;

            f.Group("1");
            f.Property(p => p.Title).Label("Title").IsRequired();
            f.Property(p => p.SelectedId).Dropdown(e => e.AllPersons, m => m.Id, m => m.FullName).IsRequired().Label("Person Select");
            f.Property(p => p.SearchedId).DropdownSearch(e => e.AllPersons, m => m.Id, m => m.FullName).IsRequired().Label("Person Search");
            f.Property(p => p.SelectedFullName).EditWithOptions(e => e.AllPersons, m => m.FullName).IsRequired().Label("Person Edit");

            f.Group("2");
            f.Property(p => p.Comment).Control(ControlType.TextArea);
            
            f.CardList(p => p.CommentHistory, e => 
            {
                e.DisplayName = "History";
                e.Card(p => p.TitleMarkup, p => p.TextMarkup);
            });

            f.Button(ButtonActionTypes.Cancel, "Cancel");
            f.Button(ButtonActionTypes.Submit, "Save");
        }

        public override IFormRule<SampleModel> RootRule()
        {
            return new SampleModelRulePackOne();
        }
    }
    
    public class SampleModelRulePackOne : FormRuleBase<SampleModel>
    {
        public override async Task<bool> Handle(SampleModel? model)
        {
            var doSomething = model?.AllPersons.Average(p => p.Id);
            return await base.Handle(model);
        }
    }

    public class SampleModel : FlowModelBase
    {
        public virtual string? Title { get; set; }
        public virtual int SelectedId { get; set; }
        public virtual int SearchedId { get; set; }
        public virtual string? SelectedFullName { get; set; }
        public virtual string? Comment { get; set; }
        public virtual List<PersonModel> AllPersons { get; set; }
        public virtual List<CommentHistoryItem> CommentHistory { get; set; } = new();
    }

    public class PersonModel
    {
        public virtual int Id { get; set; }
        public virtual string? FirstName { get; set; }
        public virtual string? LastName { get; set; }
        public virtual string? FullName { get; set; }
    }

    public class CommentHistoryItem
    {
        public virtual int Id { get; set; }
        public virtual string? FirstName { get; set; }
        public virtual string? LastName { get; set; }
        public virtual DateTime? Date { get; set; }

        public virtual string? TitleMarkup { get { return $"<p><b>{FirstName} {LastName}</b> at {Date?.ToString("dd/MM/yyyy hh:mm")}</p>"; } }
        public virtual string? TextMarkup { get { return "<p class='markup'>This is a <em>markup string</em>.</p>"; } }
    }
}
