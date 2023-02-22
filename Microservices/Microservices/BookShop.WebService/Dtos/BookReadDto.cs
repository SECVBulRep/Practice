namespace BookShop.WebService.Dtos;

public class BookReadDto
{
  
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string Title { get; set; } = null!;
    public AuthorReadDto Author { get; set; } = null!;
}