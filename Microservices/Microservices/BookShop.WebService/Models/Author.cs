using System.ComponentModel.DataAnnotations;

namespace BookShop.WebService.Models;

public class Author
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    public List<Book> Books { get; set; } = null!;
}