using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Storage;

namespace CrmLightDemoApp.Store.Onion.Services.Model
{
    public class UserModel : IEntity, IFlowModel
    {
        // UserDetails
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int TenantAccountId { get; set; }
        public int PersonId { get; set; }
        public string Login { get; set; }

        // FK
        public List<UserRoleLinkModel> RefUserRoleLink { get; } = new();

        public List<PersonModel> AllPersons { get; set; }
		public List<UserRoleLinkModel> CombinedUserRoles { get; set; }


		//public static UserModel FromDetails(UserDetails val)
  //      {
  //          var model = new UserModel();
  //          val.ReflectionCopyTo(model);
  //          return model;
  //      }

        public List<string> GetGrantedRoles()
        {
            var roles = CombinedUserRoles.Where(x => x.Selected).Select(x => x.RoleName).ToList();
            return roles;
		}
    }
}
