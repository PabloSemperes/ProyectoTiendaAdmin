using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTTShopAdmin.Entities;
using NTTShopAdmin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace NTTShopAdmin.Controllers
{
    public class ProductosController : Controller
    {
        public ActionResult Productos(int? pageSize, int? page)
        {
            AmbosUsuarios ambosUsuarios = new AmbosUsuarios();
            var products = GetAllProducts();

            pageSize = (pageSize ?? 4);
            page = (page ?? 1);
            ViewBag.PageSize = pageSize;

            return View(products.ToPagedList(page.Value, pageSize.Value));
        }
        public ActionResult AnyadeProducto(Product product) 
        {
            ViewBag.Lenguajes = GetAllLanguagesIsos();
            if (product.descriptions == null)
            {
                product = new Product();
                product.idProduct = 0;
                product.descriptions = new List<ProductDescription>();
                ProductDescription description = new ProductDescription();
                product.descriptions.Add(description);
                product.rates = new List<ProductRate>();
                return View(product);
            }
            else
            {
                if (ModelState.IsValid) 
                {
                    sbyte result = InsertProduct(product);
                    if (result == 1) return RedirectToAction("Productos");
                    else ViewBag.ErrorAnyadeProd = "Error de formato";
                }
            }

            return View(product);
        }
        public ActionResult ProductoDetalle(Product producto) 
        {
            ViewBag.Lenguajes = GetAllLanguagesIsos();
            if (ModelState.IsValid)
            {
                sbyte result = UpdateProduct(producto);
                Product productBase = (Product)TempData["ProductoBase"];
                TempData.Keep("ProductoBase");

                for (int i = 0; i < producto.rates.Count; i++)
                {
                    sbyte resultPrice;
                    if (producto.rates[i].price != productBase.rates[i].price)
                    {
                        resultPrice = SetPrice(producto.idProduct, producto.rates[i].idRate, producto.rates[i].price);
                        if (resultPrice == 1)
                        {
                            ViewBag.MessagePrice = "Precio actualizado";
                            TempData["ProductoBase"] = GetProduct(producto.idProduct);
                        }
                    }
                }

                if (result == 1) 
                {
                    ViewBag.Message = "Usuario actualizado";
                    TempData["ProductoBase"] = producto;
                    return View(producto);
                }
                else 
                {
                    ViewBag.Message = "Datos incorrectos";
                    return View(producto);
                }
            }

            return View(producto);
        }
        public ActionResult RecibirProducto(int id) 
        {
            Product product = GetProduct(id);
            ViewBag.Lenguajes = GetAllLanguagesIsos();
            TempData["ProductoBase"] = product;

            return View("ProductoDetalle", product);
        }
        public ActionResult PreparaPrecio() 
        {
            Product productoBase = (Product)TempData["ProductoBase"];
            TempData.Keep("ProductoBase");
            ProductRate rate = new ProductRate();
            rate.idProduct = productoBase.idProduct;
            rate.idRate = 0;
            TempData["NuevoPrecio"] = rate;
            return RedirectToAction("AnyadirPrecio");
        }
        public ActionResult AnyadeDescripcion(ProductDescription desc)
        {
            ViewBag.Lenguajes = GetAllLanguagesIsos();
            if (desc.title == null) //Primera entrada, se prepara el modelo
            {
                Product productoBase = (Product)TempData["ProductoBase"];
                TempData.Keep("ProductoBase");
                desc.idProduct = productoBase.idProduct;
            }
            else if (ModelState.IsValid) 
            {
                Product prod = (Product)TempData["ProductoBase"];
                TempData.Keep("ProductoBase");
                prod.descriptions.Add(desc);
                sbyte result = UpdateProduct(prod);
                if (result == 1)
                {
                    ViewBag.Message = "Usuario actualizado";
                    return RedirectToAction("RecibirProducto", new { id = prod.idProduct });
                }
                else
                {
                    ViewBag.MessageDesc = "Datos incorrectos";
                    desc = new ProductDescription();
                    desc.idProduct = prod.idProduct;
                    desc.idProductDescription = 0;
                    return View(desc);
                }
            }
            return View(desc);
        }
        public ActionResult AnyadirPrecio(ProductRate rate) 
        {
            ViewBag.Rates = GetAllRatesId();
            if (TempData["NuevoPrecio"] != null)
            {
                rate = (ProductRate)TempData["NuevoPrecio"];
                TempData["NuevoPrecio"] = null;
            }
            else if (ModelState.IsValid) 
            {
                sbyte result = SetPrice(rate.idProduct, rate.idRate, rate.price);
                if (result == 1)
                {
                    ViewBag.MessagePrice = "Precio añadido";
                    TempData["Producto"] = GetProduct(rate.idProduct);
                    TempData["ProductoBase"] = (Product)TempData["Producto"];
                    return RedirectToAction("ProductoDetalle");
                }
                else
                {
                    ViewBag.MessageAnyadPrice = "El precio debe ser superior a 0";
                    return View(rate);
                }
            }
            return View(rate);
        }
        private sbyte InsertProduct(Product product)
        {
            sbyte result = -1;
            string url = @"https://localhost:7204/api/Product/insertProduct";
            var productData = new { product = new Product(product.idProduct, product.stock, product.enabled, product.descriptions, product.rates) };
            string json = JsonConvert.SerializeObject(productData);

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
        private sbyte SetPrice(int idProd, int idRat, decimal pric) 
        {
            sbyte result = -1;
            string url = @"https://localhost:7204/api/Product/setPrice";
            var priceData = new { idProduct = idProd , idRate = idRat, price = pric};
            string json = JsonConvert.SerializeObject(priceData);

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
        private sbyte UpdateProduct(Product product)
        {
            sbyte result = -1;
            string url = @"https://localhost:7204/api/Product/updateProduct";
            var productData = new { product =  new Product(product.idProduct,product.stock,product.enabled,product.descriptions,product.rates)};
            string json = JsonConvert.SerializeObject(productData);

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
        private List<Product> GetAllProducts()
        {
            List<Product> list = new List<Product>();

            string url = @"https://localhost:7204/api/Product/getAllProducts/";

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject jsonObject = JObject.Parse(result);
                    JArray jsonArray = jsonObject["products"].ToObject<JArray>();
                    list = jsonArray.ToObject<List<Product>>();
                }
                string search = Request.Form["searchTerm"];
                if (search != null)
                {
                    if (search.Length > 0)
                    {
                        List<Product> searchedTerms = new List<Product>();
                        foreach (Product prod in list)
                        {
                            if (prod.descriptions[0].title.Contains(search)) searchedTerms.Add(prod);
                        }
                        list = searchedTerms;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return list;
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
        private Product GetProduct(int id)
        {
            Product product;

            string url = @"https://localhost:7204/api/Product/getProduct/";
            url = url + id;

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject jsonObject = JObject.Parse(result);
                    product = jsonObject["product"].ToObject<Product>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return product;
        }
    }
}