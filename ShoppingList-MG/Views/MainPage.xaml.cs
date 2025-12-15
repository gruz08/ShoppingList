using ShoppingList.Models;
using ShoppingList.Services;
using System.Collections.ObjectModel;

namespace ShoppingList.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly DataService dataService;
        private ObservableCollection<Category> categories;
        private ObservableCollection<Store> stores;

        public MainPage()
        {
            InitializeComponent();
            dataService = new DataService();
            LoadData();
        }

        private async void LoadData()
        {
            categories = await dataService.LoadCategoriesAsync();
            stores = await dataService.LoadStoresAsync();
            UpdateCategoriesList();
        }

        private void UpdateCategoriesList()
        {
            CategoriesContainer.Children.Clear();

            foreach (var category in categories)
            {
                var categoryView = new CategoryView
                {
                    BindingContext = category
                };

                categoryView.ProductChanged += OnProductChanged;
                categoryView.ProductDeleted += OnProductDeleted;

                CategoriesContainer.Children.Add(categoryView);
            }
        }

        private async void OnProductChanged(object sender, Product product)
        {
            await SaveData();
            UpdateCategoriesList();
        }

        private async void OnProductDeleted(object sender, Product product)
        {
            var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            if (category != null)
            {
                category.Products.Remove(product);
                await SaveData();
                UpdateCategoriesList();
            }
        }

        private async void OnAddCategoryClicked(object sender, EventArgs e)
        {
            string categoryName = NewCategoryEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                await DisplayAlert("B³¹d", "Podaj nazwê kategorii", "OK");
                return;
            }

            var newCategory = new Category { Name = categoryName };
            categories.Add(newCategory);

            NewCategoryEntry.Text = string.Empty;

            await SaveData();
            UpdateCategoriesList();
        }

        private async void OnAddProductClicked(object sender, EventArgs e)
        {
            if (categories.Count == 0)
            {
                await DisplayAlert("B³¹d", "Najpierw dodaj kategoriê", "OK");
                return;
            }

            string productName = await DisplayPromptAsync("Nowy produkt", "Nazwa produktu:");

            if (string.IsNullOrWhiteSpace(productName))
                return;

            string[] categoryNames = categories.Select(c => c.Name).ToArray();
            string selectedCategory = await DisplayActionSheet("Wybierz kategoriê", "Anuluj", null, categoryNames);

            if (selectedCategory == "Anuluj" || string.IsNullOrEmpty(selectedCategory))
                return;

            string unit = await DisplayActionSheet("Wybierz jednostkê", "Anuluj", null, "szt.", "kg", "l", "g", "ml");

            if (unit == "Anuluj" || string.IsNullOrEmpty(unit))
                unit = "szt.";

            string[] storeNames = stores.Select(s => s.Name).ToArray();
            string selectedStore = await DisplayActionSheet("Wybierz sklep", "Anuluj", null, storeNames);

            if (selectedStore == "Anuluj" || string.IsNullOrEmpty(selectedStore))
                selectedStore = string.Empty;

            var category = categories.FirstOrDefault(c => c.Name == selectedCategory);

            if (category != null)
            {
                var newProduct = new Product
                {
                    Name = productName,
                    Unit = unit,
                    CategoryId = category.Id,
                    StoreName = selectedStore
                };

                category.Products.Add(newProduct);

                await SaveData();
                UpdateCategoriesList();
            }
        }

        private async void OnShowShoppingListClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ShoppingListViewPage(categories, dataService));
        }

        private async void OnShowStoreViewClicked(object sender, EventArgs e)
        {
            if (stores.Count == 0)
            {
                await DisplayAlert("B³¹d", "Brak sklepów", "OK");
                return;
            }

            string[] storeNames = stores.Select(s => s.Name).ToArray();
            string selectedStore = await DisplayActionSheet("Wybierz sklep", "Anuluj", null, storeNames);

            if (selectedStore != "Anuluj" && !string.IsNullOrEmpty(selectedStore))
            {
                await Navigation.PushAsync(new StoreViewPage(categories, selectedStore, dataService));
            }
        }

        private async Task SaveData()
        {
            await dataService.SaveCategoriesAsync(categories);
        }
    }
}