using BugHelper.Identity;
using BugHelper.Models;
using PagedList;
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
        private IdentityDataContext dc = new IdentityDataContext();
        SorularPaged model = new SorularPaged();
        [RequireHttps]
        public ActionResult Index(int? page) //kullanıcı herhangi bir filtreleme işlemi talep etmedi ise gözükecek soruları, onaylanmış sorulardan tarihe göre azalan şekilde liste yaparak gönderiyoruz
        {
            if (User.Identity.IsAuthenticated) { 
            model.SoruIzleyici = dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault();
            }
            model.PagedList = sc.Sorular.Where(i => i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
            model.Sorular = sc.Sorular.Where(i => i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
            foreach (var item in model.Sorular)
            {
                if(item.SoruSahibi == "Misafir") { continue; }
                item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
            }
            return View(model);
        }
        public ActionResult Ara(string arananString, int? page) //onaylı olan sorulardan, başlığında veya içeriğinde aranan string'i barındıran soruları liste halinde gönderiyoruz
        {
            if (User.Identity.IsAuthenticated)
            {
                model.SoruIzleyici = dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault();
            }
            model.PagedList = sc.Sorular.Where(i => i.SoruBaslik.Contains(arananString) || i.SoruIcerik.Contains(arananString) && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
            model.Sorular = sc.Sorular.Where(i => i.SoruBaslik.Contains(arananString) || i.SoruIcerik.Contains(arananString) && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
            foreach (var item in model.Sorular)
            {
                if (item.SoruSahibi == "Misafir") { continue; }
                item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
            }
            return View("Index",model);
        }
        [HttpPost]
        public PartialViewResult SorularFiltre(string etiketFiltre, int? page)//burada da benzer filteleme işlemlerini, kullanıcının taleplerine göre uygulayıp gönderiyoruz
        {
            if (etiketFiltre.Equals("Java"))
            {
                model.PagedList = sc.Sorular.Where(i => i.KodlamaDili == "Java" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
                model.Sorular = sc.Sorular.Where(i => i.KodlamaDili == "Java" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                foreach (var item in model.Sorular)
                {
                    if (item.SoruSahibi == "Misafir") { continue; }
                    item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                }
                return PartialView(model);
            }
            else if (etiketFiltre.Equals("C++"))
            {
                model.PagedList = sc.Sorular.Where(i => i.KodlamaDili == "C++" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
                model.Sorular = sc.Sorular.Where(i => i.KodlamaDili == "C++" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                foreach (var item in model.Sorular)
                {
                    if (item.SoruSahibi == "Misafir") { continue; }
                    item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                }
                return PartialView(model);
            }
            else if (etiketFiltre.Equals("AspNet"))
            {
                model.PagedList = sc.Sorular.Where(i => i.KodlamaDili == "AspNet" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
                model.Sorular = sc.Sorular.Where(i => i.KodlamaDili == "AspNet" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                foreach (var item in model.Sorular)
                {
                    if (item.SoruSahibi == "Misafir") { continue; }
                    item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                }
                return PartialView(model);
            }
            else if (etiketFiltre.Equals("C"))
            {
                model.PagedList = sc.Sorular.Where(i => i.KodlamaDili == "C" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
                model.Sorular = sc.Sorular.Where(i => i.KodlamaDili == "C" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                foreach (var item in model.Sorular)
                {
                    if (item.SoruSahibi == "Misafir") { continue; }
                    item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                }
                return PartialView(model);
            }
            else if (etiketFiltre.Equals("JavaScript"))
            {
                model.PagedList = sc.Sorular.Where(i => i.KodlamaDili == "Javascript" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
                model.Sorular = sc.Sorular.Where(i => i.KodlamaDili == "JavaScript" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                foreach (var item in model.Sorular)
                {
                    if (item.SoruSahibi == "Misafir") { continue; }
                    item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                }
                return PartialView(model);
            }
            else if (etiketFiltre.Equals("Html"))
            {
                model.PagedList = sc.Sorular.Where(i => i.KodlamaDili == "Html" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
                model.Sorular = sc.Sorular.Where(i => i.KodlamaDili == "Html" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                foreach (var item in model.Sorular)
                {
                    if (item.SoruSahibi == "Misafir") { continue; }
                    item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                }
                return PartialView(model);
            }
            else if (etiketFiltre.Equals("C#"))
            {
                model.PagedList = sc.Sorular.Where(i => i.KodlamaDili == "C#" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).ToPagedList(page ?? 1, 10);
                model.Sorular = sc.Sorular.Where(i => i.KodlamaDili == "C#" && i.Onay == true).OrderByDescending(i => i.SorulmaTarihi).Skip((page - 1 ?? 0) * 10).Take(10).ToList();
                foreach (var item in model.Sorular)
                {
                    if (item.SoruSahibi == "Misafir") { continue; }
                    item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                }
                return PartialView(model);
            }
            else if(etiketFiltre.Equals("Cevapsızlar")){
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
                foreach (var item in model.Sorular)
                {
                    if (item.SoruSahibi == "Misafir") { continue; }
                    item.SoruSahibiPath = dc.Users.Where(i => i.UserName == item.SoruSahibi).FirstOrDefault().Path;
                }
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