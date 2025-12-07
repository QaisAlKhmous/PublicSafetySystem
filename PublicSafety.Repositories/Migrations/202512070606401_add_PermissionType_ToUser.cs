namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_PermissionType_ToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Type");
        }
    }
}
