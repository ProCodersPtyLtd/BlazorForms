using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Shared
{
    public class ModelBinding
    {
        public const string RejectButtonBinding = "$.ActionButtons.Reject";
        public const string SubmitButtonBinding = "$.ActionButtons.Submit";
        public const string SubmitCloseButtonBinding = "$.ActionButtons.SubmitClose";
        public const string ValidateButtonBinding = "$.ActionButtons.Validate";
        public const string SaveButtonBinding = "$.ActionButtons.Save";
        public const string CloseButtonBinding = "$.ActionButtons.Close";
        public const string CloseFinishButtonBinding = "$.ActionButtons.CloseFinish";
        public const string CustomButtonBinding = "$.ActionButtons.Custom";
        public const string EditButtonBinding = "$.ActionButtons.Edit";
        public const string DeleteButtonBinding = "$.ActionButtons.Delete";

        public const string ListFormContextMenuBinding = "$.Menu";
        public const string FlowReferenceButtonsBinding = "$.RefButtons";
        public const string FormLevelBinding = "(global)";
        public const string ListFormMainMenuBinding = "$.MainMenu";
    }
}
