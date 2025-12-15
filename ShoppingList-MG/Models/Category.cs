using System.Collections.ObjectModel;

namespace ShoppingList.Models
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public bool IsExpanded { get; set; }

        public Category()
        {
            Id = Guid.NewGuid().ToString();
            Name = string.Empty;
            Products = new ObservableCollection<Product>();
            IsExpanded = false;
        }
    }
}