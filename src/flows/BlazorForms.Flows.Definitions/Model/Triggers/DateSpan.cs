using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Flows.Engine.StateFlow
{
    public class DateSpan
    {
        public TimeSpan Value;
    }

    public class DaySpan : DateSpan
    {
        public DaySpan(int days)
        {
            Value = new TimeSpan(days, 0, 0, 0);
        }
    }
}
