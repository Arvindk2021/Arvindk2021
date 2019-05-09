using Newtonsoft.Json;
using Org.BouncyCastle.Math;
using PaymentsAPIClient.Models;
using PaymentsAPIClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static PaymentsAPIClient.Service.JwtService;

namespace PaymentsAPIClient.Controllers
{
    public class JwtLoginController : Controller
    {
        // GET: Jwt
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("reg/sso/login")]
        public ActionResult RegSsoLogin()
        {
            var model = new SsoLoginViewModel();
            model.Status = "fail";
            if (Request.QueryString["status"] != null)
            {
                model.Status = Request.QueryString["status"].ToString().ToLowerInvariant();
            }

            return View(model);
        }

        /*
            This is the method used to integrate payments platform with most the clients. This is also the default implementation. 
            When user tries to register, or access a video and he is not already login, he is redirected back to jwt login page which mentioned as login url in cofiguration. 
            Payments api sends base64 encoded url which payments api use to continue with the process. 
            cleint application has to retreieve parameter returnrul and then append jwt token and rediects back to the resolved url. 
        */

        [HttpPost]
        [Route("reg/sso/login")]
        public ActionResult RegSsoLogin(SsoLoginViewModel model)
        {
            var token = model.JwtToken;
            var resolvedUrl = Request.QueryString["returnurl"];
            resolvedUrl += $"&token={token}";

            return Redirect(resolvedUrl);
        }

        [HttpGet]
        [Route("sso/login")]
        public ActionResult SsoLogin()
        {
            var model = new SsoLoginViewModel();
            model.Status = "fail";
            if (Request.QueryString["status"] != null)
            {
                model.Status = Request.QueryString["status"].ToString().ToLowerInvariant();
            }
            
            return View(model);
        }
        /*
            Though this is not the default impmlentation at present in payments api, but one client asked to use the method. 
            The method is to log into paymentsapi using jwt without starting from registration and or from video access page. 
            Authentication using this methods requires a return url where payments api redirects back after successful url.
            The return url is a base64 encoded plus two characters "/" and "=" replaced with "$" and "~"
             
            Rest process is similar. 

            The limitation here is that payments application does not return any success or fail status back to calling app, Any error message is displayed at payments api checkout page. 
        */

