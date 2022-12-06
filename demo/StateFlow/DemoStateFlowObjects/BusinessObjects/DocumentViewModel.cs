using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Flows.Engine.StateFlow;
using System.Collections.Generic;
using System.Reflection;
using BlazorForms.Platform.Settings;
using BlazorForms.Shared;
using BlazorForms.Flows.Engine;
using BlazorForms.DynamicCode;

namespace BlazorFormsStateFlowDemoApp.BusinessObjects
{
    public class DocumentViewModel
    {
        private int _transactionId = 100000;
        private const string USER_FLOW_NAME = "CustomDocumentFlow";
        private readonly IStateFlowRunEngine _flowRunEngine;
        private readonly IDynamicFlowProvider _dynamicFlowProvider;
        private readonly IFlowRunStorage _storage;
        private Type _currentFlowType = typeof(DocumentFlow);
        private DynamicCodeContext _currentCode;

        public readonly List<DocumentModel> Transactions = new List<DocumentModel>();
        public readonly string[] Exceptions = new string[] { "Duplicate", "Amount exceeded", "Suspicious account", "Missed" };
        public string CompilationErrors { get; set; }
        public string CodeChanged { get; set; } = "(not compiled)";

        public List<DocumentModel> NewTransactions = new List<DocumentModel>();
        public List<DocumentModel> AssignedTransactions = new List<DocumentModel>();
        public List<DocumentModel> UnderInvestigationTransactions = new List<DocumentModel>();
        public List<DocumentModel> EscalatedTransactions = new List<DocumentModel>();
        public List<DocumentModel> ClosedTransactions = new List<DocumentModel>();

        private bool _isStorageEnabled;

        public bool IsStorageEnabled 
        { 
            get 
            { 
                return _isStorageEnabled; 
            } 
            set 
            {
                _isStorageEnabled = value;
                Transactions.Clear();
                PopulateTransactions();

                if (OnChanged != null)
                {
                    OnChanged();
                }
            } 
        }

        public Action OnChanged;

        public DocumentViewModel(IStateFlowRunEngine flowRunEngine, IDynamicFlowProvider dynamicFlowProvider, IFlowRunStorage storage)
        {
            _flowRunEngine = flowRunEngine;
            _dynamicFlowProvider = dynamicFlowProvider;
            _storage = storage;

            FlowCode = FlowCode.Replace($"public class {_currentFlowType.Name} : DocumentFlowBase<DocumentModel>", 
                $"public class {USER_FLOW_NAME} : DocumentFlowBase<DocumentModel>");
        }

        private object _lock = new object();

        public void ApplyFlowCode()
        {
            CompilationErrors = null;
            var codeText = FlowCode;

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(DocumentModel).GetTypeInfo().Assembly.Location),
            };

            // ToDo: DynamicAssembly name policy is required!
            // ToDo: Dynamic Assembly name should be unique for each user, in other case one user can execute flow compiled by another user
            var ps = new DynamicCodeParameters
            {
                AssemblyName = "DynamicAssembly",
                Namespace = "BlazorFormsStateFlowDemoApp.BusinessObjects",
                ClassName = USER_FLOW_NAME,
                Code = codeText,
                References = references,
            };

            _currentCode = _dynamicFlowProvider.CompileFlow(ps);

