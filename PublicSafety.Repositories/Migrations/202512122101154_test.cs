namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ItemLogs", "CreatedById");
            AddForeignKey("dbo.ItemLogs", "CreatedById", "dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLogs", "CreatedById", "dbo.Users");
            DropIndex("dbo.ItemLogs", new[] { "CreatedById" });
        }
    }
}
