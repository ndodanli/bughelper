using BugHelper.Identity;
using BugHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace BugHelper.Controllers
{
    public class SorularController : Controller
    {
        private IdentityDataContext dc = new IdentityDataContext();
        private List<SorularModel> bekletilenListesi = new List<SorularModel>();
        private SorularContext sc = new SorularContext();
        [HttpGet]
        public ActionResult SoruSor()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SoruSor(SorularModel soruSor, string button) //soru sorma view'imiz için kullandığımız metoddur
        {

            if (User.Identity.IsAuthenticated)
            {
                var soru = new SorularModel
                {
                    SoruSahibi = User.Identity.Name,
                    KodlamaDili = soruSor.KodlamaDili,
                    SoruBaslik = soruSor.SoruBaslik,
                    SoruIcerik = soruSor.SoruIcerik,
                    SorulmaTarihi = System.DateTime.Now,
                    Onay = true
                };
                sc.Sorular.Add(soru);
                sc.SaveChanges();
            }
            else
            {
                var soru = new SorularModel
                {
                    SoruSahibi = "Misafir",
                    KodlamaDili = soruSor.KodlamaDili,
                    SoruBaslik = soruSor.SoruBaslik,
                    SoruIcerik = soruSor.SoruIcerik,
                    SorulmaTarihi = System.DateTime.Now,
                    Onay = false
                };
                sc.Sorular.Add(soru);
                sc.SaveChanges();
            }
            return View();
        }
        [Authorize(Roles = "admin")]
        [HttpGet] //metod GET işlemi sağladığından eklendi
        public ActionResult BeklemeListesi() //beklemelistesine bekletilen soruları ve cevapları gönderiyoruz
        {
            SoruCevapModel model = new SoruCevapModel();
            model.SorularModelListe = sc.Sorular.Where(i => i.Onay == false).ToArray();
            model.CevaplarModelListe = sc.Cevaplar.Where(i => i.Onay == false).ToArray();
            return View(model);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult BeklemeListesi(SoruCevapModel model) //bekleme listesinden seçtiğimiz sorulara ya da cevaplara ilişkin işlemleri gerçekleştirdiğimiz metodumuz
        {
            if (model.SorularModelListe != null)
            {
                for (int i = 0; i < model.SorularModelListe.Count(); i++)
                {
                    foreach (var item in sc.Sorular.ToList())
                    {
                        if (item.Id == model.SorularModelListe.ElementAt(i).Id && model.SorularModelListe.ElementAt(i).Onay)
                        {
                            item.Onay = true;
                            sc.SaveChanges();
                        }
                    }
                }
                string mesaj = "Sorular Onaylandı";
                return PartialView("_Mesaj", mesaj);
            }
            if (model.CevaplarModelListe != null)
            {
                for (int i = 0; i < model.CevaplarModelListe.Count(); i++)
                {
                    foreach (var item in sc.Cevaplar.ToList())
                    {
                        if (item.Id == model.CevaplarModelListe[i].Id && model.CevaplarModelListe[i].Onay)
                        {
                            item.Onay = true;

                            sc.SaveChanges();
                        }
                    }
                }
                string mesaj = "Cevaplar Onaylandı";
                return PartialView("_Mesaj", mesaj);
            }
            return View();
        }
        public ActionResult Ara(string arananString)
        {
            return View();
        }

        [Route("Sorular/{baslik}/{SoruId}")]
        public ActionResult Soru() //Herhangi bir sorunun başlığına tıkladığımızda ya da url'den yazıp girdiğimizde çağırdığımız metoddur, soruya ilişkin detayları gösterir
        {
            int id = Convert.ToInt32(RouteData.Values["SoruId"]);
            sc.Sorular.Where(i => i.Id == id).FirstOrDefault().TiklanmaSayisi++;
            sc.SaveChanges();
            SoruCevapModel model = new SoruCevapModel(); //Bu metod için kullandığımız modelimiz(Ctrl'e basılı tutaral SoruCevapModel'e tıklarsanız içeriğini görebilirsiniz)
            model.SoruId = id;
            model.SoruCevapContext = sc;
            model.CevaplarModelForSoru = sc.Cevaplar.Where(i => i.Soru.Id == id).ToList();
            if (User.Identity.IsAuthenticated)
            {
                if (dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().FavoriSorular != null) //NullReferenceException
                {
                    foreach (var item in dc.FavoriSorular.Where(i => i.ApplicationUser.UserName == User.Identity.Name).ToList()) //giriş yapan kullanıcının, profiline girdiği kullanıcıyı önceden takip edip etmediğini anlamak ve view'imizi buna göre ayarlamak için kullandığımız bir kontrol(Takip Et ya da Takipten Çık)
                    {
                        if (item.FavoriSorular == id)
                        {
                            model.FavorideMi = true;
                        }
                    }
                }
            }
            return View(model);
        }
        public ActionResult SoruOyla(int soruId, string oy) //Soruları oylamak için kullandığımız metod
        {
            SoruCevapModel scm = new SoruCevapModel();
            scm.SoruCevapContext = sc;
            scm.SoruId = soruId;
            if (oy.Equals("arti")) //1-Eğer kullanıcı artı oy vermek istiyorsa
            {
                if (sc.EksiOy.Any(i => i.Soru.Id == soruId && i.EksiOySahibi == User.Identity.Name)) //1a-Kullanıcı daha önce eksi oy verdiyse
                {
                    sc.Sorular.Where(i => i.Id == soruId).FirstOrDefault().ArtiOylar.Add(new ArtiOy //kullanıcının artı oyunu veritabanına at
                    {
                        ArtiOySahibi = User.Identity.Name
                    });
                    sc.EksiOy.Remove(sc.EksiOy.Where(i => i.Soru.Id == soruId && i.EksiOySahibi == User.Identity.Name).FirstOrDefault()); //Kullanıcının eksi oyunu veritabanından sil
                    sc.Sorular.Where(i => i.Id == soruId).FirstOrDefault().Deger += 2; //Sorunun oyunu 2 arttır
                    sc.SaveChanges();
                    return PartialView(scm);
                }
                else if (sc.ArtiOy.Any(i => i.Soru.Id == soruId && i.ArtiOySahibi == User.Identity.Name)) //1b-Kullanıcı daha önce artı oy verdiyse
                {
                    sc.ArtiOy.Remove(sc.ArtiOy.Where(i => i.Soru.Id == soruId && i.ArtiOySahibi == User.Identity.Name).FirstOrDefault());//Kullanıcının artı oyunu veritabanından sil
                    sc.Sorular.Where(i => i.Id == soruId).FirstOrDefault().Deger -= 1; //Sorunun oyunu 1 azalt
                    sc.SaveChanges();
                    return PartialView(scm);
                }
                else //1c-Kullanıcı daha önce oy vermediyse
                {
                    sc.Sorular.Where(i => i.Id == soruId).FirstOrDefault().ArtiOylar.Add(new ArtiOy //Kullanıcının artı oyunu veritabanına at
                    {
                        ArtiOySahibi = User.Identity.Name
                    });
                    sc.Sorular.Where(i => i.Id == soruId).FirstOrDefault().Deger += 1; //Sorunun oyunu 1 arttır
                    sc.SaveChanges();
                    return PartialView(scm);
                }

            }
            else //2-Eğer kullanıcı eksi oy vermek istiyorsa
            {
                if (sc.ArtiOy.Any(i => i.Soru.Id == soruId && i.ArtiOySahibi == User.Identity.Name)) //2a-Kullanıcı daha önce artı oy verdiyse
                {
                    sc.Sorular.Where(i => i.Id == soruId).FirstOrDefault().EksiOylar.Add(new EksiOy //Kullanıcının eksi oyunu veritabanına at
                    {
                        EksiOySahibi = User.Identity.Name
                    });
                    sc.ArtiOy.Remove(sc.ArtiOy.Where(i => i.Soru.Id == soruId && i.ArtiOySahibi == User.Identity.Name).FirstOrDefault()); //Kullanıcının artı oyunu veritabanından sil
                    sc.Sorular.Where(i => i.Id == soruId).FirstOrDefault().Deger -= 2; //Sorunun oyunu 2 azalt
                    sc.SaveChanges();
                    return PartialView(scm);
                }
                else if (sc.EksiOy.Any(i => i.Soru.Id == soruId && i.EksiOySahibi == User.Identity.Name)) //2b-Kullanıcı daha önce eksi oy verdiyse
                {
                    sc.EksiOy.Remove(sc.EksiOy.Where(i => i.Soru.Id == soruId && i.EksiOySahibi == User.Identity.Name).FirstOrDefault()); //Kullanıcının eksi oyunu veritabanından sil
                    sc.Sorular.Where(i => i.Id == soruId).FirstOrDefault().Deger += 1; //Sorunun değerini 1 arttır
                    sc.SaveChanges();
                    return PartialView(scm);
                }
                else //2c-Kullanıcı daha önce oy vermediyse
                {
                    sc.Sorular.Where(i => i.Id == soruId).FirstOrDefault().EksiOylar.Add(new EksiOy //Kullanıcının eksi oyunu veritabanına at
                    {
                        EksiOySahibi = User.Identity.Name
                    });
                    sc.Sorular.Where(i => i.Id == soruId).FirstOrDefault().Deger -= 1; //Sorunun değerini 1 azalt
                    sc.SaveChanges();
                    return PartialView(scm);
                }
            }
        }
    }
}
