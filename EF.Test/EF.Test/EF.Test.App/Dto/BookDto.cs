namespace EF.Test.App.Dto
{
    public record BookDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Published { get; set; }

        public int PublishedYear { get; set; }

        public string PublisherName { get; set; }

        public string ISBN { get; set; }
    }
    
  
  

    public class GroupedInfo
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public int AuthorId { get; set; }
        public DateTime Published { get; set; }
        
        public string Name { get; set; }
    }
}