namespace BookShop.WebService.Dtos;

public class AuthorReadDto
{
   
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<BookReadDto> Books { get; set; }
}