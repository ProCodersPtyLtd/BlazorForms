using BlazorForms.Flows;
using CrmLightDemoApp.Onion.Domain.Repositories;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow.LeadBoard
{
    public class PersonDialogFlow : DialogFlowBase<PersonModel, FormPersonEdit>
    {
        private readonly IPersonRepository _personRepository;

        public PersonDialogFlow(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public override async Task LoadDataAsync()
        {
            var fullName = Params["Name"];

            if (fullName != null)
            {
                var split = fullName.Split(' ');
                Model.FirstName = split[0];

                if (split.Count() > 1)
                {
                    Model.LastName = split[1];
                }
            }
        }

        public override async Task SaveDataAsync()
        {
            // we need full name for drop down option
            Model.FullName = $"{Model.FirstName} {Model.LastName}";
            Model.Id = await _personRepository.CreateAsync(Model);
        }
    }
}
