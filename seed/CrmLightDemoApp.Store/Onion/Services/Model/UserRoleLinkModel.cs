using BlazorForms.Storage;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
	public class UserRoleLinkModel : IEntity
	{
        // UserRoleLink
        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual int UserId { get; set; }
        public virtual int RoleId { get; set; }

        public RoleModel Role { get; set; }
        public UserModel User { get; set; }

		// ToDo: temp properties
        public virtual string RoleName { get; set; }
		public virtual bool Selected { get; set; }

		public static List<UserRoleLinkModel> CombineFullList(int userId, IEnumerable<UserRoleLinkModel> links, IEnumerable<RoleModel> roles)
		{
			var result = roles.Select(r => new UserRoleLinkModel { RoleId = r.Id, RoleName = r.Name, UserId = userId }).ToList();

			result.ForEach(r =>
			{
				var link = links?.FirstOrDefault(l => l.RoleId == r.RoleId);

				if (link != null)
				{
					r.Selected = true;
					r.Id = link.Id;
				}
			});

			return result;
		}
	}
}
