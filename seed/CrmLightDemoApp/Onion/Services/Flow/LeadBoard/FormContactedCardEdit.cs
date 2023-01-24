using BlazorForms.Forms;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow.LeadBoard
{
    public class FormContactedCardEdit : FormEditBase<LeadBoardCardModel>
    {
        protected override void Define(FormEntityTypeBuilder<LeadBoardCardModel> f)
        {
            f.DisplayName = "Lead Contacted Card";
            f.Group("left");

            f.Property(p => p.State).IsReadOnly();
            f.Property(p => p.FollowUpDate).Label("Follow up date");
            f.Property(p => p.FollowUpDetails).Label("Follow up details");
            FormLeadCardEdit.MainSection(f);
        }
    }
}
