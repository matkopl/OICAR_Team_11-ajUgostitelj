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
        private readonly IUserService _userService;

        private int _userId;
        private bool _isLoading = false;
        public ProfileWindow(string username, string token, IAuthService authService, IUserService userService)
        {
            InitializeComponent();
            _username = username;
            _token = token;
            _authService = authService;
            _userService = userService;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            LoadProfileDetails();
        }

        private async void LoadProfileDetails()
        {
            try
            {
                _isLoading = true;
                var userDetails = await _userService.GetUserByUsernameAsync(_username, _token);

                if (userDetails != null)
                {
                    UsernameText.Text = userDetails.Username;
                    EmailText.Text = userDetails.Email;
                    RoleText.Text = userDetails.Role;
                    _userId = userDetails.Id;

                    AnonCheckBox.IsChecked = userDetails.IsAnonymized;
                    AnonCheckBox.IsEnabled = !userDetails.IsAnonymized;
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
            finally
            {
                _isLoading = false;
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var changePasswordWindow = new ChangePasswordWindow(_username, _token, _authService);
            changePasswordWindow.Show();
        }

        private async void AnonCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_isLoading)
                return;

            var result = MessageBox.Show(
                "Jeste li sigurni da želite anonimizirati svoj profil? Ova radnja je nepovratna.",
                "Potvrda anonimizacije",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var apiResult = await _userService.AnonymizeUserAsync(_token, _userId);

                if (apiResult)
                {
                    var messageResult = MessageBox.Show(
                        "Profil je uspješno anonimiziran! Bit ćete odjavljeni iz aplikacije.",
                        "Anonimizacija uspješna",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    if (messageResult == MessageBoxResult.OK)
                    {
                        Application.Current.Shutdown();
                    }

                }
                else
                {
                    MessageBox.Show("Greška pri anonimizaciji!");
                    AnonCheckBox.IsChecked = false;
                }
            }
            else
            {
                AnonCheckBox.IsChecked = false;
            }
        }


        private void AnonCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_isLoading)
            {
                MessageBox.Show("Anonimizacija je nepovratna i ne može se poništiti.");
                AnonCheckBox.IsChecked = true;
            }
        }
    }
}
