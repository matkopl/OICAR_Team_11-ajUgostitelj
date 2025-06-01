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
using System.Windows.Threading;
using WebAPI.DTOs;
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
        private readonly DispatcherTimer _timer;
        public PaymentsCrudWindow(string token, IPaymentService paymentService)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _token = token;
            _paymentService = paymentService;

            _timer = new();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += (s, args) => LoadPayments();
            _timer.Start();

            LoadPayments();
        }

        private async void LoadPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync(_token);
            PaymentListView.ItemsSource = payments;
        }

        private void UpdatePayment_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentListView.SelectedItem == null)
            {
                MessageBox.Show("No payment has been selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedPayment = PaymentListView.SelectedItem as PaymentDto;

            var updatePaymentWindow = new UpdatePaymentWindow(_paymentService, _token, selectedPayment);
            updatePaymentWindow.Closed += (s, args) => LoadPayments();
            updatePaymentWindow.Show();
        }

        private async void DeletePayment_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentListView.SelectedItem == null)
            {
                MessageBox.Show("No payment has been selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedPayment = PaymentListView.SelectedItem as PaymentDto;

            try
            {
                var success = await _paymentService.DeletePaymentAsync(_token, selectedPayment.Id);

                if (success)
                {
                    MessageBox.Show("Payment deleted successfully", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadPayments();
                } 
                else
                {
                    MessageBox.Show("Failed to delete payment!", "Delete", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _timer.Stop();
        }
    }
}
