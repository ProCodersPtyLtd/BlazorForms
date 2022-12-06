using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Extensions;
using BlazorForms.Platform.Crm.Artel;
using BlazorForms.Platform.Crm.Definitions.Services;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Platform.Shared.Attributes;

namespace BlazorForms.Platform.Crm.Business.Artel
{
    [Flow(nameof(ArtelTeamListFlow))]
    public class ArtelTeamListFlow : FluentFlowBase<ArtelTeamListModel>
    {
        private readonly IArtelProjectService _service;
        private readonly ILogger _logger;
        private ILogStreamer _logStreamer;

        public ArtelTeamListFlow(IArtelProjectService projectService, ILogger<ArtelTeamListFlow> logger, ILogStreamer logStreamer)
        {
            _logger = logger;
            _service = projectService;
            _logStreamer = logStreamer;
        }

        public override void Define()
        {
            this.ListForm(typeof(ArtelTeamListFlowForm), ViewDataCallbackTask);
        }

        [ResolveTableData]
        public async Task<ArtelTeamListModel> ViewDataCallbackTask(QueryOptions queryOptions)
        {
            int projectId = Params.ItemKey;
            var result = new ArtelTeamListModel();

            try
            {
                result.Members = await _service.GetProjectMemberList(projectId, queryOptions);
                return result;
            }
            catch (Exception exc)
            {
                _logStreamer.TrackException(new Exception("ViewDataCallbackTask failed", exc));
                _logger.LogError(exc, "ViewDataCallbackTask failed");
                throw exc;
            }
        }
    }

    [Form("Projects", ChildProcess = typeof(ArtelProjectSettingsFlow))]
    public class ArtelTeamListFlowForm : FlowTaskDefinitionBase<ArtelTeamListModel>
    {
        [Display("Id", Visible = false, IsPrimaryKey = true)]
        public object Id => TableColumn(t => t.Members, c => c.EntityId);

        [Display("Name")]
        public object NameControl => TableColumn(t => t.Members, c => c.FullName);

        [Display("Email")]
        public object Email => TableColumn(t => t.Members, c => c.Email);

        [Display("Role")]
        public object Role => TableColumn(t => t.Members, c => c.AssignedRole);

        [Display("Is Admin")]
        public IFieldBinding AdminControl => TableColumn(t => t.Members, m => m.IsAdmin);


        public object Menu => TableColumnContextMenu(t => t.Members
            , new BindingFlowNavigationReference("Details", $"start-flow-form-generic/{typeof(ArtelMemberEditFlow).FullName}/{{0}}", FlowReferenceOperation.View)
            , new BindingFlowReference("Reset Password", typeof(ArtelMemberEditFlow))
            , new BindingFlowReference("Resend Invitation", typeof(ArtelMemberEditFlow))
            , new BindingFlowReference("Delete", typeof(ArtelProjectSettingsFlow), ArtelProjectSettingsFlow.Delete)
        );

        public object RefButtons => FlowReferenceButtons(
            new BindingFlowReference("Add Artel Team Member", typeof(ArtelMemberEditFlow), FlowReferenceOperation.DialogForm)
        );
    }

}
