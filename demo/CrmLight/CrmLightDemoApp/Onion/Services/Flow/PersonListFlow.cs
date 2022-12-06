using BlazorForms.Flows;
using BlazorForms.Flows.Engine.Fluent;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow
{
    public class PersonListFlow : ListFlowBase<PersonListModel, FormPersonList>
    {
        private readonly IPersonRepository _personRepository;

        public PersonListFlow(IPersonRepository personRepository) 
        {
            _personRepository = personRepository;
        }

        public override async Task<PersonListModel> LoadDataAsync(QueryOptions queryOptions)
        {
            var list = await _personRepository.GetAllWithContactsAsync();
            var result = new PersonListModel { Data = list };
            return result;
        }
    }

    public class FormPersonList : FormListBase<PersonListModel>
    {
        protected override void Define(FormListBuilder<PersonListModel> builder)
        {
            builder.List(p => p.Data, e =>
            {
                e.DisplayName = "People";

                e.Property(p => p.Id).IsPrimaryKey();
                e.Property(p => p.FirstName).Label("First Name").Filter(FieldFilterType.TextStarts);
                e.Property(p => p.LastName).Label("Last Name").Filter(FieldFilterType.TextStarts);
                e.Property(p => p.BirthDate).Format("dd/MM/yyyy");
                e.Property(p => p.Contact.Phone);
                e.Property(p => p.Contact.Email);

                e.ContextButton("Details", "person-details/{0}");
                e.ContextButton("Edit", typeof(PersonEditFlow), FlowReferenceOperation.Edit);
                e.ContextButton("Delete", "person-delete/{0}");

                e.NavigationButton("Add", typeof(PersonEditFlow), FlowReferenceOperation.DialogForm);
            });
        }
    }
}
