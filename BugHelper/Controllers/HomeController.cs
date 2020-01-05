using BugHelper.Identity;
using BugHelper.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static BugHelper.Models.SorularPaged;

namespace BugHelper.Controllers
{
    public class HomeController : Controller
    {
        private SorularContext sc = new SorularContext();
        private IdentityDataContext dc = new IdentityDataContext();
        SorularPaged model = new SorularPaged();
        [RequireHttps]
        public ActionResult Index(int? page) //kullanıcı herhangi bir filtreleme işlemi talep etmedi ise gözükecek soruları, onaylanmış sorulardan tarihe göre azalan şekilde liste yaparak gönderiyoruz
        {
            Random rnd = new Random();
            if (User.Identity.IsAuthenticated) {
                model.SoruIzleyici = dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault();
            }
            model.PagedList = sc.Sorular.Where(i => i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
            model.Sorular = sc.Sorular.Where(i => i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
            model.SoruSayisi = sc.Sorular.Where(i => i.Onay == true).Count();
            model.Etiketler = new List<EtiketListesi>();
            foreach (var item in sc.Sorular.Where(i => i.Onay == true).Select(a => a.KodlamaDili).Distinct().ToList())
            {
                model.Etiketler.Add(new EtiketListesi
                {
                    Etiket = item,
                    SoruSayisi = sc.Sorular.Where(i => i.KodlamaDili == item).Count()
                });
            }
            
            string[] temp = new string[model.Etiketler.Count()];
            string[] renkler = { "default", "primary", "secondary", "danger", "dark", "info", "success" };
            for (int i = 0; i < model.Etiketler.Count(); i++)
            {
                int index = rnd.Next(renkler.Count());
                temp[i] = renkler[index];
            }
            model.EtiketRenkleri = temp;

            foreach (var item in model.Sorular)
            {
                if(item.SoruSahibi == "Misafir") { continue; }
                item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault()?.Path;
            }
            return View(model);
        }
        public ActionResult Ara(string arananString, int? page) //onaylı olan sorulardan, başlığında veya içeriğinde aranan string'i barındıran soruları liste halinde gönderiyoruz
        {
            Random rnd = new Random();
            if (User.Identity.IsAuthenticated) //kullanıcı giriş yapmış ise
            {
                model.SoruIzleyici = dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault(); //SoruIzleyiciye kullanıcı ata(bilgilerini gösterebilmek için)
            }
            model.PagedList = sc.Sorular.Where(i => i.Onay == true && (i.SoruBaslik.Contains(arananString) || i.SoruIcerik.Contains(arananString))).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);//sayfalama işlemi için aranan değerleri soru başlığında ve soru içeriğinde ara, uyumlu ve onaylı olanları alıp sayfalanmış listeye çevir
            model.Sorular = sc.Sorular.Where(i => i.Onay == true && (i.SoruBaslik.Contains(arananString) || i.SoruIcerik.Contains(arananString)) ).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();//sayfalama işlemi için aranan değerleri soru başlığında ve soru içeriğinde ara, uyumlu ve onaylı olanları 10'ar 10'ar alıp listeye çevir
            model.SoruSayisi = sc.Sorular.Count(); //soru sayısını bul ve ata
            model.Etiketler = new List<EtiketListesi>(); //yeni bir etiket listesine veritabanındaki etiketleri ata(her bir etiketten 1 tane olacak şekilde)
            foreach (var item in sc.Sorular.Where(i => i.Onay == true).Select(a => a.KodlamaDili).Distinct().ToList())
            {
                model.Etiketler.Add(new EtiketListesi
                {
                    Etiket = item,
                    SoruSayisi = sc.Sorular.Where(i => i.KodlamaDili == item).Count()
                });
            }

            string[] temp = new string[model.Etiketler.Count()];                                            //sayfaya her girildiğinde rastgele renklerin etiketlere atanması için oluşturupmuş yapı
            string[] renkler = { "default", "primary", "secondary", "danger", "dark", "info", "success" };
            for (int i = 0; i < model.Etiketler.Count(); i++)
            {
                int index = rnd.Next(renkler.Count());
                temp[i] = renkler[index];
            }
            model.EtiketRenkleri = temp;
                                                                                                             //sayfaya her girildiğinde rastgele renklerin etiketlere atanması için oluşturupmuş yapı


            foreach (var item in model.Sorular) //soru sahiplerinin resimlerini yüklüyoruz, eğer misafir ise 'continue' kullanıyoruz çünkü NullException hatası istemiyoruz
            {
                if (item.SoruSahibi == "Misafir") { continue; }
                item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault()?.Path;
            }
            return View("Index",model);
        }
        [HttpPost]
        public PartialViewResult SorularFiltre(string etiketFiltre, int? page)//burada da benzer filteleme işlemlerini, kullanıcının taleplerine göre uygulayıp gönderiyoruz
        {
            foreach (var etiket in sc.Etiketler.ToList())
            {
                if (etiketFiltre.Equals(etiket.KodlamaDili))
                {
                    model.PagedList = sc.Sorular.Where(i => i.KodlamaDili == etiket.KodlamaDili && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
                    model.Sorular = sc.Sorular.Where(i => i.KodlamaDili == etiket.KodlamaDili && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                    foreach (var item in model.Sorular)
                    {
                        if (item.SoruSahibi == "Misafir") { continue; }
                        item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                    }
                    return PartialView(model);
                }
            }
            if(etiketFiltre.Equals("Cevapsızlar")){
                model.PagedList = sc.Sorular.Where(i => i.CevapSayisi == 0 && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
                model.Sorular = sc.Sorular.Where(i => i.CevapSayisi == 0 && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                foreach (var item in model.Sorular)
                {
                    if (item.SoruSahibi == "Misafir") { continue; }
                    item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                }
                return PartialView(model);
            }
            else if(etiketFiltre.Equals("Beğenilenler"))
            {
                model.PagedList = sc.Sorular.Where(i => i.Onay == true).OrderByDescending(i => i.Deger).ToPagedList(page ?? 1, 10);
                model.Sorular = sc.Sorular.Where(i => i.Onay == true).OrderByDescending(i => i.Deger).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                foreach (var item in model.Sorular)
                {
                    if (item.SoruSahibi == "Misafir") { continue; }
                    item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                }
                return PartialView(model);
            }
            else if (etiketFiltre.Equals("Pasif"))
            {
                model.PagedList = sc.Sorular.Where(i => i.Onay == false).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
                model.Sorular = sc.Sorular.Where(i => i.Onay == false).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                return PartialView(model);
            }
            else
            {
                model.PagedList = sc.Sorular.Where(i => i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
                model.Sorular = sc.Sorular.Where(i => i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                foreach (var item in model.Sorular)
                {
                    if (item.SoruSahibi == "Misafir") { continue; }
                    item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                }
                return PartialView(model);
            }
        }
        public ActionResult Hakkimizda()
        {
            return View();
        }
        public ActionResult Iletisim()
        {
            return View();
        }
    }
}