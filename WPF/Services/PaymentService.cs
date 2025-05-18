using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;
using WPF.Repositories;

namespace WPF.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;

        public PaymentService(IPaymentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync(string token)
        {
            return await _repository.GetAllPaymentsAsync(token);
        }

        public async Task<bool> UpdatePaymentAsync(string token, PaymentDto paymentDto)
        {
            return await _repository.UpdatePaymentAsync(token, paymentDto);
        }

        public async Task<bool> DeletePaymentAsync(string token, int paymentId)
        {
            return await _repository.DeletePaymentAsync(token, paymentId);
        }
    }
}
