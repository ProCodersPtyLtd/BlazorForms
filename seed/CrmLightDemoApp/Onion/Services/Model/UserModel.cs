using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain;

namespace CrmLightDemoApp.Onion.Services.Model
{
    public class UserModel : UserDetails
    {
        public static UserModel FromUserDetails(UserDetails val)
        {
            var model = new UserModel();
            val.ReflectionCopyTo(model);
            return model;
        }
    }
}