        [HttpPost]
        [Route("sso/login")]
        public ActionResult SsoLogin(SsoLoginViewModel model)
        {

            ////use this method to test a jwt login with custom return url
            //var url = Request.Url.AbsoluteUri + "?status=success";

            //This rul is only for this machine, nad has to be modified as per the url at local machine. 
            //Sorry to keep it hardcoded. 

            var url="http://localhost:50376/sso/login?status=success";
            var ssoUrl = "http://localhost:3588/sso/start/";

            //Base64 encoded url. Also replace / with $ and = with ~
            var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("returnurl:" + url)).Replace("/", "$").Replace("=", "~");
            var token = model.JwtToken;
            var resolvedUrl = ssoUrl + base64Encoded + "/?lang=en&token=" + token;
            //working sercret key for above token: YTNOaGJXYzRNams9
            return Redirect(resolvedUrl);
        }

        //[HttpGet]
        //[Route("awssso/login")]
        //public ActionResult AwsSsoLogin()
        //{
        //    var model = new SsoLoginViewModel();
        //    model.Status = "fail";
        //    if (Request.QueryString["status"] != null)
        //    {
        //        model.Status = Request.QueryString["status"].ToString().ToLowerInvariant();
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //[Route("awssso/login")]
        //public ActionResult AwsSsoLogin(SsoLoginViewModel model)
        //{

        //    ////use this method to test a jwt login with custom return url
        //    var url = Request.Url.AbsoluteUri + "?status=success";
        //    var ssoUrl = "http://localhost:3588/sso/start/";

        //    //Base64 encoded url. Also replace / with $ and = with ~
        //    var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("returnurl:" + url)).Replace("/", "$").Replace("=", "~");
        //    var awsJwtToken = "eyJraWQiOiJlREN5U3RUem9yOWkyM0YrRTFURjFMbWN4ZTl3OE5WWnhoRStuN1VEUTBZPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiIwZDhiODM0Mi03NzFhLTRmYmItYTU2My1mODkzNGQxMTg2ODciLCJjdXN0b206cGFydG5lcl9zbXMiOiJ0cnVlIiwiaXNzIjoiaHR0cHM6XC9cL2NvZ25pdG8taWRwLmV1LXdlc3QtMS5hbWF6b25hd3MuY29tXC9ldS13ZXN0LTFfWTV0blZlOUdsIiwiY3VzdG9tOmNsdWJfc21zIjoidHJ1ZSIsImF1dGhfdGltZSI6MTU1MTA5ODYzMCwiY3VzdG9tOnBhcnRuZXJfZW1haWwiOiJ0cnVlIiwiZXhwIjoxNTUxMTAyMjMwLCJpYXQiOjE1NTEwOTg2MzAsImVtYWlsIjoibUBydGluZC51ayIsImN1c3RvbTpwYXJ0bmVyX2RpcmVjdCI6InRydWUiLCJjdXN0b206Y2x1Yl9kaXJlY3QiOiJ0cnVlIiwiY3VzdG9tOmNsdWJfZW1haWwiOiJ0cnVlIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImN1c3RvbTpwYXJ0bmVyX2NvbnRhY3QiOiJ0cnVlIiwicGhvbmVfbnVtYmVyX3ZlcmlmaWVkIjpmYWxzZSwiY29nbml0bzp1c2VybmFtZSI6IjBkOGI4MzQyLTc3MWEtNGZiYi1hNTYzLWY4OTM0ZDExODY4NyIsImdpdmVuX25hbWUiOiJNYXJ0aW4iLCJwaWN0dXJlIjoiaHR0cHM6XC9cL3Nzby1pbWFnZXMuczMuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMSUzQTA5MGM0NGZjLTI4N2EtNDI4NC1iZGNjLTg3Y2QzODk4NGVmZlwvWmxLa0VpeDByTUxGLmpwZWciLCJjdXN0b206cGFydG5lcl9waG9uZSI6InRydWUiLCJjdXN0b206Y2x1Yl9jb250YWN0IjoidHJ1ZSIsImF1ZCI6IjI2ZzBma2ViaHZiaDNrdHYzZ2Q1OXZjbmY3IiwiZXZlbnRfaWQiOiIwMDUxMzc4Yi0zOGZiLTExZTktOTg0Yi05YjU1YWU1MzEyYTkiLCJ0b2tlbl91c2UiOiJpZCIsImN1c3RvbTpjbHViX3Bob25lIjoidHJ1ZSIsInBob25lX251bWJlciI6Iis0NDA3ODEzODUzMDUyIiwiZmFtaWx5X25hbWUiOiJEdW5mb3JkIn0.NXLFigeMM5PkWZ4b56pxVpfE7Id4QU6ANwHjvgG5DhJsloKWvtHHkdv0hpH0b9KZDcPwf0CR5zGByoQ8ao5ovdu8fx2GX4P-Va0jgKWbzeFQ5URLtnS2kZyp2D84yFyR2VEbV7uYreMoj7lQEzwHQK_FdVECMlQub5rPNlDJrCHG0JU2V58FlSvEh_hFIBwTMOB8XMftwW4TQYP_GWhxrt_lOEjkK-aL4vuywXSQwm-MVebKexZ-T3jnk_RjtwN34BZ8xuapCJpV1xXDYRmm0t6clHOl2oR5apobey-DHYuwqn-y_AjLKBpMEA7oDZz8OQJ9MxosDcU05ymnq70p3A";
        //    var resolvedUrl = ssoUrl + base64Encoded + "/?lang=en&token=" + awsJwtToken;

        //    /* 
        //     * //User this to follow Payments api registration flow
        //    //var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE1MjU3Njk1NTYsImV4cCI6MTU1OTk4Mzk1NiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDozNTg4Iiwic3ViIjoiaHR0cDovL2xvY2FsaG9zdDo1MDM3NiIsImlkIjoiMTAxIiwiZmlyc3RuYW1lIjoiQXJ2aW5kIiwibGFzdG5hbWUiOiJLdW1hciIsImVtYWlsIjoiYXJ2aW5kLmt1bWFyQHN0cmVhbWFtZy5jb20ifQ.nrm5MeNlSSDV9Ki1jcV5ZCl9kmdNuys3SzqJCoZzHyk";
        //    //var resolvedUrl = Request.QueryString["returnurl"];
        //    //resolvedUrl += $"&token={token}";
        //    */

        //    //var setCookieUrl = "http://localhost:3588/api/v1/session/setcookie/"+token;

        //    return Redirect(resolvedUrl);


        //    //return View(model);
        //}



        //[HttpGet]
        //[Route("fancoresso/login")]
        //public ActionResult FanCoreSsoLogin()
        //{
        //    var model = new SsoLoginViewModel();
        //    model.Status = "fail";
        //    if (Request.QueryString["status"] != null)
        //    {
        //        model.Status = Request.QueryString["status"].ToString().ToLowerInvariant();
        //    }

        //    return View(model);
        //}

        //public class PayLoad
        //{
        //    public string FirstName { get; set; }
        //    public string LastName { get; set; }
        //    public string Email { get; set; }
        //    public string Id { get; set; }
        //}


        //[HttpPost]
        //[Route("fancoresso/login")]
        //public ActionResult FanCoreSsoLogin(SsoLoginViewModel model)
        //{

        //    //var payload = new PayLoad
        //    //{
        //    //    FirstName= "Arvind",
        //    //    LastName = "Kumar",
        //    //    Email= "arvind.kumar@streamamg.comg",
        //    //    Id= "1001"
        //    //};

        //    //var privateKey = @"MIIEogIBAAKCAQEA33TqqLR3eeUmDtHS89qF3p4MP7Wfqt2Zjj3lZjLjjCGDvwr9
        //    //                cJNlNDiuKboODgUiT4ZdPWbOiMAfDcDzlOxA04DDnEFGAf+kDQiNSe2ZtqC7bnIc
        //    //                8+KSG/qOGQIVaay4Ucr6ovDkykO5Hxn7OU7sJp9TP9H0JH8zMQA6YzijYH9LsupT
        //    //                errY3U6zyihVEDXXOv08vBHk50BMFJbE9iwFwnxCsU5+UZUZYw87Uu0n4LPFS9BT
        //    //                8tUIvAfnRXIEWCha3KbFWmdZQZlyrFw0buUEf0YN3/Q0auBkdbDR/ES2PbgKTJdk
        //    //                jc/rEeM0TxvOUf7HuUNOhrtAVEN1D5uuxE1WSwIDAQABAoIBAA41OeJmLx6SAlx4
        //    //                3OfiYhaoh/DZFIDhvCy+JMLdw3gafWz9PuYUiR/L5s8CZHhhvS+/RFhuG/238YGH
        //    //                XjV+3BRWoJlj0Ra5cW3euFUWBWsGR0SbftnG8zFSOgy/BCuG7uVMeak4leOCcNfY
        //    //                aA/Zw8wk3z80k0hqyg94iz3Z0RGGiBg1cXIwb908eq6792dpYRxyoRB29EUYwE3I
        //    //                wFSlfTWYTGoyeJfaaidwOCEKwgZfebsel5taFz9Iumke/HI3IbAqXDF3T91jLLx4
        //    //                E5bGU9EWSxR675IjR5T4opeBtv3h5ML0//wq3GzukpiP8wrTJsqbhyanK/l2+xjy
        //    //                aGuuFqECgYEA8K33pX90XX6PZGiv26wZm7tfvqlqWFT03nUMvOAytqdxhO2HysiP
        //    //                n4W58OaJd1tY4372Qpiv6enmUeI4MidCie+s+d0/B6A0xfhU5EeeaDN0xDOOl8yN
        //    //                +kaaVj9b4HDR3c91OAwKpDJQIeJVZtxoijxl+SRx3u7Vs/7meeSpOfECgYEA7a5K
        //    //                nUs1pTo72A+JquJvIz4Eu794Yh3ftTk/Et+83aE/FVc6Nk+EhfnwYSNpVmM6UKdr
        //    //                Aoy5gsCvZPxrq+eR9pEwU8M5UOlki03vWY/nqDBpJSIqwPvGHUB16zvggsPQUyQB
        //    //                fnN3N8XlDi12n88ltvWwEhn1LQOwMUALEfka9/sCgYAMH2ca4emVj/te/lrlQKzl
        //    //                iDGRY+0kV9shnVmv5ccIJjT0khZF44ZAbbbo6GPCLEq04r86qYAq0woz06Yq+IlE
        //    //                c1sOFtPG6Y3e7twvx1+2NelKvKIRCU+ZbJb3gyd4jZY0iu+HjCu5C4O3wTO2A6IM
        //    //                XHBydSB7LyJ6d3taZmcTsQKBgDvm0k1EODf1LkHs4JBd0w65wa2juu5XgxsEW34h
        //    //                P1NIIUL6oeQwNEEj1c5Vg2XPSlIrb4/L8bEfaNT1vRktGp9exiRGLnrS55EoSitz
        //    //                VjoQQV+ndcj/a1XR+iYYCCRMv4NErs+0wBYhXPIuyRfLuECdOQvG2QDITi6Lan7U
        //    //                HlTjAoGAInfGmkb2jNkPGuNiZ+mU0+ZrOgLza/fLL9ErZ35jUPhGFzdGxJNobklv
        //    //                sNoTd+E2GAU41YkJh24bncMLvJVYxHHA5iF7FBWx1SvpEyKVhhnIcuXGD7N5PbNZ
        //    //                zEdmr9C6I7cPVkWO+sUV7zfFukexIcANmsd/oBBGKRoYzP5Tti4=
        //    //            ";


        //    ////var privateKey = System.IO.File.ReadAllText(Server.MapPath("/App_Data/privatekey.pem"));
        //    //privateKey=privateKey.Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "");

        //    //var payloadJson= JsonConvert.SerializeObject(payload);

        //    //var jwtToken = FanCoreJwtService.Sign(payloadJson,privateKey);

        //    ////use this method to test a jwt login with custom return url
        //    var url = Request.Url.AbsoluteUri + "?status=success";

        //    //Base64 encoded url. Also replace / with $ and = with ~
        //    var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("returnurl:" + url)).Replace("/", "$").Replace("=", "~");
        //    //Working Token 
        //    //var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMzc2Iiwic3ViIjoiYXJ2aW5kLmt1bWFyQHN0cmVhbWFtZy5jb20iLCJuYmYiOjE1NDQ0NDU5MzksImV4cCI6MTU0NDQ0OTUzOSwiaWF0IjoxNTQ0NDQ1OTM5LCJqdGkiOiJpZDEyMzQ1NiIsInR5cCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MzU4OCJ9.jhM82yHlncWWVqFfNpNFc4AmngaPOIkwv2LzY_Pj-J2ZtEkONXwjHNqejCpQznDuYS6VhY7H5KmLD38lukgzVyZ6tnzHprqOLY5f7Jr6soRWNAdban65SWlEQK7tauQgVLm7R37f6HAiyzJuvSL_HgW_s9XTcyGi10BT83lbQRpKmrhgVKNspLaI2T3MTv6OMNu-BA5cdYGc6WOvpBkaxUXyi0tcB4nMDqUvZyvP9Vr8DtTJ-8zynBKzER0_g6boRLpsgaP_rVOvHmAZhTnYfOzpPn8HqwnW7lsPIff6DBFDPYnW8eVUUE6QghMPi_nItHeo3-3KvB8DVMH4Pl5g1A";
        //    //var token= "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUwMzc2Iiwic3ViIjoiYXJ2aW5kLmt1bWFyQHN0cmVhbWFtZy5jb20iLCJuYmYiOjE1NDQ0NTc1NjMsImV4cCI6MTU0NDU0Mzk2MywiaWF0IjoxNTQ0NDU3NTYzLCJqdGkiOiJpZDEyMzQ1NiIsInR5cCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MzU4OCJ9.1n2LLwb8hkqabqOxrWhsnaU90uGUm8elC5VtdEj4kgwmUIvrSJ5lfdcWqs1PmwDcCbcN7hzoV3j2q40QBX6265pPS_t7TmmXldcLLra6tnUwz8pkGsMVjmvzCfexEFw7ID4j6zv5xmrhonaJkDGV-wyNnKvgUMuVOl4OqEpAPviz6MiXvgU1NWDpNpg3ZDmikeq3Rz0g5B26f5pKPjbg3mBs52eW_spq-lN8JsPjYK7NpJNBW24qPbN19q9g9EbnxojvADKNP4j65JyWwrMEtw_4iZguEZlwOAlbDhOupU23RaNFZnTHFY-Ee3KnHA3SsGym8FfxqyoWQJz3HT4mow";
        //    //var rsaToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOjE4MzQ3Miwic2NvcGUiOlsib3Blbl9pZCIsInByb2ZpbGUiXSwiaXNzIjoiSW5Dcm93ZCIsImxhc3RfbmFtZSI6IkdvcnRvbiIsImV4cCI6MTU0MzY2MzgyMiwiZmlyc3RfbmFtZSI6IkNoYXJsaWUiLCJhdXRob3JpdGllcyI6WyJST0xFX1ZFUklGSUVEXyoiLCJST0xFX1VTRVJfKiIsIlJPTEVfUFJPRklMRUNPTVBMRVRFXyoiXSwianRpIjoiZDA0MTcyZWUtNTkxOS00ZGRiLTgyYzgtNWUzZDBhNmNjYTQ2IiwiY2xpZW50X2lkIjoiUFJPMTQiLCJlbWFpbCI6ImNoYXJsaWUuZ29ydG9uKzJAaW5jcm93ZHNwb3J0cy5jb20ifQ.eRP5mGg8sFVksakYwMVvzLmHie_4eVmcBDdOK3rUCBAw6DfiWhIVT3yGXJxIhV-dCBZU_mW7_U2cQrZC8YYvpxEfIbkggOIOYsjiShyrU9VW46a7qDfJGvp5ZSIQckNd_6oF8iQnRZ6u_abq5kQFputbBkKHUUwRK7afhodxRdnkt1BTHD9IuLtLArpHHAwlDupOk234ODeu7daR4CY6Cn7dlbxmbjIRpVYvs-pzhxT_aVEJ-mOGHMJAOIlZkY7P_-sRXTqkTxhRg_PDW5lBTNokIgjdl1vSELlVEJq2YTZ0A3RGwZeSZkhgii7xk_N-nedP5UzxMGorMyApSv4vxg";
        //    var rsaToken = "eyJraWQiOiJlREN5U3RUem9yOWkyM0YrRTFURjFMbWN4ZTl3OE5WWnhoRStuN1VEUTBZPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiIwZDhiODM0Mi03NzFhLTRmYmItYTU2My1mODkzNGQxMTg2ODciLCJjdXN0b206cGFydG5lcl9zbXMiOiJ0cnVlIiwiaXNzIjoiaHR0cHM6XC9cL2NvZ25pdG8taWRwLmV1LXdlc3QtMS5hbWF6b25hd3MuY29tXC9ldS13ZXN0LTFfWTV0blZlOUdsIiwiY3VzdG9tOmNsdWJfc21zIjoidHJ1ZSIsImF1dGhfdGltZSI6MTU1MTA5ODYzMCwiY3VzdG9tOnBhcnRuZXJfZW1haWwiOiJ0cnVlIiwiZXhwIjoxNTUxMTAyMjMwLCJpYXQiOjE1NTEwOTg2MzAsImVtYWlsIjoibUBydGluZC51ayIsImN1c3RvbTpwYXJ0bmVyX2RpcmVjdCI6InRydWUiLCJjdXN0b206Y2x1Yl9kaXJlY3QiOiJ0cnVlIiwiY3VzdG9tOmNsdWJfZW1haWwiOiJ0cnVlIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImN1c3RvbTpwYXJ0bmVyX2NvbnRhY3QiOiJ0cnVlIiwicGhvbmVfbnVtYmVyX3ZlcmlmaWVkIjpmYWxzZSwiY29nbml0bzp1c2VybmFtZSI6IjBkOGI4MzQyLTc3MWEtNGZiYi1hNTYzLWY4OTM0ZDExODY4NyIsImdpdmVuX25hbWUiOiJNYXJ0aW4iLCJwaWN0dXJlIjoiaHR0cHM6XC9cL3Nzby1pbWFnZXMuczMuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMSUzQTA5MGM0NGZjLTI4N2EtNDI4NC1iZGNjLTg3Y2QzODk4NGVmZlwvWmxLa0VpeDByTUxGLmpwZWciLCJjdXN0b206cGFydG5lcl9waG9uZSI6InRydWUiLCJjdXN0b206Y2x1Yl9jb250YWN0IjoidHJ1ZSIsImF1ZCI6IjI2ZzBma2ViaHZiaDNrdHYzZ2Q1OXZjbmY3IiwiZXZlbnRfaWQiOiIwMDUxMzc4Yi0zOGZiLTExZTktOTg0Yi05YjU1YWU1MzEyYTkiLCJ0b2tlbl91c2UiOiJpZCIsImN1c3RvbTpjbHViX3Bob25lIjoidHJ1ZSIsInBob25lX251bWJlciI6Iis0NDA3ODEzODUzMDUyIiwiZmFtaWx5X25hbWUiOiJEdW5mb3JkIn0.NXLFigeMM5PkWZ4b56pxVpfE7Id4QU6ANwHjvgG5DhJsloKWvtHHkdv0hpH0b9KZDcPwf0CR5zGByoQ8ao5ovdu8fx2GX4P-Va0jgKWbzeFQ5URLtnS2kZyp2D84yFyR2VEbV7uYreMoj7lQEzwHQK_FdVECMlQub5rPNlDJrCHG0JU2V58FlSvEh_hFIBwTMOB8XMftwW4TQYP_GWhxrt_lOEjkK-aL4vuywXSQwm-MVebKexZ-T3jnk_RjtwN34BZ8xuapCJpV1xXDYRmm0t6clHOl2oR5apobey-DHYuwqn-y_AjLKBpMEA7oDZz8OQJ9MxosDcU05ymnq70p3A";

        //    //use this when following registration path
        //    //var resolvedUrl = Request.QueryString["returnurl"];
        //    //resolvedUrl += $"&token={rsaToken}";

        //    var ssoUrl = "http://localhost:3588/sso/start/";

        //    //use this to call sso/start directly.
        //    var resolvedUrl = ssoUrl + base64Encoded + "/?lang=en&token=" + rsaToken;

        //    return Redirect(resolvedUrl);
            
        //    //var jwtToken = "eyJraWQiOiJlREN5U3RUem9yOWkyM0YrRTFURjFMbWN4ZTl3OE5WWnhoRStuN1VEUTBZPSIsImFsZyI6IlJTMjU2In0.eyJzdWIiOiIwZDhiODM0Mi03NzFhLTRmYmItYTU2My1mODkzNGQxMTg2ODciLCJjdXN0b206cGFydG5lcl9zbXMiOiJ0cnVlIiwiaXNzIjoiaHR0cHM6XC9cL2NvZ25pdG8taWRwLmV1LXdlc3QtMS5hbWF6b25hd3MuY29tXC9ldS13ZXN0LTFfWTV0blZlOUdsIiwiY3VzdG9tOmNsdWJfc21zIjoidHJ1ZSIsImF1dGhfdGltZSI6MTU1MTA5ODYzMCwiY3VzdG9tOnBhcnRuZXJfZW1haWwiOiJ0cnVlIiwiZXhwIjoxNTUxMTAyMjMwLCJpYXQiOjE1NTEwOTg2MzAsImVtYWlsIjoibUBydGluZC51ayIsImN1c3RvbTpwYXJ0bmVyX2RpcmVjdCI6InRydWUiLCJjdXN0b206Y2x1Yl9kaXJlY3QiOiJ0cnVlIiwiY3VzdG9tOmNsdWJfZW1haWwiOiJ0cnVlIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImN1c3RvbTpwYXJ0bmVyX2NvbnRhY3QiOiJ0cnVlIiwicGhvbmVfbnVtYmVyX3ZlcmlmaWVkIjpmYWxzZSwiY29nbml0bzp1c2VybmFtZSI6IjBkOGI4MzQyLTc3MWEtNGZiYi1hNTYzLWY4OTM0ZDExODY4NyIsImdpdmVuX25hbWUiOiJNYXJ0aW4iLCJwaWN0dXJlIjoiaHR0cHM6XC9cL3Nzby1pbWFnZXMuczMuZXUtd2VzdC0xLmFtYXpvbmF3cy5jb21cL2V1LXdlc3QtMSUzQTA5MGM0NGZjLTI4N2EtNDI4NC1iZGNjLTg3Y2QzODk4NGVmZlwvWmxLa0VpeDByTUxGLmpwZWciLCJjdXN0b206cGFydG5lcl9waG9uZSI6InRydWUiLCJjdXN0b206Y2x1Yl9jb250YWN0IjoidHJ1ZSIsImF1ZCI6IjI2ZzBma2ViaHZiaDNrdHYzZ2Q1OXZjbmY3IiwiZXZlbnRfaWQiOiIwMDUxMzc4Yi0zOGZiLTExZTktOTg0Yi05YjU1YWU1MzEyYTkiLCJ0b2tlbl91c2UiOiJpZCIsImN1c3RvbTpjbHViX3Bob25lIjoidHJ1ZSIsInBob25lX251bWJlciI6Iis0NDA3ODEzODUzMDUyIiwiZmFtaWx5X25hbWUiOiJEdW5mb3JkIn0.NXLFigeMM5PkWZ4b56pxVpfE7Id4QU6ANwHjvgG5DhJsloKWvtHHkdv0hpH0b9KZDcPwf0CR5zGByoQ8ao5ovdu8fx2GX4P-Va0jgKWbzeFQ5URLtnS2kZyp2D84yFyR2VEbV7uYreMoj7lQEzwHQK_FdVECMlQub5rPNlDJrCHG0JU2V58FlSvEh_hFIBwTMOB8XMftwW4TQYP_GWhxrt_lOEjkK-aL4vuywXSQwm-MVebKexZ-T3jnk_RjtwN34BZ8xuapCJpV1xXDYRmm0t6clHOl2oR5apobey-DHYuwqn-y_AjLKBpMEA7oDZz8OQJ9MxosDcU05ymnq70p3A";

        //    ////var n = "oCMQSVtwgkdVos5d3ti3Cxqwf2rMglj3ojdcQtslsDH35RR-gaz83xw07v7JoKnBqQahugDFnXO5LRUCwqdBL2z8NxF19Iv9Pa9AGSxvU21O3gFHbCTaZl91bX2KpqFWG_keQbqZ69q8pKHSy3WPLw0j1GAtR3svQnNQqCTEQulkwQgvdwGBvoqwZLcUqSj_ZdGSxtYhEnsAkPMuY6-YGejuT4osktD1lYRKqvT74iCjT_8lGI_JzJqdAslKmC_MVfGtQq2h7BilNqXL3Gg2YN-mcaLn91EAM3iSBFzhDtT_Hm6FYZrADMOxyttWBlWWgLJ32E0bdJ6V11kJeyPj0Q";
        //    ////var e = "AQAB";
        //    //var verified = FanCoreJwtService.VerifyCognitoJwt(jwtToken);

        //    //return View();
        //}

   

        [HttpGet]
        [Route("jwttoken/sportsalliance")]
        public ActionResult DecodeSportsAllianceJWT()
        {

            var model = new DecodeJwtViewModel();

            return View(model);
        }

        [HttpPost]
        [Route("jwttoken/sportsalliance")]
        public ActionResult DecodeSportsAllianceJWT(DecodeJwtViewModel model)
        {

            try
            {
                var jwtResponse = JwtService.Validate(
                        new ValidateRequest
                        {
                            JwtSecret = model.SecretKeys.Trim(),
                            Token = model.InputJwtToken.Trim(),
                            ValidateAllFields = true,
                            TokenType = model.TokenType
                        }
                    );
                model.StandardToken = jwtResponse.JwtToken;
                model.Claims = jwtResponse.Claims;
            }
            catch (Exception ex)
            {
                model.Message = ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        [Route("jwttoken/standard")]
        public ActionResult DecodeStandardJWT()
        {

            var model = new DecodeJwtViewModel();

            return View(model);
        }

        [HttpPost]
        [Route("jwttoken/standard")]
        public ActionResult DecodeStandardJWT(DecodeJwtViewModel model)
        {

            return View(model);
        }


    }
}