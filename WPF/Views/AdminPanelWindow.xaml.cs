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

        public AdminPanelWindow(string token, IAuthService authService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _token = token;
            _authService = authService;
        }

        private void OpenUserCrud_Click(object sender, RoutedEventArgs e)
        {
            //var userCrudWindow = new UserCrudWindow(_token, _authService);
            //userCrudWindow.Show();
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

    }
}
