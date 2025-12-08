namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_approvedBy_changeRequests : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChangeRequests", "ApprovedById", c => c.Guid());
            AlterColumn("dbo.ChangeRequests", "ApprovedDate", c => c.DateTime());
            CreateIndex("dbo.ChangeRequests", "ApprovedById");
            AddForeignKey("dbo.ChangeRequests", "ApprovedById", "dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChangeRequests", "ApprovedById", "dbo.Users");
            DropIndex("dbo.ChangeRequests", new[] { "ApprovedById" });
            AlterColumn("dbo.ChangeRequests", "ApprovedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.ChangeRequests", "ApprovedById");
        }
    }
}
