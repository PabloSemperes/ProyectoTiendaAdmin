using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTTShopAdmin.Entities;
using NTTShopAdmin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace NTTShopAdmin.Controllers
{
    public class OrdersController : Controller
    {
        // GET: Orders
        public ActionResult ListarOrders(int? pageSize, int? page, DateTime? dateFrom, DateTime? dateTo, int? orderStat)
        {
            var orders = GetAllOrders(dateFrom,dateTo,orderStat);
            ViewBag.Status = GetAllOrderStatus();
            ViewBag.Users = GetAllUsers();

            pageSize = (pageSize ?? 5);
            page = (page ?? 1);
            ViewBag.PageSize = pageSize;

            return View(orders.ToPagedList(page.Value, pageSize.Value));
        }
        public ActionResult RemoveOrder(int id, int pagina)
        {
            string url = @"https://localhost:7204/api/Order/DeleteOrder";
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
            return RedirectToAction("ListarOrders", new { pageSize = 5 , page = pagina });
        }
        public ActionResult UpdateOrderStatus(int? id, int? statu)
        {
            sbyte result = -1;
            string url = @"https://localhost:7204/api/Order/UpdateOrderStatus";
            var productData = new { idOrder = id, orderStatus = statu };
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
            return RedirectToAction("ListarOrders", new { pageSize = 5, page = 1 });
        }
        private List<Order> GetAllOrders(DateTime? dateFrom, DateTime? dateTo, int? orderStat)
        {
            List<Order> list = new List<Order>();
            string url = @"https://localhost:7204/api/Order/getAllOrders";
            DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
            DateTime dateF = new DateTime(), dateT = new DateTime();
            string dateFromString = "", dateToString = "";

            if (dateFrom != null)
            {
                dateF = dateFrom.Value;
                dateFromString = dateF.ToString(dtfi.SortableDateTimePattern);
            }
            if (dateTo != null)
            {
                dateT = dateTo.Value;
                dateToString = dateT.ToString(dtfi.SortableDateTimePattern);
            }
            string data = "?fromDate=" + dateFromString + "&toDate=" + dateToString + "&OrderStatus=" + orderStat;
            url += data;

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject jsonObject = JObject.Parse(result);
                    JArray jsonArray = jsonObject["orders"].ToObject<JArray>();
                    list = jsonArray.ToObject<List<Order>>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return list;
        }
        private List<OrderStatus> GetAllOrderStatus() 
        {
            List<OrderStatus> listStatus = new List<OrderStatus>();
            string url = @"https://localhost:7204/api/Order/getAllOrderStatus";

            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject jsonObject = JObject.Parse(result);
                    JArray jsonArray = jsonObject["orderStatus"].ToObject<JArray>();
                    listStatus = jsonArray.ToObject<List<OrderStatus>>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return listStatus;
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
            }
            catch (Exception ex)
            {

            }
            return users;
        }
    }
}