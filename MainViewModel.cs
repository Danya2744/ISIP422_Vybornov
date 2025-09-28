using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Collections.Generic;

namespace ISIP422_Vybornov
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Product> _products;
        private Product _selectedProduct;
        private string _newProductName;
        private decimal _newProductPrice;
        private int _newProductQuantity;
        private string _newProductCategory;
        private int _nextProductId = 1;

        private string _searchCode;
        private string _searchName;
        private string _searchCategory;
        private Product _foundProduct;

        private int _supplyQuantity;
        private int _sellQuantity;

        public MainViewModel()
        {
            Products = new ObservableCollection<Product>();
            Products.CollectionChanged += (s, e) => UpdateStatistics();

            Categories = new ObservableCollection<string>
            {
                "Электроника",
                "Одежда",
                "Продукты питания",
                "Книги",
                "Спорттовары"
            };
            NewProductCategory = Categories.First();
            SearchCategory = Categories.First();

            InitializeSampleProducts();
        }

        private void InitializeSampleProducts()
        {
            var sampleProducts = new List<Product>
            {
                new Product { Code = "1", Name = "Смартфон Samsung", Price = 25000, Quantity = 10, Category = "Электроника" },
                new Product { Code = "2", Name = "Футболка хлопковая", Price = 1500, Quantity = 25, Category = "Одежда" },
                new Product { Code = "3", Name = "Хлеб ржаной", Price = 50, Quantity = 0, Category = "Продукты питания" },
                new Product { Code = "4", Name = "Война и мир", Price = 800, Quantity = 15, Category = "Книги" },
                new Product { Code = "5", Name = "Футбольный мяч", Price = 2000, Quantity = 8, Category = "Спорттовары" }
            };

            foreach (var product in sampleProducts)
            {
                product.PropertyChanged += (s, e) => UpdateStatistics();
                Products.Add(product);
            }

            _nextProductId = 6; 
            UpdateStatistics();
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
                UpdateStatistics();
            }
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
                if (value != null)
                {
                    SearchCode = value.Code;
                    SearchName = value.Name;
                    SearchCategory = value.Category;
                }
            }
        }

        public string NewProductName
        {
            get => _newProductName;
            set
            {
                _newProductName = value;
                OnPropertyChanged();
            }
        }

        public decimal NewProductPrice
        {
            get => _newProductPrice;
            set
            {
                _newProductPrice = value;
                OnPropertyChanged();
            }
        }

        public int NewProductQuantity
        {
            get => _newProductQuantity;
            set
            {
                _newProductQuantity = value;
                OnPropertyChanged();
            }
        }

        public string NewProductCategory
        {
            get => _newProductCategory;
            set
            {
                _newProductCategory = value;
                OnPropertyChanged();
            }
        }

        public string SearchCode
        {
            get => _searchCode;
            set
            {
                _searchCode = value;
                OnPropertyChanged();
            }
        }

        public string SearchName
        {
            get => _searchName;
            set
            {
                _searchName = value;
                OnPropertyChanged();
            }
        }

        public string SearchCategory
        {
            get => _searchCategory;
            set
            {
                _searchCategory = value;
                OnPropertyChanged();
            }
        }

        public Product FoundProduct
        {
            get => _foundProduct;
            set
            {
                _foundProduct = value;
                OnPropertyChanged();
            }
        }

        public int SupplyQuantity
        {
            get => _supplyQuantity;
            set
            {
                _supplyQuantity = value;
                OnPropertyChanged();
            }
        }

        public int SellQuantity
        {
            get => _sellQuantity;
            set
            {
                _sellQuantity = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Categories { get; }

        public string TotalProductsText => $"Общее количество товаров: {Products.Count}";

        public string TotalValueText
        {
            get
            {
                var total = Products.Sum(p => p.TotalValue);
                return $"Общая стоимость: {total:C}";
            }
        }

        public string AvailableProductsText
        {
            get
            {
                var availableCount = Products.Count(p => p.IsAvailable);
                return $"Товаров в наличии: {availableCount}";
            }
        }

        public RelayCommand AddProductCommand => new RelayCommand(AddProduct, CanAddProduct);
        public RelayCommand RemoveProductCommand => new RelayCommand(RemoveProduct, CanRemoveProduct);
        public RelayCommand SupplyProductCommand => new RelayCommand(SupplyProduct, CanSupplyProduct);
        public RelayCommand SellProductCommand => new RelayCommand(SellProduct, CanSellProduct);
        public RelayCommand SearchProductCommand => new RelayCommand(SearchProduct, CanSearchProduct);
        public RelayCommand ClearSearchCommand => new RelayCommand(ClearSearch);

        private void AddProduct(object parameter)
        {
            var product = new Product
            {
                Code = _nextProductId.ToString(), 
                Name = NewProductName,
                Price = NewProductPrice,
                Quantity = NewProductQuantity,
                Category = NewProductCategory
            };

            product.PropertyChanged += (s, e) => UpdateStatistics();
            Products.Add(product);
            _nextProductId++;

            NewProductName = string.Empty;
            NewProductPrice = 0;
            NewProductQuantity = 0;
            NewProductCategory = Categories.First();

            UpdateStatistics();
            MessageBox.Show($"Товар '{product.Name}' добавлен с кодом {product.Code}", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanAddProduct(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewProductName) &&
                   NewProductPrice >= 0 &&
                   NewProductQuantity >= 0;
        }

        private void RemoveProduct(object parameter)
        {
            if (SelectedProduct != null)
            {
                var productName = SelectedProduct.Name;
                Products.Remove(SelectedProduct);
                UpdateStatistics();
                MessageBox.Show($"Товар '{productName}' удален", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanRemoveProduct(object parameter)
        {
            return SelectedProduct != null;
        }

        private void SupplyProduct(object parameter)
        {
            if (SelectedProduct != null && SupplyQuantity > 0)
            {
                SelectedProduct.Quantity += SupplyQuantity;
                UpdateStatistics();
                MessageBox.Show($"Поставка {SupplyQuantity} единиц товара '{SelectedProduct.Name}' выполнена\n" +
                              $"Новое количество: {SelectedProduct.Quantity}", "Поставка выполнена",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                SupplyQuantity = 0;
            }
        }

        private bool CanSupplyProduct(object parameter)
        {
            return SelectedProduct != null && SupplyQuantity > 0;
        }

        private void SellProduct(object parameter)
        {
            if (SelectedProduct != null && SellQuantity > 0)
            {
                if (SelectedProduct.Quantity >= SellQuantity)
                {
                    SelectedProduct.Quantity -= SellQuantity;
                    var totalSale = SelectedProduct.Price * SellQuantity;
                    UpdateStatistics();
                    MessageBox.Show($"Продано {SellQuantity} единиц товара '{SelectedProduct.Name}'\n" +
                                  $"Выручка: {totalSale:C}\n" +
                                  $"Остаток: {SelectedProduct.Quantity}", "Продажа выполнена",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    SellQuantity = 0;
                }
                else
                {
                    MessageBox.Show($"Недостаточно товара на складе!\n" +
                                  $"Доступно: {SelectedProduct.Quantity}, Запрошено: {SellQuantity}",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private bool CanSellProduct(object parameter)
        {
            return SelectedProduct != null && SellQuantity > 0;
        }

        private void SearchProduct(object parameter)
        {
            var foundProducts = Products.Where(p =>
                (string.IsNullOrWhiteSpace(SearchCode) || p.Code.Contains(SearchCode)) &&
                (string.IsNullOrWhiteSpace(SearchName) || p.Name.Contains(SearchName)) &&
                (string.IsNullOrWhiteSpace(SearchCategory) || p.Category == SearchCategory)
            ).ToList();

            if (foundProducts.Any())
            {
                if (foundProducts.Count == 1)
                {
                    FoundProduct = foundProducts.First();
                    SelectedProduct = FoundProduct;
                    MessageBox.Show($"Найден товар:\nКод: {FoundProduct.Code}\nНазвание: {FoundProduct.Name}\n" +
                                  $"Цена: {FoundProduct.Price:C}\nКоличество: {FoundProduct.Quantity}\n" +
                                  $"Категория: {FoundProduct.Category}\nВ наличии: {(FoundProduct.IsAvailable ? "Да" : "Нет")}",
                                  "Результат поиска", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var productList = string.Join("\n", foundProducts.Select(p => $"- {p.Name} (код: {p.Code})"));
                    MessageBox.Show($"Найдено {foundProducts.Count} товаров:\n{productList}",
                                  "Результат поиска", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                FoundProduct = null;
                MessageBox.Show("Товары по указанным критериям не найдены", "Результат поиска",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanSearchProduct(object parameter)
        {
            return !string.IsNullOrWhiteSpace(SearchCode) ||
                   !string.IsNullOrWhiteSpace(SearchName) ||
                   !string.IsNullOrWhiteSpace(SearchCategory);
        }

        private void ClearSearch(object parameter)
        {
            SearchCode = string.Empty;
            SearchName = string.Empty;
            SearchCategory = Categories.First();
            FoundProduct = null;
        }

        private void UpdateStatistics()
        {
            OnPropertyChanged(nameof(TotalProductsText));
            OnPropertyChanged(nameof(TotalValueText));
            OnPropertyChanged(nameof(AvailableProductsText));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}