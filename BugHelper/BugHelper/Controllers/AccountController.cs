using BugHelper.Identity;
using BugHelper.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugHelper.Controllers
{
    [Authorize] // anonim erişimi engellemek için kullandığımız bir özellik 
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private IdentityDataContext dc = new IdentityDataContext();
        private KullaniciKontrolModel kkm = new KullaniciKontrolModel();
        public AccountController()
        {
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new IdentityDataContext())); // IdentityUser(SQL Serverimiz üzerinde çalışan sınıfımız)'dan türettiğimiz ApplicationUser sınıfımızın nesnelerinini taşıyan bir UserManager objesi ürettik(özet olarak bu obje, IdentityDataContext(Identity veri tabanımızın oluşumu) sınıfından bilgileri taşıyabiliyor)
            userManager.PasswordValidator = new CustomPasswordValidator() // UserManager referansını kullanarak Identity yapısının default olarak tanımladığı parola doğrulayıcı sınıfını kullanmamak için custom bir PasswordValidator nesnesi oluşturup bunu kullanıyoruz)
            {//custom PasswordValidator'ımızın özelliklerini tanımlıyoruz
                RequireDigit = true, //içerisinde sayı olacak              Ctrl'e basılı tutarak  CustomPasswordValidator'a tıklarsanız custom olarak yazdığım parola doğrulama metodunu görebilirsiniz
                RequiredLength = 8, //en az 8 karakter olmalı
                RequireLowercase = true, //küçük harf olmalı
                RequireUppercase = true, //büyük harf olmalı
                RequireNonLetterOrDigit = true, //özel karakter ve rakam olmalı
            };
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                RequireUniqueEmail = true, //aynı e-mailden 1 tane olabilir
                AllowOnlyAlphanumericUserNames = false // alfanümerik(içinde rakam ve a-z arasında karakter bulunan) olmayan kullanıcı adlarını da kabul etmesi için false verdik
            };
        }
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost] //Metod, Post işlemi sağladığından eklendi. [HttpPost] önemli bilgileri geçirmek için kullanılır(örn.parola), ancak [HttpGet]'ten daha yavaştır.[HttpGet] daha hızlıdır ancak verileri urlye bağlar ve kullanıcının görebileceği halde sunar.Bu neden ile biz genelde Post işlemleri yapacağız
        [ValidateAntiForgeryToken] // Sql injection saldırılarına karşı platformun aldığı bir önlem, tokenler kullanılıyor
        [AllowAnonymous] // 12. satırda eklediğimiz özellik nedeniyle, kullanıcıların bu metoda ilişkin view'e ulaşabilmeleri için anonim kullanıma izin verdik
        public ActionResult Register(Register model)
        {
            if (ModelState.IsValid) //ModelState.IsValid modelin durumunun geçerli olup olmadığını belirler. Model state modelin durumunu gösterir is valid diyerek bir nevi doğrulama yaparız. Örneğin kullanıcı custom olarak oluşturduğumuz parola oluşturma kurallarına uymaz ise ModelState.IsValid false döner ve bu hatayı foruma basar
            {
                var user = new ApplicationUser(); //yeni bir kullanıcı oluşturduk
                user.UserName = model.UserName; //kayıt formundan aldığımız kullanıcı adını oluşturduğumuz kullanıcıya atadık
                user.Email = model.Email; //aynı şekilde e-maili atadık
                user.KatilmaTarihi = System.DateTime.Now; //kullanıcı kayıt olurkenki zamanı System.DateTime.Now ile atadık(ctrl'e basılı tutarak now'a basarsanız sistemin date işlemlerine ilişkin diğer özelliklerini de görebilirsiniz)
                var result = userManager.Create(user, model.Password); //userManager'ın Create tanımlamasına oluşturduğumuz kullanıcıyı ve formdan gelen şifreyi gönderdik, eğer oluşturma başarılı olursa Result.succeed true olacak ve kullanıcıyı oluşturacak
                if (result.Succeeded)
                {
                    return RedirectToAction("Login"); //oluşturma başarılı, login view'ine yönlendir
                }
                else
                {
                    foreach (var errors in result.Errors) //başarılı olmadı, kullanıcıyı oluştururken doğrulama amaçlı kullandığımız result referansının errors'una başarılı olamama nedeni yüklendi ve bunu Modelstate'in model errorları kısmına attık
                    {
                        ModelState.AddModelError("", errors); //hata ne ise modelstate'e ekle ki forma basabilsin
                    }
                }
            }
            return View(model); 
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated) //kullanıcı girdiği sayfada gerekli role(yetki) sahip değilse
            {
                return View("Hata", new string[] { "Erişim hakkınız yok" });
            }

            ViewBag.returnUrl = returnUrl; 
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                var user = userManager.Find(model.UserName, model.Password); //userManager'ı kullanarak formda girilen kullanıcı adı ver parolaraki kullanıcıyı bul
                if (user == null) //eğer böyle bir kullanıcı yoksa
                {
                    ModelState.AddModelError("", "Yanlış kullanıcı adı veya parola");
                }
                else if(user.LockoutEnabled == false) //oluşturulan kullanıcıların LockoutEnabled(kilitleme) durumu default olarak false gelir, birini engellediğimizde(ban) bu değer true olur ve else durumundaki hatayı bastırır
                {
                        var authManager = HttpContext.GetOwinContext().Authentication; //Owin ile kimlik doğrulama işlemleri için kullanacağımız referans authManager referansı

                        var identity = userManager.CreateIdentity(user, "ApplicationCookie"); //giriş yapan kullanıcınıya bir kimlik oluşturup tarayıcısına bir Cookie(çerez) bırakıyoruz
                        var authProperties = new AuthenticationProperties()
                        {
                            IsPersistent = true //kullanıcının tarayıcısına bıraktığımız Cookie'nin süresi bitene ya da kullanıcı tarafından silinene kadar kalıcı olmasını sağladık
                        };

                        authManager.SignOut();
                        authManager.SignIn(authProperties, identity); //kullanıcıyı oluşturduğumuz kimlik ve 100. satırda set ettiğimiz authentication(kimlik doğrulama) özellikleri ile giriş yaptırdık
                        Kontrol(user); //kontrol amaçlı kullanıcıyı kontrol metoduna gönderdik

                        ViewBag.identity = identity; //Viewbag'e identity'i atadık
                        return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
                }
                else
                {
                    string mesaj = "Hesabınız engellendi";
                    return PartialView("_Mesaj",mesaj);
                }

            }

            ViewBag.returnUrl = returnUrl;
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "admin")] //sadece "admin" rolüne sahip kullanıcılar kullanabilsin
        public ActionResult KullaniciKontrol() //kullanıcıları silmek, engellemek ve engelini açmak için kullandığımız view'in metodu
        {                                      //kkm, 17.satırda admin'in kullanıcı üzerindeki işlemlerini yönetmek için kullanacağımız model için oluşturduğumuz objenin referansıdır
            kkm.Kullanicilar = dc.Users.Select(i => i).ToArray(); //veri tabanındaki bütün user'ları seçip, array'e dönüştürerek kkm.Kullanicilar'a attık().Ctrl'e basılı tutarak Kullanicilar basarsanız modeli görebilirsiniz
            kkm.Engelle = dc.Users.Where(i=>i.LockoutEnabled == false).ToArray(); //engellenmemiş kullanıcıları kkm.Engelle'ye attık
            kkm.EngelKaldir = dc.Users.Where(i => i.LockoutEnabled == true).ToArray(); //engellenmiş kullanıcıları kkm.EngelKaldir'a attık
            return View(kkm);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async System.Threading.Tasks.Task<ActionResult> KullaniciKontrolActionAsync(KullaniciKontrolModel users, string durum) //kullanıcı yönetimi için kullandığımız metod
        {
            if (users != null) //view'den metoda gelen users'in boş olması durumunda NullReferenceException hatası almamak için kontrol yapıyoruz, NullReferenceException hatası için bir çok durumda buna benzer kontroller yapacağız
            {
                if (durum.Equals("sil")) //durum silme durumu ise
                {
                    for (int i = 0; i < users.Kullanicilar.Count(); i++) //gelen users modelinin içindeki kullanıcılar dizisinin uzunluğu kadar döndürüyoruz
                    {
                        if (users.Kullanicilar[i].Kontrol == true) //view'de kullandığımız checkbox'ta işaretlenenlerin kontrol değeri true olduğundan, kontrol değeri true olan objeleri seçiyoruz
                        {
                            var user = userManager.FindById(users.Kullanicilar[i].Id); //seçtiğimiz userları, oluşturduğumuz yeni bir kullanıcı referansına yönlendirip daha sonra 145. satırda seçilen user'ı siliyoruz
                            userManager.Delete(user);
                        }
                    }
                    string mesaj = "Seçilen kullanıcılar başarı ile silindi"; //if durumu başarılı olursa bu mesajı yazdıracağız
                    return PartialView("_Mesaj", mesaj); //Shared klasöründe(paylaşımlı klasör, her sınıftan yol vermeden ulaşılabilir. bkz. _Mesaj)ki _Mesaj view'ine, yazdığımız stringi attığımız mesaj objesini model olarak gönderiyoruz
                }
                else if(durum.Equals("engelle")) //engelleme durumu için
                {
                    for (int i = 0; i < users.Engelle.Count(); i++)
                    {
                        if (users.Engelle[i].Kontrol)
                        {
                            await userManager.SetLockoutEnabledAsync(users.Engelle[i].Id, true);//yukarıdaki durumla silme durumu ile aynı, bu sayırda kullanıcının default olarak false olan LockoutEnabled değerini true yapıyoruz ve kullanıcı engelleniyor.Daha sonra tarih vermediğimiz için sınırsız banlanmış oluyor
                            await userManager.UpdateAsync(userManager.FindById(users.Engelle[i].Id));//banlanan user'ın durumunu güncelliyoruz
                        } //işlemlerimizin başında await kullanmamızın nedeni bu işlemleri asenkron olarak yapmak istememiz, aksi durumda hata ile karşılaşıyoruz
                    }
                    string mesaj = "Seçilen kullanıcılar başarı ile engellendi";
                    return PartialView("_Mesaj", mesaj);
                }
                else if(durum.Equals("engelkaldir")) //engel kaldırma durumu için
                {
                    for (int i = 0; i < users.EngelKaldir.Count(); i++)
                    {
                        if (users.EngelKaldir[i].Kontrol)
                        {
                            await userManager.SetLockoutEnabledAsync(users.EngelKaldir[i].Id, false);
                            await userManager.UpdateAsync(userManager.FindById(users.EngelKaldir[i].Id));
                        }
                    }
                    string mesaj = "Seçilen kullanıcıların engeli başarı ile kaldırıldı";
                    return PartialView("_Mesaj", mesaj);
                }
            }
            return View();
        }
        private ApplicationUser Kontrol(ApplicationUser identity)
        {
            return identity;
        }
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut(); //kullanıcıyı çıkış yaptırıp kimlik doğrulamasını sona erdiriyoruz

            return RedirectToAction("Login"); //login sayfasına yönlendiriyoruz
        }

    }
}
