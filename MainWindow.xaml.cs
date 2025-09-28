using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace ISIP422_Vybornov
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void IntegerValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");

            TextBox textBox = sender as TextBox;
            string currentText = textBox.Text + e.Text;

            e.Handled = regex.IsMatch(e.Text) || !IsValidDecimal(currentText);
        }

        private bool IsValidDecimal(string text)
        {
            return decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out _);
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                Regex regex = new Regex("[^0-9.,]+");
                if (regex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }

    public class TotalValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<Product> products && products != null)
            {
                try
                {
                    var total = products.Sum(p => p.TotalValue);
                    return $"Общая стоимость: {total:C}";
                }
                catch
                {
                    return "Общая стоимость: 0 руб.";
                }
            }
            return "Общая стоимость: 0 руб.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AvailableProductsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<Product> products && products != null)
            {
                try
                {
                    var availableCount = products.Count(p => p.IsAvailable);
                    return $"Товаров в наличии: {availableCount}";
                }
                catch
                {
                    return "Товаров в наличии: 0";
                }
            }
            return "Товаров в наличии: 0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}