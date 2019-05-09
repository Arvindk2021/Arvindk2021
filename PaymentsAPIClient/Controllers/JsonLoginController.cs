using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaymentsAPIClient.Controllers
{
    public class JsonLoginController : Controller
    {
        // GET: JsonLogin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DoLogin()
        {
            return View();
        }


    }
}