using ShoppingList.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ShoppingList.Views
{
    public partial class CategoryView : ContentView
    {
        public event EventHandler<Product> ProductChanged;
        public event EventHandler<Product> ProductDeleted;

        private Category category;
        private ObservableCollection<ProductItemView> productViews;

        public CategoryView()
        {
            InitializeComponent();
            productViews = new ObservableCollection<ProductItemView>();

            ProductsCollectionView.ItemTemplate = new DataTemplate(() =>
            {
                var contentView = new ContentView();
                contentView.SetBinding(ContentView.ContentProperty, ".");
                return contentView;
            });

            ProductsCollectionView.ItemsSource = productViews;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (category != null)
            {
                category.Products.CollectionChanged -= OnProductsCollectionChanged;
            }

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
                ProductsCollectionView.IsVisible = category.IsExpanded;
                ExpandButton.Text = category.IsExpanded ? "▲" : "▼";
            }
        }

        private void OnProductsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateProductsList();
        }

        private void UpdateProductsList()
        {
            foreach (var view in productViews)
            {
                view.ProductChanged -= OnProductChangedInternal;
                view.ProductDeleted -= OnProductDeletedInternal;
            }

            productViews.Clear();

            if (category == null)
                return;

            var orderedProducts = category.Products
                .OrderBy(p => p.IsPurchased)
                .ToList();

            foreach (var product in orderedProducts)
            {
                var productView = new ProductItemView
                {
                    BindingContext = product
                };

                if (product.IsPurchased)
                {
                    productView.Opacity = 0.5;
                }

                productView.ProductChanged += OnProductChangedInternal;
                productView.ProductDeleted += OnProductDeletedInternal;

                productViews.Add(productView);
            }

            ProductsCollectionView.IsVisible = category.IsExpanded;
            ExpandButton.Text = category.IsExpanded ? "▲" : "▼";
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
