using BugHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BugHelper.Controllers
{
    public class HomeController : Controller
    {
        private SorularContext sc = new SorularContext();
        private SorularModel sm = new SorularModel();
        [RequireHttps]
        public ActionResult Index() //kullanıcı herhangi bir filtreleme işlemi talep etmedi ise gözükecek soruları, onaylanmış sorulardan tarihe göre azalan şekilde liste yaparak gönderiyoruz
        {
            var model = sc.Sorular.Where(i => i.Onay == true);
            return View(model.OrderByDescending(i => i.SorulmaTarihi).ToList());
        }
        public ActionResult Ara(string arananString) //onaylı olan sorulardan, başlığında veya içeriğinde aranan string'i barındıran soruları liste halinde gönderiyoruz
        {
            var model = sc.Sorular.Where(i => i.SoruBaslik.Contains(arananString) || i.SoruIcerik.Contains(arananString) && i.Onay == true);
            return View("Index",model.OrderByDescending(i => i.SorulmaTarihi).ToList());
        }
        [HttpPost]
        public PartialViewResult SorularFiltre(string etiketFiltre)//burada da benzer filteleme işlemlerini, kullanıcının taleplerine göre uygulayıp gönderiyoruz
        {
            if (etiketFiltre.Equals("Java"))
            {
                var model = sc.Sorular.Where(i => i.KodlamaDili == "Java" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToList();
                return PartialView(model);
            }
            else if (etiketFiltre.Equals("C++"))
            {
                var model = sc.Sorular.Where(i => i.KodlamaDili == "C++" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToList();
                return PartialView(model);
            }
            else if (etiketFiltre.Equals("C#"))
            {
                var model = sc.Sorular.Where(i => i.KodlamaDili == "C#" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToList();
                return PartialView(model);
            }
            else if(etiketFiltre.Equals("Cevapsız Sorular")){
                var model = sc.Sorular.Where(i => i.CevapSayisi == 0 && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToList();
                return PartialView(model);
            }
            else if(etiketFiltre.Equals("En çok beğenilen"))
            {
                var model = sc.Sorular.Where(i => i.Onay == true).OrderByDescending(i => i.Deger);
                return PartialView(model);
            }
            else
            {
                var model = sc.Sorular;
                return PartialView(model.OrderByDescending(i => i.SorulmaTarihi).ToList());
            }
            
        }
    }
}