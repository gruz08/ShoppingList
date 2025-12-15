namespace ShoppingList.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public bool IsPurchased { get; set; }
        public string CategoryId { get; set; }
        public string StoreName { get; set; }

        public Product()
        {
            Id = Guid.NewGuid().ToString();
            Name = string.Empty;
            Unit = "szt.";
            Quantity = 1;
            IsPurchased = false;
            CategoryId = string.Empty;
            StoreName = string.Empty;
        }
    }
}