namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ChangeRequestsTable1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChangeRequests", "ApprovedById", c => c.Guid(nullable: false));
            AddColumn("dbo.ChangeRequests", "ApprovedBy_UserId", c => c.Guid());
            AlterColumn("dbo.ChangeRequests", "ApprovedDate", c => c.DateTime());
            CreateIndex("dbo.ChangeRequests", "ApprovedBy_UserId");
            AddForeignKey("dbo.ChangeRequests", "ApprovedBy_UserId", "dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChangeRequests", "ApprovedBy_UserId", "dbo.Users");
            DropIndex("dbo.ChangeRequests", new[] { "ApprovedBy_UserId" });
            AlterColumn("dbo.ChangeRequests", "ApprovedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.ChangeRequests", "ApprovedBy_UserId");
            DropColumn("dbo.ChangeRequests", "ApprovedById");
        }
    }
}
