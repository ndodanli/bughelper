using BugHelper.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace BugHelper.Identity
{
    public class IdentityDataContext:IdentityDbContext<ApplicationUser>
    {
        public IdentityDataContext() : base("identityConnection")
        {
            Database.SetInitializer(new IdentityVTInitializer());
        }
        public DbSet<TakipciModel> Takipci { get; set; }
        public DbSet<TakipEttikleriModel> TakipEttikleri { get; set; }
        public DbSet<FavoriSorularModel> FavoriSorular { get; set; }
    }
}