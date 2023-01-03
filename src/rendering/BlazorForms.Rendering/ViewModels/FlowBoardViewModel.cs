using BlazorForms.Flows.Definitions;
using BlazorForms.Rendering.Interfaces;
using BlazorForms.Rendering.Model;
using BlazorForms.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.ViewModels
{
	public class FlowBoardViewModel : IFlowBoardViewModel
	{
		private readonly IStateFlowRunEngine _flowRunEngine;
		private Type _currentFlowType;

		public List<BoardColumn> Columns {get;set;}

		public List<BoardCard> Cards { get; set; }
		public bool IsStorageEnabled { get; set; }

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

			var flowDetails = await _flowRunEngine.GetStateDetails(ps);

			Columns = flowDetails.States.Select(s => new BoardColumn { Name = s.State }).ToList();
		}

		public async Task RefreshCardsAsync(List<BoardCard> cards)
		{
			Cards = cards.Select(c => GetLoadedCard(c)).ToList();
		}

		private BoardCard GetLoadedCard(BoardCard c)
		{
			var r = c.ReflectionGetCopy();

			if (r.Title?.Length > 20)
			{
				r.Title = $"{r.Title.Substring(0, 20)}...";
			}

			return r;
		}
	}
}
