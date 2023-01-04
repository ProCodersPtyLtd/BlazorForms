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


        public List<BoardColumn> Columns {get;set;}

		public List<BoardCard> Cards { get; set; }
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
			Columns = _flowDetails.States.Select(s => new BoardColumn { Id = s.State, Name = s.Caption }).ToList();
		}

		public async Task RefreshCardsAsync(List<BoardCard> cards)
		{
			Cards = new List<BoardCard>();
			cards.ForEach(async c => Cards.Add(await GetLoadedCard(c)));

			//Cards = cards.Select(c => GetLoadedCard(c)).OrderBy(c => c.Order).ToList();
		}

		private async Task<BoardCard> GetLoadedCard(BoardCard c)
		{
			var r = c.ReflectionGetCopy();
			r.Context = await _flowRunEngine.CreateFlowContext(_currentFlowType, r.Selector);

			if (r.Title?.Length > 20)
			{
				r.Title = $"{r.Title.Substring(0, 20)}...";
			}

			return r;
		}

		public bool IsTransitionPossible(BoardCard card, string column)
		{
			if (card.Selector == column)
			{
				return true;
			}

			var transitions = _flowDetails.Transitions.Where(x => x.FromState == card.Selector && x.IsUserActionTrigger());
			var result = transitions.Any(x => x.ToState == column);
            return result;
		}

		public async Task PerformTransition(BoardCard card, string action)
		{
			if (IsStorageEnabled)
			{
				await _flowRunEngine.ContinueFlow(card.RefId, action);
			}
			else
			{
				card.Context = await _flowRunEngine.ContinueFlowNoStorage(card.Context, action);
				card.Selector = card.Context.CurrentTask;
			}
		}
	}
}
