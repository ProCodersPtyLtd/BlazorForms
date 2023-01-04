namespace CrmLightDemoApp.Onion.Domain
{
	public class BoardCard : IEntity
	{
		public virtual int Id { get; set; }
		public virtual bool Deleted { get; set; }

		public virtual int BoardId { get; set; }
		public virtual string State { get; set; }
		public virtual string Title { get; set; }
		public virtual string? Description { get; set; }
		public virtual int Order { get; set; }

		public virtual string? Comments { get; set; }
		public virtual int? RelatedCompanyId { get; set; }
		public virtual int? RelatedPersonId { get; set; }
		public virtual string? Phone { get; set; }
		public virtual string? Email { get; set; }
		public virtual int? SalesPersonId { get; set; }
	}
}
