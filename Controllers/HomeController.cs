using Microsoft.AspNetCore.Mvc;
using YsrisCoreLibrary.Abstract;

namespace YsrisCoreLibrary.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        //[Authorize]
        public IActionResult Index() => View();
    }
}