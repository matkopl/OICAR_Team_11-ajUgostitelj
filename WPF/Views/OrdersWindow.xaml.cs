using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.DTOs;
using WPF.Repositories;

namespace WPF.Views
{
    public partial class OrdersWindow : Window
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly ITableRepository _tableRepo;
        private readonly string _token;
        private readonly DispatcherTimer _refreshTimer;
        private int? _selectedOrderId; 

        private class OrderDisplay
        {
            public OrderDto Order { get; set; } = default!;
            public string TableName { get; set; } = "";
            public IEnumerable<OrderItemDto> Items { get; set; } = Array.Empty<OrderItemDto>();
        }

        public OrdersWindow(string token)
        {
            InitializeComponent();
            _token = token;

            var sp = ((App)Application.Current).ServiceProvider;
            _orderRepo = sp.GetRequiredService<IOrderRepository>();
            _productRepo = sp.GetRequiredService<IProductRepository>();
            _tableRepo = sp.GetRequiredService<ITableRepository>();

            
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _refreshTimer.Tick += async (_, __) => await LoadAllAsync();
            _refreshTimer.Start();

            Loaded += async (_, __) => await LoadAllAsync();
        }


        private async Task LoadAllAsync()
        {
            try
            {
                _selectedOrderId = (dgOrders.SelectedItem as OrderDisplay)?.Order.Id; 

                var products = await _productRepo.GetAllAsync(_token);
                var prodDict = products.ToDictionary(p => p.Id, p => p);

                var tables = await _tableRepo.GetAllAsync(_token);
                var tableDict = tables.ToDictionary(t => t.Id, t => t.Name);

                var orders = await _orderRepo.GetAllAsync(_token);
                orders = orders.OrderByDescending(o => o.OrderDate).ToList();

                var displayList = new List<OrderDisplay>();
                foreach (var o in orders)
                {
                    var items = (await _orderRepo.GetItemsAsync(_token, o.Id)).ToList();

                    foreach (var it in items)
                    {
                        if (prodDict.TryGetValue(it.ProductId, out var prod))
                        {
                            it.ProductName = prod.Name;
                            it.UnitPrice = prod.Price;
                        }
                    }

                    displayList.Add(new OrderDisplay
                    {
                        Order = o,
                        TableName = tableDict.GetValueOrDefault(o.TableId, $"# {o.TableId}"),
                        Items = items
                    });
                }

                dgOrders.ItemsSource = displayList; 

                if (_selectedOrderId.HasValue)
                {
                    SelectOrder(_selectedOrderId.Value); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška pri učitavanju: {ex.Message}");
            }
        }


        private void OpenOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem is not OrderDisplay selectedOrder)
            {
                MessageBox.Show("Odaberite narudžbu prvo!", "Upozorenje",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var orderDetailsWindow = new OrderDetailsWindow(_token, selectedOrder.Order.Id);
            orderDetailsWindow.Show();
        }

        
        private async void BtnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            
            if ((sender as FrameworkElement)?.DataContext is not OrderDisplay od)
                return;

            var result = MessageBox.Show(
                $"Obrisati narudžbu za stol '{od.TableName}'?\nDatum: {od.Order.OrderDate:dd.MM.yyyy HH:mm}",
                "Potvrdi brisanje",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                
                await _orderRepo.DeleteAsync(_token, od.Order.Id);

                
                await LoadAllAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri brisanju narudžbe:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SelectOrder(int orderId)
        {
            var selectedOrder = dgOrders.Items.Cast<OrderDto>().FirstOrDefault(o => o.Id == orderId);

            if (selectedOrder != null)
            {
                dgOrders.SelectedItem = selectedOrder;
                dgOrders.ScrollIntoView(selectedOrder);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _refreshTimer.Stop();
            base.OnClosed(e);
        }
    }
}
