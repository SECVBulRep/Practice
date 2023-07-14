namespace OptimizeMe.App.Entities;

public class Role
{
    public int Id { get; set; }

    public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public string Name { get; set; }
}