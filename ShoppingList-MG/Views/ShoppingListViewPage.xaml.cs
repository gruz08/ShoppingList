using ShoppingList.Models;
using ShoppingList.Services;
using System.Collections.ObjectModel;

namespace ShoppingList.Views
{
    public partial class ShoppingListViewPage : ContentPage
    {
        private readonly ObservableCollection<Category> categories;
        private readonly DataService dataService;

        public ShoppingListViewPage(ObservableCollection<Category> categories, DataService dataService)
        {
            InitializeComponent();
            this.categories = categories;
            this.dataService = dataService;
            LoadProducts();
        }

        private void LoadProducts()
        {
            ProductsContainer.Children.Clear();

            var unpurchasedProducts = new List<Product>();

            foreach (var category in categories.OrderBy(c => c.Name))
            {
                var categoryProducts = category.Products
                    .Where(p => !p.IsPurchased)
                    .ToList();

                unpurchasedProducts.AddRange(categoryProducts);
            }

            if (unpurchasedProducts.Count == 0)
            {
                var noItemsLabel = new Label
                {
                    Text = "Brak produktów do kupienia",
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                ProductsContainer.Children.Add(noItemsLabel);
                return;
            }

            foreach (var product in unpurchasedProducts)
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