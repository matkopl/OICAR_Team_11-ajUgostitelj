using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Product
{
    public class DetailsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }  // Ovo će biti null dok ne povežete s bazom

        public void OnGet()
        {
            // Za sada ne radimo ništa s ID-om
            // Kasnije ćete ovdje dohvatiti proizvod iz baze
        }
    }
}