using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTTShopAdmin.Entities
{
    public class OrderDetail
    {
        public int idOrder { get; set; }
        public int idProduct { get; set; }
        public decimal price { get; set; }
        public int units { get; set; }
        public Product product { get; set; }
        public OrderDetail() { }
        public OrderDetail(int idOrder, int idProduct, decimal price, int units, Product product)
        {
            this.idOrder = idOrder;
            this.idProduct = idProduct;
            this.price = price;
            this.units = units;
            this.product = product;
        }
    }
}