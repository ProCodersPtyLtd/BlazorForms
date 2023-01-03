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
		List<BoardColumn> Columns { get; }
		List<BoardCard> Cards { get; }

		Task LoadAsync(Type flowType);
		Task RefreshCardsAsync(List<BoardCard> cards);
	}
}
