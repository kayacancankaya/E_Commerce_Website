namespace IakademiWebProject.Models
{
	public class MainPageModel
	{
        public List<Product>? SliderProducts { get; set; }
        public Product? Productoftheday { get; set; }
		public List<Product>? NewProducts { get; set; }
		public List<Product>? SpecialProducts { get; set; }
		public List<Product>? DiscountedProducts { get; set; }
		public List<Product>? HighLightedProducts { get; set; }
		public List<Product>? TopSellerProducts { get; set; }
		public List<Product>? StarProducts { get; set; }
	}
}
