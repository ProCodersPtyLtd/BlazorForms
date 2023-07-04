using BlazorForms.Flows.Definitions;
using BlazorForms.Storage;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class CardHistoryModel : IEntity, IFlowModel
    {
        // entity
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int BoardCardId { get; set; }

        public DateTime Date { get; set; }
        public DateTime? EditedDate { get; set; }
        public string Title { get; set; }
        public int PersonId { get; set; }
        public string? Text { get; set; }

        // refactor
        public string? PersonFullName { get; set; }

        // Model
        public string AvatarMarkup { get { return null; } }

        public string TitleMarkup
        {
            get
            {
                var suff = EditedDate != null ? $"<em>edited on {EditedDate.Value.ToString("dd/MM/yyyy HH:mm")}</em>" : null;
                return $"<b>{PersonFullName}</b> on {Date.ToString("dd/MM/yyyy HH:mm")} {suff}";
            }
        }

        public string TextMarkup { get { return Text; } }
    }
}
