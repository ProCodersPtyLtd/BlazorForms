using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Rendering.Interfaces;
using BlazorForms.Rendering.Model;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace BlazorForms.Rendering.ViewModels
{
	public class FlowBoardViewModel : IFlowBoardViewModel
	{
		private readonly IStateFlowRunEngine _flowRunEngine;
		private Type _currentFlowType;
		private StateFlowTaskDetails _flowDetails;


        public List<FlowBoardColumn> Columns {get;set;}

		public List<FlowBoardCard> Cards { get; set; }
		public bool IsStorageEnabled { get; set; } = false;

		public FlowBoardViewModel(IStateFlowRunEngine flowRunEngine)
		{
			_flowRunEngine = flowRunEngine;
		}

		public async Task LoadAsync(Type flowType)
		{
			_currentFlowType = flowType;

			var ps = new FlowRunParameters
			{
				FlowType = _currentFlowType,
				//Context = t.FlowContext,
				//RefId = t.RefId,
				NoStorageMode = !IsStorageEnabled
			};

			_flowDetails = await _flowRunEngine.GetStateDetails(ps);
			Columns = _flowDetails.States.Select(s => new FlowBoardColumn { Id = s.State, Name = s.Caption }).ToList();
		}

		public async Task RefreshCardsAsync(List<FlowBoardCard> cards)
		{
			Cards = new List<FlowBoardCard>();
			cards.ForEach(async c => Cards.Add(await GetLoadedCard(c)));

			//Cards = cards.Select(c => GetLoadedCard(c)).OrderBy(c => c.Order).ToList();
		}

		private async Task<FlowBoardCard> GetLoadedCard(FlowBoardCard c)
		{
			var r = c.ReflectionGetCopy();
			r.Context = await _flowRunEngine.CreateFlowContext(_currentFlowType, r.State);

			if (r.Title?.Length > 20)
			{
				r.Title = $"{r.Title.Substring(0, 20)}...";
			}

			if (r.Details?.Length > 32)
			{
				r.Details = $"{r.Details.Substring(0, 32)}...";
			}

			return r;
		}

		public bool IsTransitionPossible(FlowBoardCard card, string column)
		{
			if (card.State == column)
			{
				return true;
			}

			var transitions = _flowDetails.Transitions.Where(x => x.FromState == card.State && x.IsUserActionTrigger());
			var result = transitions.Any(x => x.ToState == column);
            return result;
		}

		public async Task PerformTransition(FlowBoardCard card, string action)
		{
			if (IsStorageEnabled)
			{
				await _flowRunEngine.ContinueFlow(card.RefId, action);
			}
			else
			{
				card.Context = await _flowRunEngine.ContinueFlowNoStorage(card.Context, action);
				card.State = card.Context.CurrentTask;
			}

			// ToDo: we need to save the changed card
		}

		public async Task ReorderCards(FlowBoardCard card, int newOrder)
		{
			int num2 = 0;
			foreach (var item2 in Cards.OrderBy(x => x.Order))
			{
				if (item2.Equals(card))
				{
					card.Order = newOrder;
					continue;
				}

				if (num2 == newOrder)
				{
					num2++;
				}

				item2.Order = num2;
				num2++;
			}

			// ToDo: we need to save the changed cards
		}
	}
}
