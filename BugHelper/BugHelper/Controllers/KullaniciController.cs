using BugHelper.Identity;
using BugHelper.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace BugHelper.Controllers
{

    public class KullaniciController : Controller
    {

        private SorularContext sc = new SorularContext();
        private IdentityDataContext dc = new IdentityDataContext();
        private ProfilModel model = new ProfilModel();
        private Profil profilGuncelle = new Profil();
        private Register register = new Register();
        private UserManager<ApplicationUser> userManager;

        [Route("kullanici/{kullaniciAdi}/favoriler")]//Route, bu metod çalıştığında metodu çağıran görevin gönderdiği url değerlerini alır({} içindeki kısım, metodu çağıran görevden gelir, diğer kısımlar sabittir(bkz. ~/Views/Sorular/Soru.cshtml satır 75))
        public ActionResult FavoriSorular()
        {
            ProfilModel model = new ProfilModel(); //Favori soruları göstermek için kullandığımız view'e göndereceğimiz model için bir referans ve obje oluşturduk(Ctrl'e basılı tutarak ProfilModel'e tıklarsanız model için kullandığımız sınıfı görebilirsiniz)
            model.FavoriListesi = new List<SorularModel>(); //ProfilModel sınıfından türettiğimiz objenin FavoriListesi(SorularModel nesnelerinden oluşan bir List(liste)) değişkeninine bir obje(new List<SorularModel>()) atadık çünkü, eğer bu listeyi kullanmadan önce tanımlamaz isek metoddaki kodlar okunurken bu listeyi kullandığımız yerde listeyi null olarak tanımlayıp kullanamayacak ve bize NullReferenceException hatası dönecektir
            if (dc.FavoriSorular.Where(i => i.ApplicationUser.UserName == User.Identity.Name).FirstOrDefault() != null) //dc referansını identity contexte(identity databasei için) ulaşmak için kullanırız(bkz. satır 17).dc.FavorSorular DbSet'inden(FavoriSorularModel nesnelerinden oluşan DbSet(databasedeki tablolar)), ApplicationUser.UserName'i giriş yapan kullanıcının UserName'ine eşit olan FavoriSorular nesnelerinin ilkini FirstOrDefault()(bulduğu ilk nesneyi alır) ile alıyoruz ve null olup olmadığınız kontrol ediyoruz(bkz. NullReferenceException)
            { //User.Identity.Name, giriş yapan kullanıcının kullanıcı adını verir
                foreach (var item in dc.FavoriSorular.Where(i => i.ApplicationUser.UserName == User.Identity.Name).ToList())//bu linq sorgusu da diğerleri ile benzer bu sorgu ile bulduğumuz listenin içinde foreach ile dönüyoruz
                {
                    foreach (var soru in sc.Sorular.ToList())//bu döngüdeki amaç favori soruları göstermek üzere kullanacağımız view için kullandığımız ProfilModel modelinin içindeki FavoriListesine(bkz. model.FavoriListesi), linq sorgusu ile bulduğumuz listenin içindeki FavoriSorular(bu bir int değer ve soru idsini temsil ediyor(bkz. Models klasöründeki UserModel içindeki FavoriSorularModel)) değerleri ile sc.Sorular(soruların database'i) üzerinde döndüğümüz soru.Id değerleri eşit olanları atmak
                    {
                        if (soru.Id == item.FavoriSorular)
                        {
                            model.FavoriListesi.Add(soru);
                        }
                    }
                }
                return View(model);
            }
            return View();
        }
        [Route("profil")]
        public ActionResult Profil()//kullanıcının, profilinde "Düzenle"ye bastığında gideceği view(methoda sağ tıklayıp "Go To View"i seçerek bakabilirsiniz)
        {
            return View();
        }
        [Route("kullanici/{takmaAd}/{KullaniciAdi}")]
        public ActionResult ProfilGoruntule()
        {
            string kullaniciAdi = (RouteData.Values["KullaniciAdi"].ToString());//urlye gelen KullaniciAdi değerini çekip oluşturduğumuz stringe daha sonra kullanmak üzere atıyoruz
            model.KullaniciAdi = kullaniciAdi;
            model.KullaniciSorulari = sc.Sorular.Where(i => i.SoruSahibi == kullaniciAdi).OrderByDescending(i => i.Id).Take(5).ToList();//sorular database'inden, profiline girdiğimiz kullanıcı adı ile ilişkili soruları Id değerine göre azalan şekilde sırala(burada amaç listeyi ters çevirip son 5 soruyu almak) ve 5'ini al
            model.KullaniciCevaplari = sc.Cevaplar.Where(i => i.CevapSahibi == kullaniciAdi).OrderByDescending(i => i.Id).Take(5).ToList();
            model.KullaniciCevapSorulari = new List<SorularModel>(); //(NullReferenceException)
            if (dc.Takipci.Where(i => i.ApplicationUser.UserName == kullaniciAdi).FirstOrDefault() != null)//(NullReferenceException)
            {
                model.Takipciler = dc.Takipci.Where(i => i.ApplicationUser.UserName == kullaniciAdi).ToList();
            }
            if (User.Identity.IsAuthenticated)
            {
                if (dc.TakipEttikleri.Where(i => i.ApplicationUser.UserName == kullaniciAdi).FirstOrDefault() != null)//(NullReferenceException)
                {
                    model.TakipEdilenler = dc.TakipEttikleri.Where(i => i.ApplicationUser.UserName == kullaniciAdi).ToList();//linq sorgularının mantığı aynı olduğu için bunlara açıklama yapmıyorum
                }
            }
            for (int x = 0; x < model.KullaniciCevaplari.Count; x++)
            {
                model.KullaniciCevapSorulari.Add(model.KullaniciCevaplari[x].Soru);
            }
            if (User.Identity.IsAuthenticated && dc.TakipEttikleri.Any(i => i.ApplicationUser.UserName == User.Identity.Name && i.TakipEttikleri == kullaniciAdi))//(NullReferenceException) için farklı bir kullanım.Sorgu yaparken Any kullanırsak sorgu yaptığımız yerde parantez içine aldığımız nesneden olup olmadığını boolean değer olarak söyler, bu şekilde de NullReferenceException handling yapabiliriz
            {                                   //bu kontroldeki amaç giriş yapan kullanıcının profiline girdiği kullanıcıyı takıp edip etmediğine dair bir boolean değer kullanmak ve bu değer ile view'de takip butonunu gerektiği şekilde göstermek(takip et ya da takipten çık)
                model.TakipteMi = true;
            }
            return View(model);
        }
        [Route("{kullaniciAdi}/profil")]
        public ActionResult ProfiliGuncelle() //ProfiliGuncelle() ve ProfiliGuncelleAction() adlı iki metod kullanmamın sebebi birinin route değerleri alması
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProfiliGuncelleAction(Profil gelenProfil)
        {
            dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().Ad = gelenProfil.Ad; //gelen from verilerine göre bu verilerin veritabanına aktarımı
            dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().Soyad = gelenProfil.Soyad;
            dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().IsTanimi = gelenProfil.IsTanimi;
            dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().Ulke = gelenProfil.Ulke;
            dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().Hakkinda = gelenProfil.Hakkinda;
            dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().Facebook = gelenProfil.Facebook;
            dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().Twitter = gelenProfil.Twitter;
            dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().GitHub = gelenProfil.GitHub;
            dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().LinkedIn = gelenProfil.LinkedIn;
            dc.SaveChanges();
            return View("~/Views/Kullanici/Profil.cshtml");
        }


        [Route("{kullaniciAdi}/paroladegistir")]
        public ActionResult ParolaDegistir()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ParolaDegistirAction(ParolaDegistir gelenParola)
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new IdentityDataContext()));
            userManager.PasswordValidator = new CustomPasswordValidator()
            {
                RequireDigit = true,
                RequiredLength = 8,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonLetterOrDigit = true,

            };
            var parola = dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().PasswordHash;
            if (ModelState.IsValid)
            {
                var result = userManager.ChangePassword(User.Identity.GetUserId(), gelenParola.OldPassword, gelenParola.Password);
                if (result.Succeeded)
                {
                    string mesaj = "Parolanız başarı ile değiştirildi";
                    return PartialView("_Mesaj", mesaj);
                }
                else
                {
                    foreach (var errors in result.Errors)
                    {
                        ModelState.AddModelError("", errors);
                    }
                }
            }
            return PartialView("ParolaDegistir");
        }
        [HttpPost]
        public PartialViewResult TakipEt(string kullaniciAdi)
        {
            dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().TakipEttikleri.Add(new TakipEttikleriModel //Burada Foreign Key'in kullanımı vardır. Görüldüğü üzere Users üzerinden TakipEttikleri ekleniyor ve bu ikisi Entity Framework tarafından foreign key ile ilişkilendiriliyor. Daha sonra bunlara ilişkin değerlere birbirleri üzerinden erişebiliyoruz 
            {                                       //alttaki kod satırları da benzer şekildeki linq sorgularından ibarettir
                TakipEttikleri = kullaniciAdi
            });
            dc.Users.Where(i => i.UserName == kullaniciAdi).FirstOrDefault().Takipci.Add(new TakipciModel
            {
                Takipci = User.Identity.Name
            });
            dc.SaveChanges();
            string mesaj = kullaniciAdi + " adlı kullanıcıyı takip ediyorsunuz";
            return PartialView("_Mesaj", mesaj);
        }
        public PartialViewResult TakiptenCik(string kullaniciAdi)
        {
            dc.TakipEttikleri.Remove(dc.TakipEttikleri.Where(i => i.ApplicationUser.UserName == User.Identity.Name && i.TakipEttikleri == kullaniciAdi).FirstOrDefault());
            dc.Takipci.Remove(dc.Takipci.Where(i => i.ApplicationUser.UserName == kullaniciAdi && i.Takipci == User.Identity.Name).FirstOrDefault());
            dc.SaveChanges();
            string mesaj = kullaniciAdi + " adlı kullanıcıyı takipten çıktınız";
            return PartialView("_Mesaj", mesaj);
        }

        [HttpPost]
        public ActionResult FavoriyeEkle(int soruId)
        {
            dc.Users.Where(i => i.UserName == User.Identity.Name).FirstOrDefault().FavoriSorular.Add(new FavoriSorularModel
            {
                FavoriSorular = soruId
            });
            dc.SaveChanges();
            string mesaj = "Favorilere Eklendi";
            return PartialView("_Mesaj", mesaj);
        }
        [HttpPost]
        public ActionResult FavoridenCikar(int soruId)
        {
            dc.FavoriSorular.Remove(dc.FavoriSorular.Where(i => i.ApplicationUser.UserName == User.Identity.Name && i.FavoriSorular == soruId).FirstOrDefault());
            dc.SaveChanges();
            string mesaj = "Favorilerden Çıkarıldı";
            return PartialView("_Mesaj", mesaj);
        }



    }
}