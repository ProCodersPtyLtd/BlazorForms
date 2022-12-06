using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorForms.Flows.Definitions
{
    public class TaskExecutionResult
    {
        public TaskExecutionResultStateEnum ResultState { get; set; }
        public TaskExecutionFlowStateEnum FlowState { get; set; }
        public FormTaskStateEnum FormState { get; set; }
        public string FormLastAction { get; set; }
        public bool IsFormTask { get; set; }
        public bool IsWaitTask { get; set; }
        public string FormId { get; set; }
        public string CallbackTaskId { get; set; }
        public bool PreloadTableData { get; set; }
        public string NextStep { get; set; }
        public string ExceptionMessage { get; set; }
        public List<TaskExecutionValidationResult> TaskExecutionValidationIssues { get; set; } = new List<TaskExecutionValidationResult>();
        public string ExceptionStackTrace { get; set; }
        public string ExceptionType { get; set; }
        public Exception ExecutionException { get; set; }

        // Dates
        public DateTime CreatedDate { get; set; }
        public DateTime? ChangedDate { get; set; }
        public DateTime? FinishedDate { get; set; }

        // States and Statuses
    }
}
