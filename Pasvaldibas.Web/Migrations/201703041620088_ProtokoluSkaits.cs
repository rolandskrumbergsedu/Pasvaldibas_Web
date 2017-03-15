namespace Pasvaldibas.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProtokoluSkaits : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pasvaldibas", "ProtokoluSkaits", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pasvaldibas", "ProtokoluSkaits");
        }
    }
}
