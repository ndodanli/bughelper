using BugHelper.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugHelper.Controllers
{
    public class CevaplarController : Controller
    {
        public SorularContext sc = new SorularContext();
        public CevaplarModel cm = new CevaplarModel();

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public PartialViewResult Cevapla(int soruIdCevap, string gelenCevap) //soruların cevaplandığında kullanacağımız metod
        {
            if (User.Identity.IsAuthenticated) //eğer kullanıcı giriş yapmış ise
            {
                var cevap = new CevaplarModel //gelen cevap için bir CevapModel objesi oluşturduk ve gelen cevabın değerlerini bu objeye attık
                {
                    Cevap = gelenCevap,
                    CevapSahibi = User.Identity.Name,
                    CevapTarihi = System.DateTime.Now,
                    Onay = true //kullanıcı giriş yaptığı için onay durumunu true yapıyoruz
                };
                sc.Sorular.Where(i => i.Id == soruIdCevap).FirstOrDefault().Cevaplar.Add(cevap); //soruIdCevap, view'den gelen bir değerdir ve kullanıcının cevabı yazdığı sorunun ID'sidir.Bu soruyu veritabanından bulup, soru üzerinden Cevaplar'a bu cevabı ekliyoruz.Entity framework, foreign key(tabloları birbiri ile ilişkilendirmek için kullanılan bir anahtar diyebiliriz) ile soru ile cevabı bağlıyor ve daha sonradan bu değişkenlere birbirleri üzerinden erişebiliyoruz
                sc.SaveChanges(); //database'de yaptığımız değişiklikleri kaydediyoruz
            }
            else
            {
                var cevap = new CevaplarModel //kullanıcı giriş yapmamışsa onay durumunu false(giriş yapmadığı için), cevabın sahibini de misafir olarak belirliyoruz
                {
                    Cevap = gelenCevap,
                    CevapSahibi = "Misafir",
                    CevapTarihi = System.DateTime.Now,
                    Onay = false
                };
                sc.Sorular.Where(i => i.Id == soruIdCevap).FirstOrDefault().Cevaplar.Add(cevap);
                sc.SaveChanges();
            }
            if (User.Identity.IsAuthenticated) //eğer giriş yapmışsa kullanıcının girdiği cevabı soruya ekle
            {
                var model = sc.Cevaplar.Where(i => i.Soru.Id == soruIdCevap).ToList();
                return PartialView("Cevapla",model);
                
            }
            else//giriş yapmamışsa mesajı bastırıp cevabı bir yönetici onaylayana kadar bekleme listesine alıyoruz
            {
                string mesaj = "Giriş yapmadığınız için cevabınız onaylanana dek kullanıcılara kapalı olacaktır.";
                return PartialView("_Mesaj", mesaj);
            }
            
        }
        public PartialViewResult CevaplariGoster() {
            return PartialView();
        }
    }
}