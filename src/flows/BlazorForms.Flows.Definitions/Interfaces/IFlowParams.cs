namespace BlazorForms.Flows.Definitions
{
    public interface IFlowParams
    {
        string this[string index] { get; set;}
        string AssignedUser { get; set; }
        string AssignedTeam { get; set; }
    }
}
