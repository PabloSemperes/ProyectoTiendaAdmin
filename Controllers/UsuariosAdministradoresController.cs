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
            if (Session["UserLogin"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
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
        private ManagementUser GetManagementUser(int id)
        {
            string url = @"https://localhost:7204/api/ManagementUser/getManagementUser/";
            url = url + id;
            ManagementUser user = null;

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject jsonObject = JObject.Parse(result);
                    user = jsonObject["managementUser"].ToObject<ManagementUser>();
                }
            }
            catch (Exception ex)
            {

            }
            return user;
        }
        public ActionResult UsuarioAdministradorVista(ManagementUser user)
        {
            if (Session["UserLogin"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Lenguajes = GetAllLanguagesIsos();
            if (ModelState.IsValid)
            {
                UpdateManagementUser(user);
                ViewBag.Correcto = "Usuario actualizado";
            }
            return View(user);
        }
        public ActionResult UsuarioAdministradorAnyadir(ManagementUser user)
        {
            if (Session["UserLogin"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.Lenguajes = GetAllLanguagesIsos();
            if (ModelState.IsValid)
            {
                InsertManagementUser(user);
                ViewBag.Correcto = "Usuario añadido";
            }
            return View(user);
        }
        private sbyte InsertManagementUser(ManagementUser user)
        {
            sbyte result = -1;
            string url = @"https://localhost:7204/api/ManagementUser/InsertManagementUser";
            var userData = new { managementUser = user };
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

                result = 1;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("409")) result = 2;
                else if (ex.Message.Contains("400")) result = 0;
                else if (ex.Message.Contains("404")) result = 3;
            }
            return result;
        }
        public ActionResult MostrarUsuarioAdministrador(int id)
        {
            ViewBag.Lenguajes = GetAllLanguagesIsos();
            ManagementUser user = GetManagementUser(id);
            return View("UsuarioAdministradorVista", user);
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
        private List<string> GetAllLanguagesIsos()
        {
            List<Language> list = new List<Language>();
            List<string> isos = new List<string>();

            string url = @"https://localhost:7204/api/Language/getAllLanguages";

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject jsonObject = JObject.Parse(result);
                    JArray jsonArray = jsonObject["languageList"].ToObject<JArray>();
                    list = jsonArray.ToObject<List<Language>>();
                }

                foreach (Language item in list)
                {
                    isos.Add(item.iso);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return isos;
        }
        private sbyte UpdateManagementUser(ManagementUser user)
        {
            user.Password = "";
            sbyte result = -1;
            string url = @"https://localhost:7204/api/ManagementUser/UpdateManagementUser";
            var userData = new { managementUser = user };
            string json = JsonConvert.SerializeObject(userData);

            HttpWebResponse httpResponse = null;

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "PUT";

                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                result = 1;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("404")) result = 2;
                else if (ex.Message.Contains("400")) result = 0;
            }
            return result;
        }
        
    }
}