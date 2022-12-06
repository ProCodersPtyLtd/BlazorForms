using BlazorForms.Flows.Definitions;
using BlazorForms.Shared.Extensions;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows
{
    public static class FlowRunHelper
    {
        public static async Task<UserViewModelPageResult> ExecuteFlowTask(IServiceProvider serviceProvider, IFlowParser parser, string flowTypeName,
            IFlowContext context, string userViewCallbackTaskName, QueryOptions queryOptions, dynamic dynamicParams = null)
        {
            var flowType = parser.GetTypeByName(flowTypeName);
            var method = flowType.GetMethod(userViewCallbackTaskName);
            var parameters = TypeHelper.GetConstructorParameters(serviceProvider, flowType);
            IFlow flow = Activator.CreateInstance(flowType, parameters) as IFlow;
            flow.SetModel(context.Model);
            flow.SetParams(context.Params);
            IFlowModel resultModel = null;
            Task resultModelTask;

            if (TypeHelper.IsAsyncMethod(method))
            {
                if (method.GetParameters().Count() == 3)
                {
                    resultModelTask = method.Invoke(flow, new object[] { queryOptions, dynamicParams }) as Task;
                }
                else if (method.GetParameters().Any())
                {
                    resultModelTask = method.Invoke(flow, new object[] { queryOptions }) as Task;
                }
                else
                {
                    resultModelTask = method.Invoke(flow, null) as Task;
                }

                await resultModelTask;
                resultModel = ((dynamic)resultModelTask).Result as IFlowModel;
            }
            else
            {
                if (method.GetParameters().Count() == 3)
                {
                    resultModel = method.Invoke(flow, new object[] { queryOptions, dynamicParams }) as IFlowModel;
                }
                else if (method.GetParameters().Any())
                {
                    resultModel = method.Invoke(flow, new object[] { queryOptions }) as IFlowModel;
                }
                else
                {
                    resultModel = method.Invoke(flow, null) as IFlowModel;
                }
            }

            var result = new UserViewModelPageResult
            {
                Model = await Task.FromResult(resultModel),
                MethodTaskAttributes = method.GetCustomAttributes()
            };

            return result;
        }
    }
}
