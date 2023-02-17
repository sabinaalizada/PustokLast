using Pustok.Models;

namespace Pustok.viewModel
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Book> NewBooks { get; set; }
        public List<Book> DiscountBooks { get; set; }
        public List<Book> FeaturedBooks { get; set; }
        public List<Feature> Features { get; set; }


    }
}
