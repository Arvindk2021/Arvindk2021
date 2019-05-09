using PaymentsAPIClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaymentsAPIClient.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("register")]
        public ActionResult Register()
        {
            return View();
        }



        [HttpGet]
        [Route("jwt/login")]
        public ActionResult JwtLogin()
        {
            var model = new JwtLoginViewModel();
            return View();
        }

        [HttpPost]
        [Route("jwt/login")]
        public ActionResult JwtLogin(JwtLoginViewModel model)
        {
            return View(model);
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}