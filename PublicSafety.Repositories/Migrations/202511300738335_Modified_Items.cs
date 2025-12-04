namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modified_Items : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Items", "AddedById", c => c.Guid(nullable: false));
            CreateIndex("dbo.Items", "AddedById");
            AddForeignKey("dbo.Items", "AddedById", "dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "AddedById", "dbo.Users");
            DropIndex("dbo.Items", new[] { "AddedById" });
            DropColumn("dbo.Items", "AddedById");
            DropColumn("dbo.Items", "IsActive");
        }
    }
}
