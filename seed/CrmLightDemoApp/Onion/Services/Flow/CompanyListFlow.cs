using BlazorForms.Flows;
using BlazorForms.Flows.Engine.Fluent;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using CrmLightDemoApp.Onion.Domain.Repositories;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow
{
    public class CompanyListFlow : ListFlowBase<CompanyListModel, FormCompanyList>
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyListFlow(ICompanyRepository companyRepository) 
        {
            _companyRepository = companyRepository;
        }

        public override async Task<CompanyListModel> LoadDataAsync(QueryOptions queryOptions)
        {
            var list = (await _companyRepository.GetAllAsync()).Select(x =>
            {
                var item = new CompanyModel();
                x.ReflectionCopyTo(item);
                return item;
            }).ToList();

            var result = new CompanyListModel { Data = list };
            return result;
        }
    }

    public class FormCompanyList : FormListBase<CompanyListModel>
    {
        protected override void Define(FormListBuilder<CompanyListModel> builder)
        {
            builder.List(p => p.Data, e =>
            {
                e.DisplayName = "Companies";

                e.Property(p => p.Id).IsPrimaryKey();
                e.Property(p => p.Name);
                e.Property(p => p.RegistrationNumber).Label("Reg. No.");
                e.Property(p => p.EstablishedDate).Label("Established date").Format("dd/MM/yyyy");

                e.ContextButton("Details", "company-edit/{0}");
                e.NavigationButton("Add", "company-edit/0");
            });
        }
    }
}
