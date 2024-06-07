namespace Domain.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string Action { get; set; } // Name
        public string Description { get; set; }
    }
}