using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

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
                Products.Remove(SelectedProduct);
                UpdateStatistics();
            }
        }

        private bool CanRemoveProduct(object parameter)
        {
            return SelectedProduct != null;
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