
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [StringLength(maximumLength: 150)]
        public string Desc { get; set; }
        [StringLength(maximumLength: 50)]

        public string Name { get; set; }
        public double CostPrice { get; set; }
        public double DisCountPrice { get; set; }
        public double SalePrice { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }

        public string Code { get; set; }

        public int BookCount { get; set; }
        [NotMapped]
        public List<IFormFile>? ImageFiles { get; set; }
        [NotMapped]
        public IFormFile? PosterImageFile { get; set; }
        [NotMapped]
        public IFormFile? HoverImageFile { get; set; }

        public Category? Category { get; set; }
        public Author? Author  { get; set; }

        public List<BookImage>? bookImages { get; set; }

        [NotMapped]
        public List<int>? BookImageIds { get; set; }
        public List<OrderItem> OrderItems { get; set; }= new List<OrderItem>();

    }
}
