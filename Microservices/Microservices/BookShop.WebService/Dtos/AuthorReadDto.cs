namespace BookShop.WebService.Dtos;

public class AuthorReadDto
{
   
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public List<BookReadDto>? Books { get; set; }
}