using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize] // Sadece giriş yapmış yetkili yöneticiler erişebilir!
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}