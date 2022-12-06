using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Shared;
using BlazorForms.Platform;
using BlazorForms.Platform.Definitions.Shared;
using BlazorForms.Rendering.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering
{
    public class RenderingViewModel : IRenderingViewModel
    {
        protected string? _baseUri;

        protected readonly IFlowRunProvider _flowRunProvider;

        public string? ExceptionMessage { get; private set; }
        public string? ExceptionStackTrace { get; private set; }
        public string? ExceptionType { get; private set; }

        public RenderingViewModel(IFlowRunProvider flowRunProvider)
        {
            _flowRunProvider = flowRunProvider;
        }

        public async Task SaveException()
        {
            var flowParams = new FlowParamsGeneric();
            flowParams.Operation = FlowReferenceOperation.QuickAction;
            flowParams[PlatformConstants.BaseUri] = _baseUri;
            flowParams["Type"] = ExceptionType;
            flowParams["Message"] = ExceptionMessage;
            flowParams["StackTrace"] = ExceptionStackTrace;

            await RunActionFlow("BlazorForms.Platform.ErrorEditFlow", flowParams, false);
            ClearException(false);
        }

        public void PopulateException([NotNullAttribute] Exception exc)
        {
            ExceptionMessage = exc.Message;
            ExceptionStackTrace = exc.StackTrace;
            ExceptionType = exc.GetType().FullName;
        }

        public void PopulateException(IFlowContextNoModel context)
        {
            ExceptionMessage = context.ExecutionResult.ExceptionMessage;
            ExceptionStackTrace = context.ExecutionResult.ExceptionStackTrace;
            ExceptionType = context.ExecutionResult.ExceptionType;
        }

        public void SetBaseUri(string uri)
        {
            _baseUri = uri;
        }

        private void ClearException(bool clearAll)
        {
            ExceptionType = null;

            if (clearAll)
            {
                ExceptionMessage = null;
                ExceptionStackTrace = null;
            }
        }

        public async Task RunActionFlow(string flowType, FlowParamsGeneric parameters, bool showResult = true)
        {
            parameters[PlatformConstants.BaseUri] = _baseUri;

            await Task.Run(async () =>
            {
                var context = await _flowRunProvider.ExecuteClientKeptContextFlow(new ClientKeptContext { FlowName = flowType }, null, parameters);

                // ToDo: Showing exception flow form is not implemented, do we need it?
                if (showResult)
                {
                    PopulateException(context);
                }
            });
        }
    }
}
