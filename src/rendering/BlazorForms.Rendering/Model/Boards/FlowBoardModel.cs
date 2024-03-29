﻿using BlazorForms.Flows.Definitions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Rendering.Model
{
    public interface IFlowBoardCard : IFlowModel
    {
		public string State { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int Order { get; set; }
	}

    public record CardInfo<T>(T Item)
        where T : class, IFlowBoardCard
	{
		public string? RefId { get; set; }
		public IFlowContext? Context { get; set; }

		public string? TitleInitials
		{
			get
			{
				return Item.Title?.Length > 2 ? Item.Title.Substring(0, 2) : Item.Title;
			}
		}

		public string? Title
		{
			get
			{
				return Item.Title?.Length > 20 ? $"{Item.Title.Substring(0, 20)}..." : Item.Title;
			}
		}

		public string? Description
		{
			get
			{
				return Item.Description?.Length > 40 ? $"{Item.Description.Substring(0, 40)}..." : Item.Description;
			}
		}
	}

	public enum ItemChangedType
	{
		Order,
		State,
		Creating,
		Added,
		Changed,
		Deleted,
	}

	//public class ItemsChangedArgs<T>
	//{ }

	public class BoardDialogSubmittedArgs
	{
		public ItemChangedType Type { get; set; }
		public IFlowBoardCard? Card { get; set; }
	}

    public class BoardCardChangedArgs<T>
		where T: class, IFlowBoardCard
	{
		public T Item { get; private set; }
		public ItemChangedType Type { get; private set; }
		public string NewState { get; private set; }
		public string OldState { get; private set; }

		public BoardCardChangedArgs(T item, ItemChangedType type)
		{
			Item = item;
			Type = type;
		}

        public BoardCardChangedArgs(T item, ItemChangedType type, string oldState, string newState)
        {
            Item = item;
            Type = type;
			OldState = oldState;
			NewState = newState;
        }

		public bool ChangedToTargetState(string state)
		{
			return Type == ItemChangedType.State && OldState != NewState && NewState == state;
		}
    }

	public class FlowBoardColumn
    {
		public string Id { get; set; }
		public string Name { get; set; }
	}

	public class FlowBoardContextMenuAction
	{
		public const string DIVIDER_ACTION = "$.Divider";
		public const string EDIT_ACTION = "$.Edit";
		public const string DELETE_ACTION = "$.Delete";

		public string? Name { get; set; }
		public string? State { get; set; }
		public string? FormType { get; set; }
		public bool Disabled { get; set; }
	}


	//public class FlowBoardCard : IFlowBoardCard
	//{
 //       public IFlowContext Context { get; set; }       
 //       public string RefId { get; set; }       
 //       //public state State { get; set; }  
 //       public object ModelUntyped { get; set; }  

 //       public string State { get; set; }  
 //       public string Title { get; set; }       
 //       public string Description { get; set; }       
 //       public int Order { get; set; }       
	//}

	//public class FlowBoardCard<T> : FlowBoardCard
	//	where T: class
 //   {
 //       public T Model { get { return ModelUntyped as T; } set { ModelUntyped = value; } }
 //   }
}
