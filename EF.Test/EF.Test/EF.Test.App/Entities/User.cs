namespace EF.Test.App.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public DateTime Created { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime LastActivity { get; set; }
    }
}