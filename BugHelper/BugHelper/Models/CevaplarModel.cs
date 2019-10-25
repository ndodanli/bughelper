using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugHelper.Models
{
    public class CevaplarModel
    {
        public int Id { get; set; }
        public string Cevap { get; set; }
        public string CevapSahibi { get; set; }
        public DateTime CevapTarihi { get; set; }
        public bool Onay { get; set; }
        public SorularModel Soru { get; set; }
    }
    
}