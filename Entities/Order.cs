using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTTShopAdmin.Entities
{
    public class Order
    {
        public int idOrder { get; set; }
        public DateTime dateTime { get; set; }
        public int orderStatus { get; set; }
        public decimal totalPrice { get; set; }
        public int idUser { get; set; }
        public List<OrderDetail> orderDetails { get; set; }
        public Order() { }
        public Order(int idOrder, DateTime dateTime, int orderStatus, decimal totalPrice, int idUser, List<OrderDetail> orderDetails)
        {
            this.idOrder = idOrder;
            this.dateTime = dateTime;
            this.orderStatus = orderStatus;
            this.totalPrice = totalPrice;
            this.idUser = idUser;
            this.orderDetails = orderDetails;
        }
    }
}