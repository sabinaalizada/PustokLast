using System.ComponentModel.DataAnnotations;

namespace Pustok.Models
{
    public class Feature
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 50)]
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
    }
}
