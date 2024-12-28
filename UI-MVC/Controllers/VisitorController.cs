using Microsoft.AspNetCore.Mvc;
using EM.BL;

namespace EM.UI.MVC.Controllers
{
    public class VisitorController : Controller
    {
        private readonly IManager _manager;

        public VisitorController(IManager manager)
        {
            _manager = manager;
        }

        public IActionResult Details(int id)
        {
            var visitor = _manager.GetVisitor(id);
            return View(visitor);
        }
    }
}