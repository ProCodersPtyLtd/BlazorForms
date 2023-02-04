using CrmLightDemoApp.Onion.Domain.Entities;

namespace CrmLightDemoApp.Onion.Domain
{
	public class UserDetails : User
	{
		public virtual string? PersonFullName { get; set; }
	}
}
