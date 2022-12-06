using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine;
using BlazorForms.Forms;
using BlazorForms.ItemStore;
using BlazorForms.Proxyma;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform;
using BlazorForms.Platform.ProcessFlow;
using BlazorForms.Platform.Shared.ApplicationParts;
using BlazorForms.Platform.Shared.Interfaces;
using BlazorForms.Platform.Stubs;
using BlazorForms.Rendering;
using BlazorForms.Rendering.Interfaces;
using BlazorForms.Rendering.State;
using BlazorForms.Rendering.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;

namespace BlazorForms
{
    public static class BlazorFormsServerServiceApplicationBuilderExtensions
    {
        public static IApplicationBuilder BlazorFormsRun(this IApplicationBuilder builder)
        {
            ValidateRuleCreation(builder);
            ValidateFlowCreation(builder);
            return builder;
        }

        private static void ValidateRuleCreation(IApplicationBuilder builder)
        {
            var scope = builder.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            // RuleEngine creates all rules in constructor
            var engine = services.GetService<IRuleExecutionEngine>();
        }

        private static void ValidateFlowCreation(IApplicationBuilder builder)
        {
            var scope = builder.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var engine = services.GetService<IFluentFlowRunEngine>();
            var provider = services.GetService<IFlowRunProvider>();
            var flows = engine.GetAllFlowTypes();

            foreach(var flow in flows)
            {
                provider.ValidateFlow(flow);
            }
        }
    }
}
