using System.ComponentModel.DataAnnotations;

namespace BookShop.WebService.Models;

public class Book
{
    [Key]
    public int Id { get; set; }
    public int AuthorId { get; set; }
    [Required]
    public string Title { get; set; } = null!;

    [Required]

    public Author Author { get; set; } = null!;
}