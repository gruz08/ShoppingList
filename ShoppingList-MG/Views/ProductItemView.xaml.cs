using ShoppingList.Models;

namespace ShoppingList.Views
{
    public partial class ProductItemView : ContentView
    {
        public event EventHandler<Product> ProductChanged;
        public event EventHandler<Product> ProductDeleted;

        public ProductItemView()
        {
            InitializeComponent();
        }

        private void OnPurchasedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (BindingContext is Product product)
            {
                product.IsPurchased = e.Value;
                ProductChanged?.Invoke(this, product);
            }
        }

        private void OnIncreaseClicked(object sender, EventArgs e)
        {
            if (BindingContext is Product product)
            {
                product.Quantity++;
                ProductChanged?.Invoke(this, product);
            }
        }

        private void OnDecreaseClicked(object sender, EventArgs e)
        {
            if (BindingContext is Product product)
            {
                if (product.Quantity > 1)
                {
                    product.Quantity--;
                    ProductChanged?.Invoke(this, product);
                }
            }
        }

        private void OnQuantityChanged(object sender, TextChangedEventArgs e)
        {
            if (BindingContext is Product product)
            {
                if (int.TryParse(e.NewTextValue, out int quantity))
                {
                    if (quantity >= 1)
                    {
                        product.Quantity = quantity;
                        ProductChanged?.Invoke(this, product);
                    }
                }
            }
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (BindingContext is Product product)
            {
                ProductDeleted?.Invoke(this, product);
            }
        }
    }
}