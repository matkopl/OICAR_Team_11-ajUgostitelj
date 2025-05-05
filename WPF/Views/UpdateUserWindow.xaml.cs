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
    /// Interaction logic for UpdateUserWindow.xaml
    /// </summary>
    public partial class UpdateUserWindow : Window
    {
        private readonly string _token;
        private readonly IUserService _userService;
        private readonly int _userId;

        public UpdateUserWindow(string token, IUserService userService, UserDto user)
        {
            InitializeComponent();
            _token = token;
            _userService = userService;
            _userId = user.Id;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            UsernameTextBox.Text = user.Username;
            EmailTextBox.Text = user.Email;

            if (user.Role == "Admin")
            {
                AdminRoleButton.IsChecked = true;
            }
            else
            {
                UserRoleButton.IsChecked = true;
            }
        }

        private async void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            int selectedRoleId = AdminRoleButton.IsChecked == true ? 1 : 2;

            var updatedUser = new UpdateUserDto
            {
                Id = _userId,
                Username = UsernameTextBox.Text,
                Email = EmailTextBox.Text,
                RoleId = selectedRoleId
            };

            try
            {
                var success = await _userService.UpdateUserAsync(_token, updatedUser);

                if (success)
                {
                    MessageBox.Show($"User {updatedUser.Username} has been updated successfully!", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed to update user!", "Update", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
