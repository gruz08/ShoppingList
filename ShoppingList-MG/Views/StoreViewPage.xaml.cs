using ShoppingList.Models;
using ShoppingList.Services;
using System.Collections.ObjectModel;

namespace ShoppingList.Views
{
    public partial class StoreViewPage : ContentPage
    {
        private readonly ObservableCollection<Category> categories;
        private readonly string storeName;
        private readonly DataService dataService;

        public StoreViewPage(ObservableCollection<Category> categories, string storeName, DataService dataService)
        {
            InitializeComponent();
            this.categories = categories;
            this.storeName = storeName;
            this.dataService = dataService;
            Title = $"Lista: {storeName}";
            LoadProducts();
        }

        private void LoadProducts()
        {
            ProductsContainer.Children.Clear();

            var storeProducts = new List<Product>();

            foreach (var category in categories.OrderBy(c => c.Name))
            {
                var categoryProducts = category.Products
                    .Where(p => p.StoreName == storeName && !p.IsPurchased)
                    .ToList();

                storeProducts.AddRange(categoryProducts);
            }

            if (storeProducts.Count == 0)
            {
                var noItemsLabel = new Label
                {
                    Text = $"Brak produktów dla sklepu {storeName}",
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                ProductsContainer.Children.Add(noItemsLabel);
                return;
            }

            foreach (var product in storeProducts)
            {
                var productView = new ProductItemView
                {
                    BindingContext = product
                };

                productView.ProductChanged += OnProductChanged;
                productView.ProductDeleted += OnProductDeleted;

                ProductsContainer.Children.Add(productView);
            }
        }

        private async void OnProductChanged(object sender, Product product)
        {
            await dataService.SaveCategoriesAsync(categories);
            LoadProducts();
        }

        private async void OnProductDeleted(object sender, Product product)
        {
            var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            if (category != null)
            {
                category.Products.Remove(product);
                await dataService.SaveCategoriesAsync(categories);
                LoadProducts();
            }
        }
    }
}