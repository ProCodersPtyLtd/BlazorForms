using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Engine.Fluent;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Repositories;
using CrmLightDemoApp.Onion.Infrastructure;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow
{
    public class PersonCompanyLinkTypeEditFlow : FluentFlowBase<PersonCompanyLinkTypeListModel>
    {
        private readonly IPersonCompanyLinkTypeRepository _repository;

        public PersonCompanyLinkTypeEditFlow(IPersonCompanyLinkTypeRepository repository)
        {
            _repository = repository;
        }

        public override void Define()
        {
            this
                .Begin(LoadData)
                .NextForm(typeof(FormPersonCompanyLinkTypeEdit))
                .Next(SaveData)
                .NextForm(typeof(FormPersonCompanyLinkTypeSaved))
                .End();
        }

        public async Task LoadData()
        {
            var items = await _repository.GetAllAsync();
            
            Model.Data = items.Select(x =>
            {
                var item = new PersonCompanyLinkTypeModel();
                x.ReflectionCopyTo(item);
                return item;
            }).ToList();
        }

        public async Task SaveData()
        {
            foreach(var item in Model.Deleted)
            {
                if (item.Id != 0)
                {
                    await _repository.SoftDeleteAsync(item.Id);
                }
            }

            foreach (var item in Model.Data)
            {
                if (item.Id == 0)
                {
                    await _repository.CreateAsync(item);
                }
                else if (item.Changed)
                {
                    await _repository.UpdateAsync(item);
                }
            }
        }
    }

    public class FormPersonCompanyLinkTypeEdit : FormEditBase<PersonCompanyLinkTypeListModel>
    {
        protected override void Define(FormEntityTypeBuilder<PersonCompanyLinkTypeListModel> f)
        {
            f.DisplayName = "Person Company Link Type";

            f.Repeater(p => p.Data, e =>
            {
                e.Property(p => p.Id).IsReadOnly().Label("Id")
                    .Rule(typeof(PersonCompanyLinkType_ItemDeletingRule), FormRuleTriggers.ItemDeleting);

                e.Property(p => p.Name).IsRequired().IsUnique().Label("Name")
                    .Rule(typeof(PersonCompanyLinkType_ItemChangedRule), FormRuleTriggers.ItemChanged);
            });

            f.Button(ButtonActionTypes.Close);
            f.Button(ButtonActionTypes.Submit, "Save Changes");
        }
    }

    public class PersonCompanyLinkType_ItemDeletingRule : FlowRuleBase<PersonCompanyLinkTypeListModel>
    {
        public override string RuleCode => "PCLT-1";

        public override void Execute(PersonCompanyLinkTypeListModel model)
        {
            // preserve all deleted items
            model.Deleted.Add(model.Data[RunParams.RowIndex]);
        }
    }

    public class PersonCompanyLinkType_ItemChangedRule : FlowRuleBase<PersonCompanyLinkTypeListModel>
    {
        public override string RuleCode => "PCLT-2";

        public override void Execute(PersonCompanyLinkTypeListModel model)
        {
            model.Data[RunParams.RowIndex].Changed = true;
        }
    }

    public class FormPersonCompanyLinkTypeSaved : FormEditBase<PersonCompanyLinkTypeListModel>
    {
        protected override void Define(FormEntityTypeBuilder<PersonCompanyLinkTypeListModel> f)
        {
            f.DisplayName = "Person Company Link Type changes saved successfully";
            f.Button(ButtonActionTypes.Close);
        }
    }

    //public class PersonCompanyLinkTypeListFlow : ListFlowBase<PersonCompanyLinkTypeListModel, FormPersonCompanyLinkType>
    //{
    //    private readonly IPersonCompanyLinkTypeRepository _repository;

        //    public PersonCompanyLinkTypeListFlow(IPersonCompanyLinkTypeRepository repository)
        //    {
        //        _repository = repository;
        //    }

        //    public override async Task<PersonCompanyLinkTypeListModel> LoadDataAsync(QueryOptions queryOptions)
        //    {
        //        var list = await _repository.GetAllAsync();
        //        var result = new PersonListModel { Data = list };
        //        return result;
        //    }
        //}

        //public class FormPersonCompanyLinkType : FormListBase<PersonListModel>
        //{
        //    protected override void Define(FormListBuilder<PersonListModel> builder)
        //    {
        //        builder.List(p => p.Data, e =>
        //        {
        //            e.DisplayName = "People";

        //            e.Property(p => p.Id).IsPrimaryKey();
        //            e.Property(p => p.FirstName).Label("First Name").Filter(FieldFilterType.TextStarts);
        //            e.Property(p => p.LastName).Label("Last Name").Filter(FieldFilterType.TextStarts);
        //            e.Property(p => p.BirthDate).Label("Date of birth").Format("dd/MM/yyyy");
        //            e.Property(p => p.Phone);
        //            e.Property(p => p.Email);

        //            e.ContextButton("Details", "person-edit/{0}");
        //            //e.ContextButton("Edit", typeof(PersonEditFlow), FlowReferenceOperation.Edit);
        //            //e.ContextButton("Delete", "person-delete/{0}");

        //            e.NavigationButton("Add", "person-edit/0");
        //        });
        //    }
        //}
}
