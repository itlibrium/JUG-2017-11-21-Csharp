namespace JUG.CRUD
{
    public class PricingCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal MinPrice { get; set; }
        public decimal PricePerHour { get; set; }

        //...
    }
}