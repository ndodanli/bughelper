using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugHelper.Models
{
    public class ProfilModel
    {
        public List<SorularModel> KullaniciSorulari { get; set; }
        public List<SorularModel> KullaniciCevapSorulari { get; set; }
        public List<CevaplarModel> KullaniciCevaplari { get; set; }
        public List<SorularModel> FavoriListesi { get; set; }
        public List<TakipciModel> Takipciler { get; set; }
        public List<TakipEttikleriModel> TakipEdilenler { get; set; }
        public string KullaniciAdi { get; set; }
        public bool TakipteMi = false;
    }
}