using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Forms;
using BlazorForms.Shared;
using CrmLightDemoApp.Onion.Domain.Repositories;
using CrmLightDemoApp.Onion.Infrastructure;
using CrmLightDemoApp.Onion.Services.Model;

namespace CrmLightDemoApp.Onion.Services.Flow.Resolution
{
	public class ResolutionFlow : FluentFlowBase<ResolutionModel>
	{
		private const string CaseReview = "CaseReview";

		public override void Define()
		{
			this
				.Begin(LoadDataAsync)
				.NextForm(typeof(FormResolutionEntry))
				.Label(CaseReview)
				.Next(AssignArbitratorAsync)
				.NextForm(typeof(FormResolutionReview))
				.If(() => _flowContext.ExecutionResult.FormLastAction == ModelBinding.RejectButtonBinding)
					.NextForm(typeof(FormResolutionMoreDetails))
					.Goto(CaseReview)
				.EndIf()
				.Next(AssignApplicantAsync)
				.NextForm(typeof(FormResolutionAgree))
				.GotoIf(CaseReview, () => _flowContext.ExecutionResult.FormLastAction != ModelBinding.SubmitButtonBinding)
				.Next(TransferFundsAsync)
				.End();
		}

		private async Task LoadDataAsync()
		{ }
		private async Task AssignArbitratorAsync()
		{ }
		private async Task AssignApplicantAsync()
		{ }
		private async Task TransferFundsAsync()
		{ }
	}

	public class FormResolutionEntry : FormEditBase<ResolutionModel>
	{
		protected override void Define(FormEntityTypeBuilder<ResolutionModel> builder)
		{
		}
	}

	public class FormResolutionReview : FormEditBase<ResolutionModel>
	{
		protected override void Define(FormEntityTypeBuilder<ResolutionModel> builder)
		{
		}
	}
	public class FormResolutionMoreDetails : FormEditBase<ResolutionModel>
	{
		protected override void Define(FormEntityTypeBuilder<ResolutionModel> builder)
		{
		}
	}
	public class FormResolutionAgree : FormEditBase<ResolutionModel>
	{
		protected override void Define(FormEntityTypeBuilder<ResolutionModel> builder)
		{
		}
	}
}
