using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Repositories
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync(string token);
    }
}
