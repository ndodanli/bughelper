namespace BugHelper.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Ad : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FavoriSorularModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FavoriSorular = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.TakipciModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Takipci = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.TakipEttikleriModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TakipEttikleri = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            AddColumn("dbo.AspNetUsers", "Kontrol", c => c.Boolean(nullable: false));
            DropColumn("dbo.AspNetUsers", "Takipci");
            DropColumn("dbo.AspNetUsers", "TakipEttikleri");
            DropColumn("dbo.AspNetUsers", "FavoriSorular");
            DropColumn("dbo.AspNetUsers", "SilinmeDurumu");
            DropColumn("dbo.AspNetUsers", "BanDurumu");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "BanDurumu", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "SilinmeDurumu", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "FavoriSorular", c => c.String());
            AddColumn("dbo.AspNetUsers", "TakipEttikleri", c => c.String());
            AddColumn("dbo.AspNetUsers", "Takipci", c => c.String());
            DropForeignKey("dbo.TakipEttikleriModels", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TakipciModels", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.FavoriSorularModels", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.TakipEttikleriModels", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.TakipciModels", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.FavoriSorularModels", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.AspNetUsers", "Kontrol");
            DropTable("dbo.TakipEttikleriModels");
            DropTable("dbo.TakipciModels");
            DropTable("dbo.FavoriSorularModels");
        }
    }
}
