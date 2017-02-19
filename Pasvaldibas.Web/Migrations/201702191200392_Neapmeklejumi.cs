namespace Pasvaldibas.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Neapmeklejumi : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Apmeklejums", "NeapmeklesanasIemesls", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Apmeklejums", "NeapmeklesanasIemesls");
        }
    }
}
