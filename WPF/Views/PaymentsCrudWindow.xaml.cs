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
    /// Interaction logic for PaymentsCrudWindow.xaml
    /// </summary>
    public partial class PaymentsCrudWindow : Window
    {
        private readonly string _token;
        private readonly IPaymentService _paymentService;
        public PaymentsCrudWindow(string token, IPaymentService paymentService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _token = token;
            _paymentService = paymentService;

            LoadPayments();
        }

        private async void LoadPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync(_token);
            PaymentListBox.ItemsSource = payments;
            PaymentListBox.DisplayMemberPath = "Method";
        }

        private void CreatePayment_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("create");
        }

        private void UpdatePayment_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("update");
        }

        private void DeletePayment_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("delete");
        }
    }
}
