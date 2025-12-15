using System.Collections.ObjectModel;
using System.Text.Json;
using ShoppingList.Models;

namespace ShoppingList.Services
{
    public class DataService
    {
        private readonly string categoriesFilePath;
        private readonly string storesFilePath;

        public DataService()
        {
            string appDataPath = FileSystem.AppDataDirectory;
            categoriesFilePath = Path.Combine(appDataPath, "categories.json");
            storesFilePath = Path.Combine(appDataPath, "stores.json");
        }

        public async Task<ObservableCollection<Category>> LoadCategoriesAsync()
        {
            try
            {
                if (File.Exists(categoriesFilePath))
                {
                    string json = await File.ReadAllTextAsync(categoriesFilePath);
                    var categories = JsonSerializer.Deserialize<List<Category>>(json);
                    if (categories != null)
                    {
                        return new ObservableCollection<Category>(categories);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
            }

            return GetDefaultCategories();
        }

        public async Task SaveCategoriesAsync(ObservableCollection<Category> categories)
        {
            try
            {
                string json = JsonSerializer.Serialize(categories, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(categoriesFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving categories: {ex.Message}");
            }
        }

        public async Task<ObservableCollection<Store>> LoadStoresAsync()
        {
            try
            {
                if (File.Exists(storesFilePath))
                {
                    string json = await File.ReadAllTextAsync(storesFilePath);
                    var stores = JsonSerializer.Deserialize<List<Store>>(json);
                    if (stores != null)
                    {
                        return new ObservableCollection<Store>(stores);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stores: {ex.Message}");
            }

            return GetDefaultStores();
        }

        public async Task SaveStoresAsync(ObservableCollection<Store> stores)
        {
            try
            {
                string json = JsonSerializer.Serialize(stores, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(storesFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving stores: {ex.Message}");
            }
        }

        private ObservableCollection<Category> GetDefaultCategories()
        {
            return new ObservableCollection<Category>
            {
                new Category { Name = "Nabiał" },
                new Category { Name = "Warzywa i owoce" },
                new Category { Name = "Mięso i wędliny" },
                new Category { Name = "Pieczywo" },
                new Category { Name = "Napoje" }
            };
        }

        private ObservableCollection<Store> GetDefaultStores()
        {
            return new ObservableCollection<Store>
            {
                new Store { Name = "Biedronka" },
                new Store { Name = "Lidl" },
                new Store { Name = "Kaufland" },
                new Store { Name = "Auchan" },
                new Store { Name = "Carrefour" }
            };
        }
    }
}