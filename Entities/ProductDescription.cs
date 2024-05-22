using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTTShopAdmin.Entities
{
    public class ProductDescription
    {
        public int idProductDescription { get; set; }
        public int idProduct { get; set; }
        public string language { get; set; }
        [Required]
        public string title { get; set; }
        public string description { get; set; }
    }
}