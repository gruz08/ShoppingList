using ShoppingList.Models;
using System.Collections.Specialized;

namespace ShoppingList.Views
{
    public partial class CategoryView : ContentView
    {
        public event EventHandler<Product> ProductChanged;
        public event EventHandler<Product> ProductDeleted;

        private Category category;

        public CategoryView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is Category cat)
            {
                category = cat;
                category.Products.CollectionChanged += OnProductsCollectionChanged;
                UpdateProductsList();
            }
        }

        private void OnToggleExpanded(object sender, EventArgs e)
        {
            if (category != null)
            {
                category.IsExpanded = !category.IsExpanded;
                ProductsContainer.IsVisible = category.IsExpanded;

                if (sender is Button button)
                {
                    button.Text = category.IsExpanded ? "▲" : "▼";
                }
            }
        }

        private void OnProductsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateProductsList();
        }

        private void UpdateProductsList()
        {
            ProductsContainer.Children.Clear();

            if (category == null)
                return;

            var sortedProducts = category.Products
                .OrderBy(p => p.IsPurchased)
                .ToList();

            foreach (var product in sortedProducts)
            {
                var productView = new ProductItemView
                {
                    BindingContext = product
                };

                productView.ProductChanged += OnProductChangedInternal;
                productView.ProductDeleted += OnProductDeletedInternal;

                if (product.IsPurchased)
                {
                    productView.Opacity = 0.5;
                }

                ProductsContainer.Children.Add(productView);
            }
        }

        private void OnProductChangedInternal(object sender, Product product)
        {
            UpdateProductsList();
            ProductChanged?.Invoke(this, product);
        }

        private void OnProductDeletedInternal(object sender, Product product)
        {
            ProductDeleted?.Invoke(this, product);
        }
    }
}