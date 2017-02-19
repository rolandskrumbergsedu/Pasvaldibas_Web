namespace Pasvaldibas.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Apmeklejums",
                c => new
                    {
                        ApmeklejumsId = c.Int(nullable: false, identity: true),
                        ApmeklejumaNr = c.String(),
                        Apmekleja = c.Boolean(nullable: false),
                        Datums = c.DateTime(nullable: false),
                        Deputats_DeputatsId = c.Int(),
                    })
                .PrimaryKey(t => t.ApmeklejumsId)
                .ForeignKey("dbo.Deputats", t => t.Deputats_DeputatsId)
                .Index(t => t.Deputats_DeputatsId);
            
            CreateTable(
                "dbo.Deputats",
                c => new
                    {
                        DeputatsId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Pasvaldiba_PasvaldibaId = c.Int(),
                    })
                .PrimaryKey(t => t.DeputatsId)
                .ForeignKey("dbo.Pasvaldibas", t => t.Pasvaldiba_PasvaldibaId)
                .Index(t => t.Pasvaldiba_PasvaldibaId);
            
            CreateTable(
                "dbo.Pasvaldibas",
                c => new
                    {
                        PasvaldibaId = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        CodeNr = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.PasvaldibaId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Deputats", "Pasvaldiba_PasvaldibaId", "dbo.Pasvaldibas");
            DropForeignKey("dbo.Apmeklejums", "Deputats_DeputatsId", "dbo.Deputats");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Deputats", new[] { "Pasvaldiba_PasvaldibaId" });
            DropIndex("dbo.Apmeklejums", new[] { "Deputats_DeputatsId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Pasvaldibas");
            DropTable("dbo.Deputats");
            DropTable("dbo.Apmeklejums");
        }
    }
}
