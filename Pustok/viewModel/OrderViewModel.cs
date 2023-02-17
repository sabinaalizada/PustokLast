namespace Pustok.viewModel
{
    public class OrderViewModel
    {
        public List<CheckOutItemViewModel>? CheckOutItemViewModels { get; set; }
        public List<ShoppingCardViewModel>? ShoppingCardViewModels { get; set; }
        public string FullName { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string? Note { get; set; }
    }
}
