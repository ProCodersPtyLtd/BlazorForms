using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorFormsDemoFlows;

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

            Model.AllPersons.ForEach(p => p.FullName = $"{p.FirstName} {p.LastName}");
        }

        private async Task SaveAsync()
        { }
    }

    public class FormSampleEdit : FormEditBase<SampleModel>
    {
        protected override void Define(FormEntityTypeBuilder<SampleModel> f)
        {
            f.Property(p => p.Title).Label("Title").IsRequired();
            f.Property(p => p.SelectedId).Dropdown(e => e.AllPersons, m => m.Id, m => m.FullName).IsRequired().Label("Person Select");
            f.Property(p => p.SearchedId).DropdownSearch(e => e.AllPersons, m => m.Id, m => m.FullName).IsRequired().Label("Person Search");
            f.Property(p => p.SelectedFullName).EditWithOptions(e => e.AllPersons, m => m.FullName).IsRequired().Label("Person Edit");

            f.List(p => p.AllPersons, e => 
            {
                e.DisplayName = "History";
                e.Card(p => p.FirstName, p => p.LastName);
            });

            f.Button(ButtonActionTypes.Cancel, "Cancel");
            f.Button(ButtonActionTypes.Submit, "Save");
        }
    }

    public class SampleModel : FlowModelBase
    {
        public virtual string? Title { get; set; }
        public virtual int SelectedId { get; set; }
        public virtual int SearchedId { get; set; }
        public virtual string? SelectedFullName { get; set; }
        public virtual List<PersonModel> AllPersons { get; set; }
    }

    public class PersonModel
    {
        public virtual int Id { get; set; }
        public virtual string? FirstName { get; set; }
        public virtual string? LastName { get; set; }
        public virtual string? FullName { get; set; }
    }
}
