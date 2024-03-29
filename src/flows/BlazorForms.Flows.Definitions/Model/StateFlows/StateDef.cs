﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Definitions
{
	public class StateDef
	{
		public string State { get; set; }
		public string Type { get; set; }
		public string Caption { get; set; }
		public bool IsEnd { get; internal set; }
		public Func<Task> OnBeginAsync { get; internal set; }
	}
}
