using System.Windows;
using WebAPI.DTOs;
using WPF.Services;

namespace WPF.Views
{
    /// <summary>
    /// Interaction logic for UserCrudWindow.xaml
    /// </summary>
    public partial class UserCrudWindow : Window
    {
        private readonly string _token;
        private readonly string _username;
        private readonly IUserService _userService;

        public UserCrudWindow(string token, IUserService userService, string username)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _token = token;
            _userService = userService;
            _username = username;

            LoadUsers();
        }

        private async void LoadUsers()
        {
            var users = await _userService.GetAllUsersAsync(_token);
            var filteredUsers = users.Where(user => user.Username != _username);

            UserListBox.ItemsSource = filteredUsers;
            UserListBox.DisplayMemberPath = "Username";
        }

        private async void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            var createUserWindow = new CreateUserWindow(_token, _userService);
            createUserWindow.Closed += (s, args) => LoadUsers();
            createUserWindow.Show();
        }

        private async void UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserListBox.SelectedItem is null)
            {
                MessageBox.Show("No user is selected", "Select a user", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedUser = UserListBox.SelectedItem as UserDto;

            if (selectedUser is null)
            {
                MessageBox.Show("Failed to get selected user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var updateUserWindow = new UpdateUserWindow(_token, _userService, selectedUser);
            updateUserWindow.Closed += (s, args) => LoadUsers();
            updateUserWindow.Show();
        }

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserListBox.SelectedItem is null)
            {
                MessageBox.Show("No user is selected", "Select a user", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedUser = UserListBox.SelectedItem as UserDto;

            try
            {
                var success = await _userService.DeleteUserAsync(_token, selectedUser.Id);

                if (success)
                {
                    MessageBox.Show("User deleted successfully!", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUsers();
                } 
                else
                {
                    MessageBox.Show("Failed to delete user!", "Delete", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
