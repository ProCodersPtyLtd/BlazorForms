using BlazorForms.Flows;
using BlazorForms.Flows.Engine.Fluent;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using CrmLightDemoApp.Store.Onion.Domain.Repositories;
using CrmLightDemoApp.Store.Onion.Services.Model;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CrmLightDemoApp.Store.Onion.Services.Flow.Admin
{
    public class UserListFlow : ListFlowBase<UserListModel, FormUserList>
    {
        private readonly IUserRepository _userRepository;

        public UserListFlow(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        public override async Task<UserListModel> LoadDataAsync(QueryOptions queryOptions)
        {
            using var ctx = _userRepository.GetAllDetailsContextQuery();

            if (!string.IsNullOrWhiteSpace(queryOptions.SearchString))
            {
                ctx.Query = ctx.Query.Where(x => x.PersonFullName.Contains(queryOptions.SearchString, StringComparison.OrdinalIgnoreCase) 
                        || (x.Login != null && x.Login.Contains(queryOptions.SearchString, StringComparison.OrdinalIgnoreCase)) );
            }

            if (queryOptions.AllowSort && !string.IsNullOrWhiteSpace(queryOptions.SortColumn) && queryOptions.SortDirection != SortDirection.None)
            {
                ctx.Query = ctx.Query.QueryOrderByDirection(queryOptions.SortDirection, queryOptions.SortColumn);
            }
                
            var list = (await _userRepository.RunAllDetailsContextQueryAsync(ctx)).Select(x =>
            {
                var item = new UserModel();
                x.ReflectionCopyTo(item);
                return item;
            }).ToList();

            var result = new UserListModel { Data = list };
            return result;
        }
    }

    public class FormUserList : FormListBase<UserListModel>
    {
        protected override void Define(FormListBuilder<UserListModel> builder)
        {
            builder.List(p => p.Data, e =>
            {
                e.DisplayName = "Users";

                e.Property(p => p.Id).IsPrimaryKey();
                e.Property(p => p.PersonFullName).Label("User name");
                e.Property(p => p.Login);

                e.ContextButton("View", "user-edit/{0}");
                e.NavigationButton("Add", "user-edit/0");
            });
        }
    }
}
