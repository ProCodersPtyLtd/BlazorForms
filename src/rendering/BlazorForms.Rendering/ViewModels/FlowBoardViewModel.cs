﻿using BlazorForms.Flows;
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

		public List<CardInfo<IFlowBoardCard>> Cards { get; set; }
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

		public async Task RefreshCardsAsync(List<IFlowBoardCard> cards)
		{
			Cards = new List<CardInfo<IFlowBoardCard>>();
			cards.ForEach(async c => Cards.Add(await GetLoadedCard(c)));

			//Cards = cards.Select(c => GetLoadedCard(c)).OrderBy(c => c.Order).ToList();
		}

		private async Task<CardInfo<IFlowBoardCard>> GetLoadedCard(IFlowBoardCard c)
		{
			//var r = new CardInfo<IFlowBoardCard>(c.ReflectionGetCopy()); 
			var r = new CardInfo<IFlowBoardCard>(c); 
			r.Context = await _flowRunEngine.CreateFlowContext(_currentFlowType, c, r.Item.State);

			//if (r.Item.Title?.Length > 20)
			//{
			//	r.Item.Title = $"{r.Item.Title.Substring(0, 20)}...";
			//}

			//if (r.Item.Description?.Length > 32)
			//{
			//	r.Item.Description = $"{r.Item.Description.Substring(0, 32)}...";
			//}

			return r;
		}

		public bool IsTransitionPossible(CardInfo<IFlowBoardCard> card, string column)
		{
			if (card.Item.State == column)
			{
				return true;
			}

			var transitions = _flowDetails.Transitions.Where(x => x.FromState == card.Item.State && x.IsUserActionTrigger());
			var result = transitions.Any(x => x.ToState == column);
            return result;
		}

		public async Task PerformTransition(CardInfo<IFlowBoardCard> card, string action)
		{
			if (IsStorageEnabled)
			{
				await _flowRunEngine.ContinueFlow(card.RefId, action);
			}
			else
			{
				card.Context = await _flowRunEngine.ContinueFlowNoStorage(card.Context, action);
				card.Item.State = card.Context.CurrentTask;
			}

			// ToDo: we need to save the changed card
		}

		public async Task ReorderCards(CardInfo<IFlowBoardCard> card, int newOrder)
		{
			int num2 = 0;
			foreach (var item2 in Cards.OrderBy(x => x.Item.Order))
			{
				if (item2.Equals(card))
				{
					card.Item.Order = newOrder;
					continue;
				}

				if (num2 == newOrder)
				{
					num2++;
				}

				item2.Item.Order = num2;
				num2++;
			}

			// ToDo: we need to save the changed cards
		}

		public string? GetStateForm(string state)
		{
			var result = (_flowDetails.Forms.FirstOrDefault(x => x.State == state) ?? _flowDetails.Forms.FirstOrDefault(x => x.State == null))?.FormType;
			return result;
		}
	}
}
