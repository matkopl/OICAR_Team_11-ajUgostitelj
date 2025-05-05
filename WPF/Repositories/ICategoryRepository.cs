using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Repositories
{
    internal interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync(string token);
    }
}
