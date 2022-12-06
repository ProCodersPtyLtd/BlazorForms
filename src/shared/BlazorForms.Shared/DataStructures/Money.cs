using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared.DataStructures
{
    public class Money
    {
        public virtual string Currency { get; set; } = "";
        public virtual decimal? Amount { get; set; } = 0m;

        public string DefaultFormatted => $"{Amount} {Currency}";

        public Money()
        {
        }

        public Money(decimal? amount)
        {
            Amount = amount;
        }

        public Money(decimal? amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }
    }
}
