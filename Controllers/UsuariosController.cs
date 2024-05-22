using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTTShopAdmin.Entities;
using NTTShopAdmin.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Xml.Linq;

namespace NTTShopAdmin.Controllers
{
    public class UsuariosController : Controller
    {
        //Acción principal de usuarios
        public ActionResult Usuarios(int? pageSize, int? page) 
        {
            AmbosUsuarios ambosUsuarios = new AmbosUsuarios();
            var users = GetAllUsers();

            pageSize = (pageSize ?? 5);
            page = (page ?? 1);
            ViewBag.PageSize = pageSize;

            return View(users.ToPagedList(page.Value, pageSize.Value));
        }
        public ActionResult RemoveUser(int id) 
        {
            string url = @"https://localhost:7204/api/User/DeleteUser";
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
            return RedirectToAction("Usuarios");
        }
        public ActionResult UsuarioVista(User user)
        {
            ViewBag.Lenguajes = GetAllLanguagesIsos();
            ViewBag.Rates = GetAllRatesId();
            if (ModelState.IsValid)
            {
                UpdateUser(user);
                ViewBag.Correcto = "Usuario actualizado";
            }
            return View(user);
        }
        public ActionResult MostrarUsuario(int id) 
        {
            ViewBag.Lenguajes = GetAllLanguagesIsos();
            ViewBag.Rates = GetAllRatesId();
            User user = GetUser(id);
            return View("UsuarioVista", user);
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
        private List<User> GetAllUsers()
        {
            string url = @"https://localhost:7204/api/User/getAllUsers";
            List<User> users = new List<User>();

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject jsonObject = JObject.Parse(result);
                    JArray jsonArray = jsonObject["usersList"].ToObject<JArray>();
                    users = jsonArray.ToObject<List<User>>();
                }
                string search = Request.Form["searchTerm"];
                if (search != null)
                {
                    if (search.Length > 0)
                    {
                        List<User> searchedTerms = new List<User>();
                        foreach (User user in users) 
                        {
                            if(user.Login.Contains(search)) searchedTerms.Add(user);
                        }
                        users = searchedTerms;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return users;
        }
        private sbyte UpdateUser(User user)
        {
            user.Password = "";
            sbyte result = -1;
            string url = @"https://localhost:7204/api/User/UpdateUser";
            var userData = new { user = user };
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
        private List<int> GetAllRatesId()
        {
            List<Rate> list = new List<Rate>();
            List<int> ides = new List<int>();

            string url = @"https://localhost:7204/api/Rate/getAllRates";

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject jsonObject = JObject.Parse(result);
                    JArray jsonArray = jsonObject["ratesList"].ToObject<JArray>();
                    list = jsonArray.ToObject<List<Rate>>();
                }

                foreach (Rate item in list)
                {
                    ides.Add(item.idRate);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return ides;
        }

    }
}