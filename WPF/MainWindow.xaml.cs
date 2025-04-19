using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using WPF.Repositories;
using WPF.Services;
using WPF.Views;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IAuthService _authService;

        public MainWindow()
        {
            InitializeComponent();
            _authService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAuthService>();
        }

    public MainWindow(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                var token = await _authService.LoginAsync(username, password);

                var mainUi = new MainUiWindow(username);
                mainUi.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(((App)Application.Current).ServiceProvider.GetRequiredService<IAuthService>());
            registerWindow.Show();
            Close();
        }
    }
}