using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WebAPI.DTOs;
using Microsoft.Extensions.DependencyInjection;
using WPF.Repositories;

namespace WPF.Views
{
    public partial class OrdersWindow : Window
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly string _token;

        // Wrapper
        private class OrderDisplay
        {
            public OrderDto Order { get; set; } = default!;
            public IEnumerable<OrderItemDto> Items { get; set; } = Array.Empty<OrderItemDto>();
        }

        public OrdersWindow(string token)
        {
            InitializeComponent();
            _token = token;

            var sp = ((App)Application.Current).ServiceProvider;
            _orderRepo = sp.GetRequiredService<IOrderRepository>();
            _productRepo = sp.GetRequiredService<IProductRepository>();

            Loaded += async (_, __) => await LoadAllAsync();
        }

        private async Task LoadAllAsync()
        {
            try
            {
                // 1) Dohvati sve proizvode jednom
                var products = await _productRepo.GetAllAsync(_token);
                var prodDict = products.ToDictionary(p => p.Id, p => p);

                // 2) Dohvati sve narudžbe
                var orders = await _orderRepo.GetAllAsync(_token);

                var displayList = new List<OrderDisplay>();
                foreach (var o in orders)
                {
                    // 3) Dohvati stavke
                    var items = (await _orderRepo.GetItemsAsync(_token, o.Id)).ToList();

                    // 4) Spoji ih s proizvodima
                    foreach (var it in items)
                    {
                        if (prodDict.TryGetValue(it.ProductId, out var prod))
                        {
                            it.ProductName = prod.Name;
                            it.UnitPrice = prod.Price;
                        }
                    }

                    displayList.Add(new OrderDisplay { Order = o, Items = items });
                }

                // 5) Pokaži
                dgOrders.ItemsSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju:\n{ex.Message}",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
