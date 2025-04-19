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
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private readonly string _username;
        private readonly string _token;
        private readonly IAuthService _authService;
        public ChangePasswordWindow(string username, string token, IAuthService authService)
        {
            InitializeComponent();
            _username = username;
            _token = token;
            _authService = authService;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private async void SubmitChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentPassword = CurrentPasswordBox.Password;
                var newPassword = NewPasswordBox.Password;

                if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
                {
                    MessageBox.Show("Both fields are required.");
                    return;
                }

                ChangePasswordDto changePasswordDto = new()
                {
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword
                };

                var success = await _authService.ChangePasswordAsync(_token, changePasswordDto);
                if (success)
                {
                    MessageBox.Show("Password changed succesfully.");
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed to change password, please check your entries and try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while changing password: {ex.Message}");
            }
        }
    }
}
