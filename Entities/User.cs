using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTTShopAdmin.Entities
{
    public class User
    {
        public int PkUser { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname1 { get; set; }
        public string Surname2 { get; set; }
        public string Adress { get; set; }
        public string Province { get; set; }
        public string Town { get; set; }
        [RegularExpression(@"^[0-9]+$",
        ErrorMessage = "Por favor, introduzca un número")]
        public string PostalCode { get; set; }
        [RegularExpression(@"^[0-9]+$",
        ErrorMessage = "Por favor, introduzca un número")]
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Language { get; set; }
        public int Rate { get; set; }
    }
}