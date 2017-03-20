namespace Pasvaldibas.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GPS : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pasvaldibas", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 7));
            AddColumn("dbo.Pasvaldibas", "Longtitude", c => c.Decimal(nullable: false, precision: 18, scale: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pasvaldibas", "Longtitude");
            DropColumn("dbo.Pasvaldibas", "Latitude");
        }
    }
}
