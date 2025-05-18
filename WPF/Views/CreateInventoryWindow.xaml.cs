using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WebAPI.DTOs;
using WPF.Services;

namespace WPF.Views
{
    /// <summary>
    /// Interaction logic for CreateInventoryWindow.xaml
    /// </summary>
    public partial class CreateInventoryWindow : Window
    {
        private readonly string _token;
        private readonly IInventoryService _inventoryService;
        private readonly IProductService _productService;
        public CreateInventoryWindow(string token, IInventoryService inventoryService, IProductService productService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _token = token;
            _inventoryService = inventoryService;
            _productService = productService;

            LoadAvailableProducts();
        }

        private async void LoadAvailableProducts()
        {
            var allProducts = await _productService.GetAllProductsAsync();
            var inventoryItems = await _inventoryService.GetAllInventoriesAsync(_token);

            var productsNotInInventory = allProducts.Where(p => !inventoryItems.Any(i => i.ProductId == p.Id)).ToList();

            ProductComboBox.ItemsSource = productsNotInInventory;
            ProductComboBox.DisplayMemberPath = "Name";
        }

        private async void AddInventory_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem == null)
            {
                MessageBox.Show("No product is selected!", "Create", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out var quantity))
            {
                MessageBox.Show("Please enter a valid quantity!", "Create", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var selectedProduct = ProductComboBox.SelectedItem as ProductDto;

            var newInventory = new InventoryDto
            {
                ProductId = selectedProduct.Id,
                LastUpdated = DateTime.UtcNow,
                Quantity = quantity,
            };

            try
            {
                var success = await _inventoryService.AddProductToInventoryAsync(_token, newInventory);

                if (success)
                {
                    MessageBox.Show("Inventory added successfully!", "Create", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show($"Failed to add inventory!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
