using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTTShopAdmin.Entities
{
    public class Language
    {
        public int idLanguage { get; set; }
        public string description { get; set; }
        public string iso { get; set; }

        public Language() { }

        public Language(int id, string desc, string iso)
        {
            this.idLanguage = id;
            this.description = desc;
            this.iso = iso;
        }
    }
}