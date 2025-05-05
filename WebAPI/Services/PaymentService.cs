using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public PaymentService(IRepositoryFactory repositoryFactory, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
        }

        public async Task<PaymentDto> CreatePaymentAsync(PaymentDto paymentDto)
        {
            var paymentsRepo = _repositoryFactory.GetRepository<Payment>();
            var newPayment = _mapper.Map<Payment>(paymentDto);

            await paymentsRepo.AddAsync(newPayment);
            await paymentsRepo.SaveChangesAsync();

            return _mapper.Map<PaymentDto>(newPayment);
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            var paymentsRepo = _repositoryFactory.GetRepository<Payment>();
            var payment = await paymentsRepo.GetByIdAsync(id);

            if (payment == null)
            {
                return false;
            }

            paymentsRepo.Remove(payment);
            await paymentsRepo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync()
        {
            var paymentsRepo =  _repositoryFactory.GetRepository<Payment>();
            var payments = await paymentsRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto?>? GetPaymentByIdAsync(int id)
        {
            var paymentsRepo = _repositoryFactory.GetRepository<Payment>();
            var payment = paymentsRepo.GetByIdAsync(id);
            return payment != null ? _mapper.Map<PaymentDto>(payment) : null;
        }

        public async Task<bool> UpdatePaymentAsync(PaymentDto paymentDto)
        {
            var paymentsRepo = _repositoryFactory.GetRepository<Payment>();
            var payment = await paymentsRepo.GetByIdAsync(paymentDto.Id);

            if (payment == null)
            {
                return false;
            }

            _mapper.Map(paymentDto, payment);
            paymentsRepo.Update(payment);
            await paymentsRepo.SaveChangesAsync();

            return true;
        }
    }
}
