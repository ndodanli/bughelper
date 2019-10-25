using BugHelper.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace BugHelper.Identity
{
    public class ApplicationUser: IdentityUser
    {
        public ApplicationUser()
        {
            Takipci = new List<TakipciModel>();
            TakipEttikleri = new List<TakipEttikleriModel>();
            FavoriSorular = new List<FavoriSorularModel>();
        }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string IsTanimi { get; set; }
        public string Ulke { get; set; }
        public DateTime KatilmaTarihi { get; set; }
        public int SoruSayisi { get; set; }
        public int CevapSayisi { get; set; }
        public string Facebook { get; set; }
        public string LinkedIn { get; set; }
        public string Twitter { get; set; }
        public string GitHub { get; set; }
        public string Hakkinda { get; set; }
        public bool Kontrol { get; set; }
        public List<TakipciModel> Takipci { get; set; }
        public List<TakipEttikleriModel> TakipEttikleri { get; set; }
        public List<FavoriSorularModel> FavoriSorular { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}