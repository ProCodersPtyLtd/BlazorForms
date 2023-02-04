using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class UserModel : UserDetails
    {
        public static UserModel FromDetails(UserDetails val)
        {
            var model = new UserModel();
            val.ReflectionCopyTo(model);
            return model;
        }
    }
}
