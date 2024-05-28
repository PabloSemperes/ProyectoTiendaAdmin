
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTTShopAdmin.Entities;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace NTTShopAdmin.Controllers
{
    public class LanguageController : Controller
    {
        public ActionResult Language(int? pageSize, int? page)
        {
            List<Language> languages = GetAllLanguages();

            pageSize = (pageSize ?? 5);
            page = (page ?? 1);
            ViewBag.PageSize = pageSize;

            return View(languages.ToPagedList(page.Value, pageSize.Value));
        }
        public ActionResult UpdateLanguage(int id, string iso, string descripcion)
        {
            List<Language> languages = GetAllLanguages();
            Language languag = new Language();
            languag.idLanguage = id;
            languag.iso = iso;
            languag.description = descripcion;

            string url = @"https://localhost:7204/api/Language/updateLanguage";
            var languageData = new { language = languag };
            string json = JsonConvert.SerializeObject(languageData);

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
                ViewBag.Correct = id;
                languages = GetAllLanguages();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("400")) ViewBag.Wrong = id;
            }
            return View("Language", languages.ToPagedList(1, 5)); //RedirectToAction("Language", new { pageSize = 5, page = 1 });
        }
        public ActionResult RemoveLanguage(int id, int pagina) 
        {
            string url = @"https://localhost:7204/api/Language/DeleteLanguage";
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
                if (ex.Message.Contains("409")) TempData["sErrMsg"] = "Ese lenguaje está aún en uso";
            }

            return RedirectToAction("Language", new { pageSize = 5, page = pagina });
        }
        public PartialViewResult ShowError(String sErrorMessage)
        {
            return PartialView("ErrorLanguage");
        }
        private List<Language> GetAllLanguages()
        {
            List<Language> list = new List<Language>();

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
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return list;
        }
        public ActionResult AgregarLanguage(string action,  string txtDescripcion, string txtIso)
        {
            List<Language> languages = GetAllLanguages();
            if ( !string.IsNullOrWhiteSpace(txtDescripcion) && !string.IsNullOrWhiteSpace(txtIso))
            {
               
                if (action == "Añadir")
                {
                    
                    Language language = new Language();

                    language.idLanguage = 1;
                    language.description = txtDescripcion;
                    language.iso = txtIso;

                    if (!InsertarLanguage(language))
                    {
                        MessageBox.Show("Error: ISO ya existe", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        languages = GetAllLanguages();
                    }
                    



                    return View("Language", languages.ToPagedList(1, 5));
                }


            }


            return View("Language", languages.ToPagedList(1, 5));
        }
        public bool InsertarLanguage(Language language)
        {
            bool insertado = false;
            string baseUrl = "https://localhost:7204/api/";
            // esto nos permite poder poner lo de Language{
            //   idLanguage = "", descripcion = "", iso "" }
            var languageData = new { language = language };

            string jsonData = JsonConvert.SerializeObject(languageData);

            string url = baseUrl + "Language/insertLanguage";

            try
            {
                //HTTP POST
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.Accept = "application/json";
                httpRequest.ContentType = "application/json";

                // Escribimos el cuerpo del mensaje
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonData);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                HttpStatusCode statusCode = httpResponse.StatusCode;

                // Si es OK
                if (statusCode == HttpStatusCode.OK)
                {
                    insertado = true;
                }
                else
                {
                    // Si hay un error, leer el mensaje de error 
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string errorMessage = streamReader.ReadToEnd();
                        Console.WriteLine("Error de la API: " + errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al realizar la solicitud: " + ex.Message);
            }

            return insertado;
        }
    }
}
