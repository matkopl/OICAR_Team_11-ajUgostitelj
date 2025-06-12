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
using WebAPI;
using WebAPI.DTOs;
using WPF.Repositories;
using WPF.Services;

namespace WPF.Views
{
    /// <summary>
    /// Interaction logic for OrderDetailsWindow.xaml
    /// </summary>
    public partial class OrderDetailsWindow : Window
    {
        private readonly IOrderService _orderService;
        private readonly string _token;
        private readonly int _orderId;
        public OrderDetailsWindow(string token, int orderId)
        {
            InitializeComponent();
            _token = token;
            _orderId = orderId;
            var orderRepo = ((App)Application.Current).ServiceProvider.GetRequiredService<IOrderRepository>();
            _orderService = new OrderService(orderRepo, _token);

            Loaded += async (_, __) => await LoadOrderDetailsAsync();
        }

        private async Task LoadOrderDetailsAsync()
        {
            var order = await _orderService.GetByIdAsync(_orderId);
            OrderIdText.Text = order.Id.ToString();
            StatusComboBox.ItemsSource = new[] { "Pending", "InProgress", "Completed", "Cancelled", "Paid" };
            StatusComboBox.SelectedItem = order.Status.ToString();
            NotesTextBox.Text = order.Notes;
        }

        private async void UpdateStatus_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (!Enum.TryParse<OrderStatus>(StatusComboBox.SelectedItem.ToString(), out var newStatus))
            {
                MessageBox.Show("Invalid status selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var notes = NotesTextBox.Text;

            var orderStatusDto = new OrderStatusDto
            {
                OrderId = _orderId,
                Status = newStatus,
                Notes = string.IsNullOrEmpty(notes) ? "No additional notes" : notes
            };

            var success = await _orderService.UpdateOrderStatusAsync(orderStatusDto);

            if (success)
            {
                MessageBox.Show($"Order {_orderId} updated to {newStatus}!\nNotes: {notes}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Failed to update order status!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
