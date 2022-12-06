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
    [Flow(nameof(ArtelProjectListFlow))]
    public class ArtelProjectListFlow : FluentFlowBase<ArtelProjectListModel>
    {
        private readonly IArtelProjectService _service;
        private readonly ILogger _logger;
        private ILogStreamer _logStreamer;

        public ArtelProjectListFlow(ILogger<ArtelProjectListFlow> logger, ILogStreamer logStreamer, IArtelProjectService projectService)
        {
            _logger = logger;
            _service = projectService;
            _logStreamer = logStreamer;
        }

        public override void Define()
        {
            this.ListForm(typeof(ArtelProjectListFlowForm), ViewDataCallbackTask);
        }

        [ResolveTableData]
        public async Task<ArtelProjectListModel> ViewDataCallbackTask(QueryOptions queryOptions)
        {
            var result = new ArtelProjectListModel();

            try
            {
                result.Projects = await _service.GetProjects(queryOptions);
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
    public class ArtelProjectListFlowForm : FlowTaskDefinitionBase<ArtelProjectListModel>
    {

        [Display("Id", Visible = false, IsPrimaryKey = true)]
        public object Id => TableColumn(t => t.Projects, c => c.Id);

        [Display("Project Name")]
        public object CompanyName => TableColumn(t => t.Projects, c => c.Name);

        [Display("Members")]
        public object UsersCount => TableColumn(t => t.Projects, c => c.Statistics.MemberCount);

        [Display("Total Value")]
        public object TotalValue => TableColumn(t => t.Projects, c => c.Statistics.TotalValue.DefaultFormatted);


        public object Menu => TableColumnContextMenu(t => t.Projects
            , new BindingFlowNavigationReference("View", $"start-flow-form-generic/{typeof(ArtelProjectDashboardFlow).FullName}/{{0}}", FlowReferenceOperation.Edit)
            //, new BindingFlowReference("View", typeof(ArtelProjectDashboardFlow))
            , new BindingFlowReference("Project Settings", typeof(ArtelProjectSettingsFlow))
            , new BindingFlowNavigationReference("Team Settings", $"artel-project-team/{{0}}", FlowReferenceOperation.Details)
            , new BindingFlowReference("Delete", typeof(ArtelProjectSettingsFlow), ArtelProjectSettingsFlow.Delete)
        );

        public object RefButtons => FlowReferenceButtons(
            new BindingFlowReference("Start new Artel Project", typeof(ArtelProjectSettingsFlow), FlowReferenceOperation.DialogForm)
            // , new BindingFlowNavigationReference("Start new Artel Project page", $"start-flow-form-generic/{typeof(ProjectListEditFlow).FullName}/0")
        );
    }

}
