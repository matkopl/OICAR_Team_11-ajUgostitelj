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

namespace WPF.Views
{
    /// <summary>
    /// Interaction logic for MainUiWindow.xaml
    /// </summary>
    public partial class MainUiWindow : Window
    {
        private readonly string _username;
        public MainUiWindow(string username)
        {
            InitializeComponent();
            _username = username;

            WelcomeMessage.Text = $"Welcome, {username}!";
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