            if (_currentCode.Success)
            {
                _currentFlowType = _currentCode.ClassType;
                CodeChanged = null;
            }
            else
            {
                CompilationErrors = "";

                foreach (var err in _currentCode.ValidationIssues)
                {
                    CompilationErrors += err.ToString() + "\r\n";
                }

                foreach (var err in _currentCode.CompilationResult.Diagnostics)
                {
                    CompilationErrors += err.ToString() + "\r\n";
                }
            }
        }

        public async Task<StateFlowTaskDetails> GetTransactionSelectorData(string id)
        {
            var t = Transactions.First(d => d.Document.TransactionId == id);
            
            var ps = new FlowRunParameters 
            { 
                FlowType = _currentFlowType, 
                Context = t.FlowContext, 
                RefId = t.RefId,
                NoStorageMode = !IsStorageEnabled 
            };
            
            var result = await _flowRunEngine.GetStateDetails(ps);
            return result;
        }

        public async Task TransitionClicked(string action, TransitionDef transition, DocumentModel transaction)
        {
            if (IsStorageEnabled)
            {
                await _flowRunEngine.ContinueFlow(transaction.RefId, transaction, action);
            }
            else
            {
                await _flowRunEngine.ContinueFlowNoStorage(transaction.FlowContext, action);
            }

            await RefreshTransactions();
        }

        public async Task RandomChangeTransactionAndSync()
        { }

        public async Task CreateTransaction()
        {
            _transactionId++;
            var rnd = new Random();

            var t = new DocumentModel
            {
                Document = new Document
                {
                    TransactionId = _transactionId.ToString(),
                    AccountId = $"{rnd.Next(899) + 100}-{rnd.Next(899) + 100} {rnd.Next(10000) + 1000000}",
                    Date = DateTime.Now - new TimeSpan(rnd.Next(899), 0, 0, 0),
                    Amount = Convert.ToDecimal(rnd.Next(20000)),
                    FoundIssue = Exceptions[rnd.Next(4)],
                    CreatedUser = rnd.Next(3) < 1 ? "tcra" : "none"
                }
            };

            Transactions.Add(t);

            var ps = new FlowRunParameters { FlowType = _currentFlowType, Model = t, NoStorageMode = !IsStorageEnabled };
            var context = await _flowRunEngine.ExecuteFlow(ps);

            if (IsStorageEnabled)
            {
                t.RefId = context.RefId;
            }
            else
            {
                t.FlowContext = context;
            }

            await RefreshTransactions();
        }

        private static IEnumerable<FlowStatus> FlowStatusesAllNotDeleted
        {
            get
            {
                return new[] { FlowStatus.Created, FlowStatus.Waiting, FlowStatus.Started, FlowStatus.Finished, FlowStatus.Failed };
            }
        }

        public async Task RefreshTransactions()
        {
            if (IsStorageEnabled)
            {
                var ids = Transactions.Select(t => t.RefId);

                var ctxs = await _storage.GetFlowContexts(new FlowModelsQueryOptions
                {
                    RefIds = ids,
                    FlowStatuses = FlowStatusesAllNotDeleted
                });

                var dict = ctxs.ToDictionary(c => c.RefId, c => c);

                Transactions.ForEach(t =>
                {
                    t.State = dict[t.RefId].GetState();
                    t.StatusMessage = dict[t.RefId].StatusMessage;
                });
            }
            else
            {
                Transactions.ForEach(t =>
                {
                    t.State = t.FlowContext.GetState();
                    t.StatusMessage = t.FlowContext.StatusMessage;
                });
            }

            PopulateTransactions();
        }

        private void PopulateTransactions()
        {            
            NewTransactions = Transactions.Where(t => t.State == "New").ToList();
            AssignedTransactions = Transactions.Where(t => t.State == "Assigned").ToList();
            UnderInvestigationTransactions = Transactions.Where(t => t.State == "Reviewing").ToList();
            EscalatedTransactions = Transactions.Where(t => t.State == "Alarmed").ToList();
            ClosedTransactions = Transactions.Where(t => t.State == "Closed").ToList();
        }

        #region FlowCode
        public string FlowCode { get; set; } = @"
using System.Collections.Generic;
using BlazorForms.Flows;
using BlazorForms.Shared;

namespace BlazorFormsStateFlowDemoApp.BusinessObjects
{
    public class DocumentFlow : DocumentFlowBase<DocumentModel>
    {
        public override void Define()
        {
            this.State(New)
                .Transition(AssignTrigger, Assigned, OnAssigning)
                .Transition(DaySpanTrigger(1), Assigned, OnAssigning)
            .State(Assigned)
                .Transition(ReviewingTrigger, Reviewing, OnReviewing)
                .Transition(FastResolveTrigger, Closed, OnAssignClosing)
                .Transition(DaySpanTrigger(15), Alarmed, OnAlarming)
            .State(Reviewing)
                .Transition(ReturnAssignTrigger, Assigned, OnReturnAssigning)
                .Transition(CloseTrigger, Closed, OnClosing)
                .Transition(DaySpanTrigger(5), Alarmed, OnAlarming)
            .State(Alarmed)
                .Transition(CloseTrigger, Closed, OnClosing)
            .State(Closed)
                .Transition(ReopenTrigger, Assigned, OnReopening)
                .End();
        }

        private TransitionTrigger AssignTrigger()
        {
            return new ButtonTransitionTrigger(""Assign"", new string[] { ""ivan@satoshi.com"", ""john@satoshi.com"", ""michael@satoshi.com"" });
        }
        private TransitionTrigger ReturnAssignTrigger()
        {
            return new ButtonTransitionTrigger(""Return"", new string[] { ""ivan@satoshi.com"", ""john@satoshi.com"", ""michael@satoshi.com"" });
        }

        private TransitionTrigger FastResolveTrigger()
        {
            return new ButtonTransitionTrigger(""Resolve"", new string[] { ""Ignore"", ""Report"", ""Rollback"" });
        }

        private TransitionTrigger ReviewingTrigger()
        {
            return new ButtonTransitionTrigger(""Reviewing"");
        }

        private TransitionTrigger CloseTrigger()
        {
            return new ButtonTransitionTrigger(""Close"", new string[] { ""Ignore"", ""Report"", ""Rollback"" });
        }

        private TransitionTrigger ReopenTrigger()
        {
            return new ButtonTransitionTrigger(""Open"");
        }

        private void OnAssigning()
        {
            Status = Open;
            Model.AssignedUser = Model.TriggerSelectedValue;
        }
        private void OnReturnAssigning()
        {
            Status = Open;
            Model.AssignedUser = Model.TriggerSelectedValue;
        }

        private void OnAssignClosing()
        {
            Status = StatusClosed;
            Model.Resolution = Model.TriggerSelectedValue;
        }

        private void OnClosing()
        {
            Status = StatusClosed;
            Model.Resolution = Model.TriggerSelectedValue;
        }

        private void OnReviewing()
        {
            Status = Open;
        }

        private void OnReopening()
        {
            Status = Open;
        }

        private void OnAlarming()
        {
            Status = Stale;
        }
    }
}
";
        #endregion
    }
}
