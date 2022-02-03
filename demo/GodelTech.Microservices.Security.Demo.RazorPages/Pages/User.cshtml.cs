using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GodelTech.Microservices.Security.Demo.RazorPages.Pages
{
    [Authorize]
    public class UserModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}
