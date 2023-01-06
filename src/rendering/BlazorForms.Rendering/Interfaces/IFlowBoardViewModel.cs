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
		List<CardInfo<IFlowBoardCard>> Cards { get; }

		void SetItemsChangedCallback(Func<List<BoardCardChangedArgs<IFlowBoardCard>>, Task> callback);
		Task LoadAsync(Type flowType);
		Task RefreshCardsAsync(List<IFlowBoardCard> cards);
		bool IsTransitionPossible(CardInfo<IFlowBoardCard> card, string column);
		Task PerformTransition(CardInfo<IFlowBoardCard> card, string newState, Func<string, CardInfo<IFlowBoardCard>, Task<bool>> dialogCallback);
		Task ReorderCards(string state, CardInfo<IFlowBoardCard> card, int newOrder);

		string? GetStateForm(string state);
		List<FlowBoardContextMenuAction> GetContextMenuActions(CardInfo<IFlowBoardCard> card);
	}
}
