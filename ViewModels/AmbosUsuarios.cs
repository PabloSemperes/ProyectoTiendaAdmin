using NTTShopAdmin.Entities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTTShopAdmin.ViewModels
{
    public class AmbosUsuarios
    {
        public List<ManagementUser> managementUsers { get; set; }
        public List<User> users { get; set; }
    }
}