﻿using System.ComponentModel.DataAnnotations.Schema;

namespace EF.Test.App.Entities
{
    public class Author
    {
        public int Id { get; set; }

        public int Age { get; set; }

        public string Country { get; set; }

        public int BooksCount { get; set; }

        public string NickName { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int UserId { get; set; }

        public List<Book> Books { get; set; } = new List<Book>();
    }
}