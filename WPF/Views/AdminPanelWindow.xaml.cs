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
    /// Interaction logic for AdminPanelWindow.xaml
    /// </summary>
    public partial class AdminPanelWindow : Window
    {
        private readonly string _token;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly string _username;

        public AdminPanelWindow(string token, string username, IAuthService authService, IUserService userService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _username = username;
            _token = token;
            _authService = authService;
            _userService = userService;
        }

        private void OpenUserCrud_Click(object sender, RoutedEventArgs e)
        {
            var userCrudWindow = new UserCrudWindow(_token, _userService, _username);
            userCrudWindow.Show();
        }

        private void OpenProductCrud_Click(object sender, RoutedEventArgs e)
        {
            var productCrudWindow = new ProductWindow(_token);
            productCrudWindow.Show();
        }

        private void OpenTableCrud_Click(object sender, RoutedEventArgs e)
        {
            var tableCrudWindow = new TablesWindow(_token);
            tableCrudWindow.Show();
        }

        private void OpenPaymentsCrud_Click(object sender, RoutedEventArgs e)
        {
            var _paymentsService = ((App)Application.Current).ServiceProvider.GetRequiredService<IPaymentService>();
            var paymentsCrudWindow = new PaymentsCrudWindow(_token, _paymentsService);
            paymentsCrudWindow.Show();
        }

        private void OpenInventoryCrud_Click(object sender, RoutedEventArgs e)
        {
            var _inventoryService = ((App)Application.Current).ServiceProvider.GetRequiredService<IInventoryService>();
            var inventoryCrudWindow = new InventoryCrudWindow(_token, _inventoryService);
            inventoryCrudWindow.Show();
        }
    }
}
