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
        private readonly string _username;
        private readonly string _token;
        public MainUiWindow(string username, string token)
        {
            InitializeComponent();
            _username = username;
            _token = token;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            WelcomeMessage.Text = $"Welcome, {username}!";

            var userRole = JwtDecodeService.GetUserRole(token);
            AdminPanelButton.Visibility = userRole.Equals("Admin") ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var authService =  ((App)Application.Current).ServiceProvider.GetRequiredService<IAuthService>();
            var profileWindow = new ProfileWindow(_username, _token, authService);
            profileWindow.Show();
        }

        private void AdminPanelButton_Click(object sender, RoutedEventArgs e)
        {
            var authService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAuthService>();
            var adminPanel = new AdminPanelWindow(_token, authService);
            adminPanel.Show();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new OrdersWindow(_token);
            ordersWindow.Show();
        }
    }
}
