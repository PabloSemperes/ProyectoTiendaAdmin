using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTTShopAdmin.Entities
{
    public class Product
    {
        public int idProduct { get; set; }
        public int stock { get; set; }
        public bool enabled { get; set; }
        public List<ProductDescription> descriptions { get; set; }
        public List<ProductRate> rates { get; set; }
        public Product() { }
        public Product(int id, int stoc, bool enab, List<ProductDescription> desc, List<ProductRate> rat) 
        {
            idProduct= id; stock = stoc; enabled= enab; descriptions = desc; rates = rat;
        }
    }
}