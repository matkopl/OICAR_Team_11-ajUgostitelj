using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync(string token);
        Task<bool> UpdatePaymentAsync(string token, PaymentDto payment);
        Task<bool> DeletePaymentAsync(string token, int paymentId);
    }
}
