using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.ApiClients;  
using WebAPI.DTOs;        

namespace WebApp.Pages.Menu
{
    public class IndexModel : PageModel
    {
        private readonly ProductApiClient _api;
        public IndexModel(ProductApiClient api) => _api = api;

        public List<ProductDto> Products { get; private set; } = new();
        public List<string> Categories { get; private set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                Products = await _api.LoadProductsAsync();

                Categories = Products
                    .Select(p => p.CategoryName)
                    .Distinct()
                    .ToList();
            }
            catch (HttpRequestException ex) 
            {
               Console.WriteLine($"Error fetching products: {ex.Message}");
            }
        }
     


    }
}
