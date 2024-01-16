using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RSOAdMicroservice.Pages
{
    public class ShowUIModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int AdId { get; set; }

        public void OnGet()
        {
        }
    }
}
