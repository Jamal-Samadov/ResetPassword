using FlowerSite.Models;

namespace FlowerSite.ViewModels
{
    public class BasketViewModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? Count { get; set; }
    }
}
