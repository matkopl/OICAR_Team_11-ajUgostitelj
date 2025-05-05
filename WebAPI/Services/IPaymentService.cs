using WebAPI.DTOs;

namespace WebAPI.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync();
        Task<PaymentDto?> GetPaymentByIdAsync(int id);
        Task<PaymentDto> CreatePaymentAsync(PaymentDto paymentDto);
        Task<bool> UpdatePaymentAsync(PaymentDto paymentDto);
        Task<bool> DeletePaymentAsync(int id);
    }
}
