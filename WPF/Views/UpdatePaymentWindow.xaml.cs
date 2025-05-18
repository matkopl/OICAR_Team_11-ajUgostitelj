using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for UpdatePaymentWindow.xaml
    /// </summary>
    public partial class UpdatePaymentWindow : Window
    {
        private readonly string _token;
        private readonly IPaymentService _paymentService;
        private readonly int _paymentId;
        private readonly int _orderId;

        public UpdatePaymentWindow(IPaymentService paymentService, string token, PaymentDto payment)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _paymentService = paymentService;
            _token = token;
            _paymentId = payment.Id;
            _orderId = payment.OrderId;

            AmountTextBox.Text = payment.Amount.ToString();
            MethodTextBox.Text = payment.Method;
            PaymentDateTextBox.Text = payment.PaymentDate.ToString("dd-MM-yyyy");
        }

        private async void UpdatePayment_Click(object sender, RoutedEventArgs e)
        {
            var updatedPayment = new PaymentDto
            {
                Id = _paymentId,
                Amount = decimal.Parse(AmountTextBox.Text),
                Method = MethodTextBox.Text,
                PaymentDate = DateTime.ParseExact(PaymentDateTextBox.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToUniversalTime(),
                OrderId = _orderId
            };

            try
            {
                var success = await _paymentService.UpdatePaymentAsync(_token, updatedPayment);

                if (success)
                {
                    MessageBox.Show("Payment updated successfully!", "Update", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {

                    MessageBox.Show("Failed to update payment!", "Update", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
