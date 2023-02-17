using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [StringLength(maximumLength:50)]
        public string Title1 { get; set; }
        [StringLength(maximumLength: 50)]
        public string Title2 { get; set; }

        [StringLength(maximumLength: 150)]
        public string Desc { get; set; }
        [StringLength(maximumLength: 100)]

        public string? ImageUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string ButtonText { get; set; }
        public int Order { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
