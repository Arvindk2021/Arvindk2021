using ClientCustom.UI.Models;
using ClientCustom.UI.Services;
using System;
using System.Text;
using System.Web.Mvc;

namespace ClientCustom.UI.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login() 
        {
            if (Request.QueryString["status"] != null && Request.QueryString["status"].Contains("success"))
            {
                return RedirectToAction("Success");//redirect to success view
            }
            else
            {
                var model = new LoginViewModel();
                //set default values
                model.Id = "2000";
                model.Firstname = "Alice";
                model.Lastname = "Test";
                model.Email = "yordanka.stoykova@streamamg.com";

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            //generate a token from cliams is empty
            if (String.IsNullOrEmpty(model.Token))
            {
                var jwtModel = JWTModel.GetJWTModel(model.Id, model.Firstname, model.Lastname, model.Email); //todo
                var service = new JWTService();
                model.Token = service.GenerateToken(jwtModel);
                //todo for test purposes show token
            }

            /*Arvind implementation
             *  var url="http://localhost:50376/sso/login?status=success";
            var ssoUrl = "http://localhost:3588/sso/start/";
            //Base64 encoded url. Also replace / with $ and = with ~
            var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("returnurl:" + url)).Replace("/", "$").Replace("=", "~");
            var token = model.JwtToken.Trim();
            var resolvedUrl = ssoUrl + base64Encoded + "/?lang=en&token=" + token;
            //working sercret key for above token: YTNOaGJXYzRNams9
            return Redirect(resolvedUrl);
             * */

            var paymentApiSSOUrl = PaymentSettings.PaymentSSOUrl;
            //set the returnurl
            var returnurl = $"{Request.Url}?status=success";

            var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("returnurl:" + returnurl)).Replace("/", "$").Replace("=", "~");
            //generated token from client side using secretkey and claims
            var token = model.Token;

            //paymentapi url using to redirect to  paymentapi
            var paymentApiWithToken = paymentApiSSOUrl + base64Encoded + "/?lang=en&token=" + token;

            //redirect to paymentapi
            return Redirect(paymentApiWithToken);
        }

      
        public ActionResult Success()
        {
            return View();
        }
    }
}