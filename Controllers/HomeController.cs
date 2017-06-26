﻿using Microsoft.AspNetCore.Mvc;
using YsrisCoreLibrary.Abstract;

namespace YsrisCoreLibrary.Controllers
{
    public class HomeController : AbstractController
    {
        [HttpGet]
        //[Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}