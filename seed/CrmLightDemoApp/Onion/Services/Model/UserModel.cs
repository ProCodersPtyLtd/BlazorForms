using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;
using CrmLightDemoApp.Onion.Domain.Entities;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class UserModel : UserDetails, IFlowModel
    {
		public virtual List<PersonModel> AllPersons { get; set; }
		public virtual List<UserRoleLinkModel> CombinedUserRoles { get; set; }


		public static UserModel FromDetails(UserDetails val)
        {
            var model = new UserModel();
            val.ReflectionCopyTo(model);
            return model;
        }

        public List<string> GetGrantedRoles()
        {
            var roles = CombinedUserRoles.Where(x => x.Selected).Select(x => x.Name).ToList();
            return roles;
		}
    }
}
