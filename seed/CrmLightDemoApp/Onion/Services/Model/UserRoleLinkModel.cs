using CrmLightDemoApp.Onion.Domain.Entities;

namespace CrmLightDemoApp.Onion.Services.Model
{
	public class UserRoleLinkModel : UserRoleLink
	{
		public virtual string Name { get; set; }
		public virtual bool Selected { get; set; }

		public static List<UserRoleLinkModel> CombineFullList(int userId, IEnumerable<UserRoleLink> links, IEnumerable<Role> roles)
		{
			var result = roles.Select(r => new UserRoleLinkModel { RoleId = r.Id, Name = r.Name, UserId = userId }).ToList();

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
