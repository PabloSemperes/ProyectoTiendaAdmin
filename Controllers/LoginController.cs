using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTTShopAdmin.Entities;


namespace NTTShopAdmin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ManagementUser objUser)
        {
            ManagementUser usuario = new ManagementUser();
            if (ModelState.IsValid)
            {
                ////Aqui llamar a vuestra api 

                sbyte result = UserLogin(objUser.Login, objUser.Password);

                if (result == 1)
                    return RedirectToAction("../Home/Index");
                else if (result == 2) 
                {
                    ViewBag.Message = "Usuario/contraseña incorrecto";
                    return View(objUser);
                }
                else
                    return View(objUser);
            }
            
            return View(objUser);
        }
        private sbyte UserLogin(string login, string password)
        {
            sbyte result = -1;
            string url = @"https://localhost:7204/api/ManagementUser/ManagementUserLogin";
            var userData = new { managementUser = new ManagementUser(login, password) };
            string json = JsonConvert.SerializeObject(userData);

            HttpWebResponse httpResponse = null;

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";

                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                ManagementUser user = GetUser(login);

                if (user != null && user.PkUser > 0)
                {
                    Session["UserID"] = user.PkUser;
                    Session["UserLogin"] = user.Login;
                    result = 1;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("409")) result = 2;
                else if (ex.Message.Contains("400")) result = 0;
            }
            return result;
        }
        private ManagementUser GetUser(string login)
        {
            string url = @"https://localhost:7204/api/ManagementUser/getAllManagementUsers";
            ManagementUser user = null;
            List<ManagementUser> users = new List<ManagementUser>();

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject jsonObject = JObject.Parse(result);
                    JArray jsonArray = jsonObject["managementUsersList"].ToObject<JArray>();
                    users = jsonArray.ToObject<List<ManagementUser>>();
                }

                int count = 0; bool finish = false;
                while (count < users.Count && !finish)
                {
                    if (users[count].Login == login)
                    {
                        user = users[count];
                        finish = true;
                    }
                    count++;
                }
            }
            catch (Exception ex)
            {

            }
            return user;
        }

        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult LogOut() 
        {
            Session["UserLogin"] = null;
            Session["UserID"] = null;
            return RedirectToAction("Login");
        }
    }
}