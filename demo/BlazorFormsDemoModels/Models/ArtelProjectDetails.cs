using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BlazorForms.Shared.DataStructures;

namespace BlazorForms.Platform.Crm.Domain.Models.Artel
{
    public class ArtelProjectDetails
    {
        public virtual int Id { get; set; }
        
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual ArtelProjectStatistics Statistics { get; set; }
        public virtual string BaseCurrencySearch { get; set; }
        public virtual decimal DefaultSharesPaymentProportionPercent { get; set; }
        public virtual Money InitialSharePrice { get; set; } = new Money();
        public virtual string PaymentFrequencyCode { get; set; }
        public virtual int PaymentFrequencyDay { get; set; }
        public virtual bool PaymentNotification { get; set; }
        public virtual byte[] RoadmapAttached { get; set; }
    }
}
