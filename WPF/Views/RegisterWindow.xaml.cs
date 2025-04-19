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
using WPF.Services;

namespace WPF.Views
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly IAuthService _authService;


        public RegisterWindow(IAuthService authService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _authService = ((App)Application.Current).ServiceProvider.GetRequiredService<IAuthService>();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (!password.Equals(confirmPassword))
            {
                MessageBox.Show("Passwords do not match!");
            }

            RegisterDto registerDto = new()
            {
                Username = username,
                Email = email,
                Password = password
            };

            try
            {
                var success = await _authService.RegisterAsync(registerDto);

                if (success)
                {
                    MessageBox.Show($"Registration successful, you can now login.");

                    var loginWindow = new MainWindow();
                    loginWindow.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Registration failed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
