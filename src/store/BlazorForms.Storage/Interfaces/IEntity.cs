namespace BlazorForms.Storage.Interfaces
{
    public interface IEntity
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
    }
}
