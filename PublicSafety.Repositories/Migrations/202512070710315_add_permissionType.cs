namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_permissionType : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Type", c => c.Int(nullable: false));
        }
    }
}
