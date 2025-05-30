using Microsoft.Extensions.DependencyInjection;
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
using WPF.Repositories;
using WPF.Services;

namespace WPF.Views
{
    /// <summary>
    /// Interaction logic for InventoryCrudWindow.xaml
    /// </summary>
    public partial class InventoryCrudWindow : Window
    {
        private readonly string _token;
        private readonly IInventoryService _inventoryService;
        public InventoryCrudWindow(string token, IInventoryService inventoryService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _token = token;
            _inventoryService = inventoryService;

            LoadInventory();
        }

        private async void LoadInventory()
        {
            var inventories = await _inventoryService.GetAllInventoriesAsync(_token);

            InventoryListView.ItemsSource = inventories;
        }

        private void CreateInventory_Click(object sender, RoutedEventArgs e)
        {
            var productRepo = ((App)Application.Current).ServiceProvider.GetRequiredService<IProductRepository>();
            var _productService = new ProductService(productRepo, _token);
            var createInventoryWindow = new CreateInventoryWindow(_token, _inventoryService, _productService);

            createInventoryWindow.Closed += (s, args) => LoadInventory();
            createInventoryWindow.Show();
        }

        private async void UpdateInventory_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryListView.SelectedItem is null)
            {
                MessageBox.Show("Please select an inventory item to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedInventory = InventoryListView.SelectedItem as InventoryDto;
            var newQuantity = int.Parse(Microsoft.VisualBasic.Interaction.InputBox("Enter new quantity:", "Update Inventory", selectedInventory.Quantity.ToString()));

            selectedInventory.Quantity = newQuantity;

            var success = await _inventoryService.UpdateInventoryAsync(_token, selectedInventory);

            if (success)
            {
                MessageBox.Show("Inventory updated successfully!", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadInventory();
            }
            else
            {
                MessageBox.Show("Failed to update inventory!", "Update", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteInventory_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryListView.SelectedItem is null)
            {
                MessageBox.Show("Please select an inventory item to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedInventory = InventoryListView.SelectedItem as InventoryDto;

            var success = await _inventoryService.DeleteInventoryAsync(_token, selectedInventory.Id);

            if (success)
            {
                MessageBox.Show("Inventory item deleted successfully!", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadInventory();
            }
            else
            {
                MessageBox.Show("Failed to delete inventory item!", "Delete", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PerformStockCheck_Click(object sender, RoutedEventArgs e)
        {
            var stockCheckWindow = new StockCheckHistoryWindow(_token, _inventoryService);
            stockCheckWindow.Show();
        }
    }
}
