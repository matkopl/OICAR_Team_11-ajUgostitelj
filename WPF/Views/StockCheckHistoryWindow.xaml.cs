using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for StockCheckHistoryWindow.xaml
    /// </summary>
    public partial class StockCheckHistoryWindow : Window
    {
        private readonly string _token;
        private readonly IInventoryService _inventoryService;

        public StockCheckHistoryWindow(string token, IInventoryService inventoryService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _token = token;
            _inventoryService = inventoryService;

            LoadStockCheckHistory();
        }

        private async void LoadStockCheckHistory()
        {
            var stockChecks = await _inventoryService.GetStockCheckHistoryAsync(_token);
            StockCheckHistoryListView.ItemsSource = stockChecks;
        }

        private async void PerformStockCheck_Click(object sender, RoutedEventArgs e)
        {
            var inventoryItems = await _inventoryService.GetAllInventoriesAsync(_token);
            var productRepo = ((App)Application.Current).ServiceProvider.GetRequiredService<IProductRepository>();
            var productService = new ProductService(productRepo, _token);

            var stockCheckItems = new List<StockCheckDto>();

            foreach (var inventory in inventoryItems)
            {
                var product = await productService.GetProductByIdAsync(inventory.ProductId);

                stockCheckItems.Add(new StockCheckDto
                {
                    ProductId = inventory.ProductId,
                    ProductName = inventory.ProductName,
                    RecordedQuantity = inventory.Quantity,
                    ActualQuantity = product != null ? product.Quantity : inventory.Quantity
                });
            }


            var success = await _inventoryService.PerformStockCheckAsync(_token, stockCheckItems);

            if (success)
            {
                MessageBox.Show("Stock check recorded successfully!", "Stock Check", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadStockCheckHistory(); 
            }
            else
            {
                MessageBox.Show("Failed to record stock check!", "Stock Check", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ClearStockCheckHistory_Click(object sender, RoutedEventArgs e)
        {
            var success = await _inventoryService.ClearStockCheckHistoryAsync(_token);

            if (success) 
            {
                MessageBox.Show("Stock check history cleared successfully!", "Stock Check", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadStockCheckHistory();
            }
            else
            {
                MessageBox.Show("Failed to clear stock check history!", "Stock Check", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExportStockCheckToCsv_Click(object sender, RoutedEventArgs e)
        {
            var stockChecks = await _inventoryService.GetStockCheckHistoryAsync(_token);
            if (stockChecks == null || !stockChecks.Any())
            {
                MessageBox.Show("No stock check history found!", "Export CSV", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Product ID,Product name,Recorded quantity,Actual quantity, Difference");

            foreach (var s in stockChecks)
            {
                sb.AppendLine($"{s.ProductId}, {s.ProductName}, {s.RecordedQuantity},{s.ActualQuantity}, {s.Difference}");
            }

            var saveFileDialog = new SaveFileDialog
            {
                FileName = $"Inventura {DateTime.Now:dd.MM.yyyy}",
                DefaultExt = ".csv",
                Filter = "CSV files (*.csv)|*.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, sb.ToString());
                MessageBox.Show("Stock check exported successfully!", "CSV Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
