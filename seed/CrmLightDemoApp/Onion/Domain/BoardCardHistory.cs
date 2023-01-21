namespace CrmLightDemoApp.Onion.Domain
{
	public class BoardCardHistory : IEntity
	{
		public virtual int Id { get; set; }
		public virtual int BoardCardHistoryId { get; set; }
		public virtual bool Deleted { get; set; }

		public virtual DateTime Date { get; set; }
		public virtual string Title { get; set; }
		public virtual string? Text { get; set; }
    }
}
