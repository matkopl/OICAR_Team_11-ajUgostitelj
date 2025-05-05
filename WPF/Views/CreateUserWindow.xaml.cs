using System.Windows;
using System.Windows.Controls;
using WebAPI.DTOs;
using WPF.Services;

namespace WPF.Views
{
    /// <summary>
    /// Interaction logic for CreateUserWindow.xaml
    /// </summary>
    public partial class CreateUserWindow : Window
    {
        private readonly string _token;
        private readonly IUserService _userService;

        public CreateUserWindow(string token, IUserService userService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _token = token;
            _userService = userService;
        }

        private async void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            int selectedRoleId = AdminRoleButton.IsChecked == true ? 1 : 2;

            var newUser = new CreateUserDto
            {
                Username = UsernameTextBox.Text,
                Email = EmailTextBox.Text,
                Password = PasswordBox.Password,
                RoleId = selectedRoleId
            };

            try
            {
                var success = await _userService.CreateUserAsync(_token, newUser);

                if (success)
                {
                    MessageBox.Show($"User {newUser.Username} has been created successfully!", "Create", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed to create user!", "Create", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
