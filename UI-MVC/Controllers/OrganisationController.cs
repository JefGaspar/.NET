using Microsoft.AspNetCore.Mvc;

namespace EM.UI.MVC.Controllers;

public class OrganisationController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}