using PaymentsAPIClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaymentsAPIClient.Controllers
{
    public class VideoController : Controller
    {
        // GET: Video
        [HttpGet]
        [Route("video")]
        public ActionResult Index()
        {
            var model = new VideoListViewModel();
            return View(model);
        }


        [HttpGet]
        [Route("video/{entryid}")]
        public ActionResult Video(string entryid)
        {

            return View();
        }

    }
}