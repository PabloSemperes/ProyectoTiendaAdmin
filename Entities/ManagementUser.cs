using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTTShopAdmin.Entities
{
    public class ManagementUser
    {
        public int PkUser { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        [StringLength(100,ErrorMessage = "Password must be longer than 10 characters", MinimumLength = 10)]
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname1 { get; set; }
        public string Surname2 { get; set; }
        public string Email { get; set; }
        public string Language { get; set; }
        public ManagementUser() { }
        public ManagementUser(string login, string password)
        {
            PkUser = 0;
            Login = login;
            Password = password;
            Name = "name";
            Surname1 = "surname1";
            Surname2 = "surname2";
            Email = "email";
            Language = "es";
        }
    }
}