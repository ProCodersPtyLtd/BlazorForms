using BlazorForms.Forms;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow.LeadBoard
{
    public class FormLeadCardEdit : FormEditBase<LeadBoardCardModel>
    {
        protected override void Define(FormEntityTypeBuilder<LeadBoardCardModel> f)
        {
            f.DisplayName = "Lead Card";

            f.Group("left");
            f.Property(p => p.State).IsReadOnly();
            MainSection(f);

        }

        public static void MainSection(FormEntityTypeBuilder<LeadBoardCardModel> f)
        {
            f.Confirm(ConfirmType.ChangesWillBeLost, "If you leave before saving, your changes will be lost.", ConfirmButtons.OkCancel);
            f.Layout = FormLayout.TwoColumns;
            f.Group("left");

            f.Rule(typeof(FormLeadCard_RefreshSources), FormRuleTriggers.Loaded);
            f.Property(p => p.Title).IsRequired();
            f.Property(p => p.Description);

            f.Property(p => p.SalesPersonId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Sales person").IsRequired()
                .NewItemDialog(typeof(PersonDialogFlow));

            f.Property(p => p.LeadSourceTypeId).Dropdown(p => p.AllLeadSources, m => m.Id, m => m.Name).Label("Lead source");

            f.Property(p => p.RelatedPersonId).DropdownSearch(p => p.AllPersons, m => m.Id, m => m.FullName).Label("Lead Contact")
                .NewItemDialog(typeof(PersonDialogFlow));

            f.Property(p => p.RelatedCompanyId).DropdownSearch(p => p.AllCompanies, m => m.Id, m => m.Name).Label("Company")
                .NewItemDialog(typeof(CompanyDialogFlow));

            f.Property(p => p.Phone);
            f.Property(p => p.Email);
            f.Property(p => p.ContactDetails).Label("Other contact info");

            f.Group("right");
            f.Property(p => p.Comments).Control(ControlType.TextArea);

            f.List(p => p.CardHistory, e =>
            {
                e.DisplayName = "History";
                e.Card(p => p.TitleMarkup, p => p.TextMarkup, p => p.AvatarMarkup);
            });

            f.Button(ButtonActionTypes.Cancel, "Cancel");
            f.Button(ButtonActionTypes.Submit, "Save");
        }
    }
}
