using BlazorForms.Rendering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Interfaces
{
	public interface IFlowBoardViewModel
	{
		List<FlowBoardColumn> Columns { get; }
		List<FlowBoardCard> Cards { get; }

		Task LoadAsync(Type flowType);
		Task RefreshCardsAsync(List<FlowBoardCard> cards);
		bool IsTransitionPossible(FlowBoardCard card, string column);
		Task PerformTransition(FlowBoardCard card, string newState);
		Task ReorderCards(FlowBoardCard card, int newOrder);

	}
}
