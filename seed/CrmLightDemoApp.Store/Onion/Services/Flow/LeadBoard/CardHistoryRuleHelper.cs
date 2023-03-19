using BlazorForms.FlowRules;
using BlazorForms.Forms;
using BlazorForms.Shared;
using CrmLightDemoApp.Store.Onion.Domain.Repositories;
using CrmLightDemoApp.Store.Onion.Services.Abstractions;
using CrmLightDemoApp.Store.Onion.Services.Model;

namespace CrmLightDemoApp.Store.Onion.Services.Flow.LeadBoard
{
    public static class CardHistoryRuleHelper
    {
        public static void RefreshButtons(LeadBoardCardModel model, FlowRuleAsyncBase<LeadBoardCardModel> rule, RuleExecutionResult Result, 
            IAppAuthState _appAuthState)
        {
            // display buttons only for comment owners
            for (int i = 0; i < model.CardHistory.Count; i++)
            {
                var isCurrentUser = _appAuthState.GetCurrentUser().Id == model.CardHistory[i].PersonId;
                Result.Fields[rule.FindField(m => m.CardHistory, ModelBinding.EditButtonBinding, i)].Visible = isCurrentUser;
                Result.Fields[rule.FindField(m => m.CardHistory, ModelBinding.DeleteButtonBinding, i)].Visible = isCurrentUser;
            }
        }
    }
    
}
