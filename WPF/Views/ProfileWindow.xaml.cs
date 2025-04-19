using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private readonly string _username;
        private readonly string _token;
        private readonly IAuthService _authService;
        public ProfileWindow(string username, string token, IAuthService authService)
        {
            InitializeComponent();
            _username = username;
            _token = token;
            _authService = authService;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            LoadProfileDetails();
        }

        private async void LoadProfileDetails()
        {
            try
            {
                var userDetails = await _authService.GetUserDetailsAsync(_username, _token);

                if (userDetails != null)
                {
                    UsernameText.Text = userDetails.Username;
                    EmailText.Text = userDetails.Email;
                    RoleText.Text = userDetails.Role;
                }
                else
                {
                    MessageBox.Show("Failed to load user details");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var changePasswordWindow = new ChangePasswordWindow(_username, _token, _authService);
            changePasswordWindow.Show();
        }
    }
}
