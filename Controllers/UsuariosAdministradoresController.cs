using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTTShopAdmin.Entities;
using NTTShopAdmin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NTTShopAdmin.Controllers
{
    public class UsuariosAdministradoresController : Controller
    {
        public ActionResult UsuariosAdministradores(int? pageSize, int? page)
        {
            AmbosUsuarios ambosUsuarios = new AmbosUsuarios();
            var users = GetAllManagementUser();

            pageSize = (pageSize ?? 5);
            page = (page ?? 1);
            ViewBag.PageSize = pageSize;

            return View(users.ToPagedList(page.Value, pageSize.Value));
        }
        private List<ManagementUser> GetAllManagementUser()
        {
            string url = @"https://localhost:7204/api/ManagementUser/getAllManagementUsers";
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
            }
            catch (Exception ex)
            {

            }
            return users;
        }
        private User GetUser(int id)
        {
            string url = @"https://localhost:7204/api/User/getUser/";
            url = url + id;
            User user = null;

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject jsonObject = JObject.Parse(result);
                    user = jsonObject["user"].ToObject<User>();
                }
            }
            catch (Exception ex)
            {

            }
            return user;
        }
        public ActionResult RemoveManagementUser(int id, int pagina)
        {
            string url = @"https://localhost:7204/api/ManagementUser/DeleteManagementUser";
            var idData = new { id };
            string json = JsonConvert.SerializeObject(idData);

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "DELETE";

                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("UsuariosAdministradores", new { pageSize = 5, page = pagina });
        }

    }
}