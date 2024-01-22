using GraphDispaly.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace GraphDispaly.Controllers
{
    public class HomeController : Controller
    {
        private static string WEBAPIURI = ConfigurationManager.AppSettings["WebApiUri"];
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult UpdateUserPrinciple()
        {
            if((Session["Usersession"])!= null)
            {
                return View();
            }
            else
            {
                return Redirect("../Home/Login");
            }
           
        }
        [HttpPost]
        public ActionResult SaveUpdateUuserPrinciple(string ObjectId,string emailid)
        {
           string sResult = null;
            UserDetails ud = new UserDetails();
            using (var client = new HttpClient())
            {
                ud.ObjectId = ObjectId;
                ud.emailid = emailid;
                client.BaseAddress = new Uri(WEBAPIURI + "api/MyDirectory/UpdateUsers");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PostAsJsonAsync("UpdateUsers", ud).Result;
             

                if (response.IsSuccessStatusCode)
                {
                    sResult = response.Content.ReadAsAsync<string>().Result;

                }
            }
            return Json(sResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveLogin(string Username, string Password)
        {
            if(Username=="varaprasad@dentsply.com" && Password=="Dents!@#$")
            {
                Session["Usersession"] = Username;
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(2, JsonRequestBehavior.AllowGet);
            }
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