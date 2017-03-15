namespace Pasvaldibas.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeputatuSkaits : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pasvaldibas", "DeputatuSkaits", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pasvaldibas", "DeputatuSkaits");
        }
    }
}
