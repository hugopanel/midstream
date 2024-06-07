using Domain.Entities;

namespace Domain.Interfaces;

public interface IModule
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Author { get; set; } // Replace with a "Module Author" object
    public List<Permission> Permissions { get; set; } // List of new permissions defined by the module
    public string FrontPath { get; set; } // The path to the front-end files
}