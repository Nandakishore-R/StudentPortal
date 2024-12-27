using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentPortal.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View(); // Create a view named AdminDashboard.cshtml
        }

        [Authorize(Roles = "User")]
        public IActionResult UserDashboard()
        {
            return View(); // Create a view named UserDashboard.cshtml
        }
    }
}
