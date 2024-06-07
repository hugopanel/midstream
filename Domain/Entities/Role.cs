namespace Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Permission> Permissions { get; set; }
        public Project Project { get; set; }

        /*
        public Role(string name, List<Permission> permissions, Project project)
        {
            Id = Guid.NewGuid();
            Name = name;
            Permissions = permissions;
            Project = project;
        }*/
    }
}