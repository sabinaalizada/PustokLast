namespace Pustok.Models
{
    public class BookImage
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsPoster { get; set; }
        public Book? Book { get; set; }  

    }
}
