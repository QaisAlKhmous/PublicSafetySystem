namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addexceptiontoissuance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Issuances", "IsException", c => c.Boolean(nullable: false));
            AddColumn("dbo.Issuances", "ExceptionReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Issuances", "ExceptionReason");
            DropColumn("dbo.Issuances", "IsException");
        }
    }
}
