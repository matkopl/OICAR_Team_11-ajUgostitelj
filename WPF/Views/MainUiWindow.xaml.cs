using Microsoft.AspNetCore.SignalR.Client;
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
using WPF.Services;

namespace WPF.Views
{
    /// <summary>
    /// Interaction logic for MainUiWindow.xaml
    /// </summary>
    public partial class MainUiWindow : Window
    {
        private HubConnection _hubConnection;

        private readonly string _username;
        private readonly string _token;
        public MainUiWindow(string username, string token)
        {
            InitializeComponent();
            _username = username;
            _token = token;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5207/orderHub")
                .Build();

            _hubConnection.On<int>("ReceiveOrderNotification", (orderId) =>
            {
                var result = MessageBox.Show($"New order received! Order ID: {orderId}\nWould you like to view details?",
                                 "New Order", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    Dispatcher.Invoke(() => OpenOrderDetails(orderId));
                }
            });

            _hubConnection.On<int, string>("ReceiveOrderStatusUpdate", (orderId, newStatus) =>
            {
                MessageBox.Show($"Order: {orderId} is now {newStatus}", "Order status update", MessageBoxButton.OK, MessageBoxImage.Information);
            });

            _hubConnection.StartAsync();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            WelcomeMessage.Text = $"Welcome, {username}!";

            var userRole = JwtDecodeService.GetUserRole(token);
            AdminPanelButton.Visibility = userRole.Equals("Admin") ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OpenOrderDetails(int orderId)
        {
            var ordersWindow = new OrdersWindow(_token);
            ordersWindow.Show();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var authService =  ((App)Application.Current).ServiceProvider.GetRequiredService<IAuthService>();
            var userService = ((App)Application.Current).ServiceProvider.GetRequiredService<IUserService>();
            var profileWindow = new ProfileWindow(_username, _token, authService, userService);
            profileWindow.Show();
        }

        private void AdminPanelButton_Click(object sender, RoutedEventArgs e)
        {
            var authService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAuthService>();
            var userService = ((App)Application.Current).ServiceProvider.GetService<IUserService>();
            var adminPanel = new AdminPanelWindow(_token, _username, authService, userService);
            adminPanel.Show();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new OrdersWindow(_token);
            ordersWindow.Show();
        }
    }
}
