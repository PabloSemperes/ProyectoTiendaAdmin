using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTTShopAdmin.Entities
{
    public class ProductRate
    {
        public int idProduct { get; set; }
        public int idRate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0.00}")]
        [RegularExpression(@"^[-0123456789]+[0-9.,]*$",
        ErrorMessage = "El valor introducido debe ser de tipo monetario.")]
        public decimal price { get; set; }
    }
}