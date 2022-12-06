using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorFormsDemoFlows.Flows;
using BlazorFormsDemoModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorFormsDemoFlows.Forms
{
    public class FieldSetSampleForm1 : FormEditBase<FieldSetModel1>
    {
        protected override void Define(FormEntityTypeBuilder<FieldSetModel1> f)
        {
            f.DisplayName = this.GetType().Name;

            f.Group("Person");
            f.Property(p => p.Name).Control(ControlType.Label);
            f.Property(p => p.Amount).Label("Credit").Control(ControlType.MoneyEdit).IsReadOnly();
            
            f.Group("Company");
            f.Property(p => p.Company).Control(ControlType.Label);
            f.Property(p => p.Abn).Control(ControlType.TextEdit).IsReadOnly();

            f.Table(p => p.Addresses, e =>
            {
                e.DisplayName = "Contacts";
                e.Property(p => p.Phone);
                e.Property(p => p.Title);
                e.Property(p => p.FirstName);
                e.Property(p => p.LastName);
            });

            f.Group("2");
        }
    }
}
