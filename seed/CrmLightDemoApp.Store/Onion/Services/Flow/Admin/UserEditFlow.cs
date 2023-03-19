using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using CrmLightDemoApp.Store.Onion.Domain.Repositories;
using CrmLightDemoApp.Store.Onion.Infrastructure;
using CrmLightDemoApp.Store.Onion.Services.Abstractions;
using CrmLightDemoApp.Store.Onion.Services.Flow.LeadBoard;
using CrmLightDemoApp.Store.Onion.Services.Model;

namespace CrmLightDemoApp.Store.Onion.Services.Flow.Admin
{
    public class UserEditFlow : FluentFlowBase<UserModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleLinkRepository _userRoleLinkRepository;
		private readonly INotificationService _notificationService;

		public UserEditFlow(IUserRepository userRepository, IPersonRepository personRepository, IRoleRepository roleRepository,
			IUserRoleLinkRepository userRoleLinkRepository, INotificationService notificationService)
        {
			_userRepository = userRepository;
            _personRepository = personRepository;
            _roleRepository = roleRepository;
            _userRoleLinkRepository = userRoleLinkRepository;
            _notificationService = notificationService;
		}

        public override void Define()
        {
            this
                .Begin()
                .Next(LoadDataAsync)
                .NextForm(typeof(FormUserEdit))
                .Next(SaveDataAsync)
                .End();
        }

        public async Task LoadDataAsync()
        {
            var roles = await _roleRepository.GetAllAsync();

            if (_flowContext.Params.ItemKeyAboveZero)
            {
                var item = await _userRepository.GetByIdAsync(_flowContext.Params.ItemKey);
                // item and Model have different types - we use reflection to copy similar properties
                item.ReflectionCopyTo(Model);
                int userId = _flowContext.Params.ItemKey;
                var links = await _userRoleLinkRepository.GetAllByUserIdAsync(userId);
				Model.CombinedUserRoles = UserRoleLinkModel.CombineFullList(userId, links, roles);
            }
            else
            {
				Model.CombinedUserRoles = UserRoleLinkModel.CombineFullList(0, null, roles);
			}

            var persons = (await _personRepository.GetAllAsync())
                .Select(x => 
                {
                    var item = new PersonModel(); 
                    x.ReflectionCopyTo(item);
                    item.FullName = $"{x.FirstName} {x.LastName}";
                    return item;
                }).OrderBy(x => x.FullName).ToList();

            Model.AllPersons = persons;
        }
 
        public async Task SaveDataAsync()
        {
            if (_flowContext.Params.ItemKeyAboveZero)
            {
                await _userRepository.UpdateAsync(Model);
            }
            else
            {
                Model.Id = await _userRepository.CreateAsync(Model);
            }

            foreach (var item in Model.CombinedUserRoles)
            {
                if (item.Id == 0 && item.Selected)
                {
                    item.UserId = Model.Id;
                    await _userRoleLinkRepository.CreateAsync(item);
                }
                else if (item.Id > 0 && !item.Selected)
                {
                    await _userRoleLinkRepository.SoftDeleteAsync(item);
                }
            }

			await _notificationService.PostMessageAsync(new MessageEventArgs { Type = MessageEventType.UserAccount });
		}
    }

    public class FormUserEdit : FormEditBase<UserModel>
    {
        protected override void Define(FormEntityTypeBuilder<UserModel> f)
        {

            f.DisplayName = "User Edit";
            f.Confirm(ConfirmType.ChangesWillBeLost, "If you leave before saving, your changes will be lost.", ConfirmButtons.OkCancel);

            f.Property(p => p.PersonId).DropdownSearch(e => e.AllPersons, m => m.Id, m => m.FullName).IsRequired().Label("Person")
				.ItemDialog(typeof(PersonDialogFlow));

            f.Property(p => p.Login).IsRequired();
            
            f.Repeater(p => p.CombinedUserRoles, e =>
            {
                e.DisplayName = "Security roles";
                e.Property(p => p.Id).IsReadOnly();
                e.Property(p => p.RoleName).IsReadOnly();
                e.Property(p => p.Selected).Control(ControlType.Checkbox).Label("Granted");
            });

            f.Button(ButtonActionTypes.Submit, "Save");
            f.Button(ButtonActionTypes.Cancel, "Cancel");
        }
    }
}
