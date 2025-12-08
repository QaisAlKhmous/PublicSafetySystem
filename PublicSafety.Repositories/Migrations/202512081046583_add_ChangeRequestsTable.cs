namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ChangeRequestsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChangeRequests",
                c => new
                    {
                        RequestId = c.Guid(nullable: false),
                        EntityType = c.Int(nullable: false),
                        EntityId = c.Guid(nullable: false),
                        OldValue = c.String(),
                        NewValue = c.String(),
                        ChangedById = c.Guid(nullable: false),
                        RequestDate = c.DateTime(nullable: false),
                        ApprovedDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        AdminComment = c.String(),
                    })
                .PrimaryKey(t => t.RequestId)
                .ForeignKey("dbo.Users", t => t.ChangedById)
                .Index(t => t.ChangedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChangeRequests", "ChangedById", "dbo.Users");
            DropIndex("dbo.ChangeRequests", new[] { "ChangedById" });
            DropTable("dbo.ChangeRequests");
        }
    }
}
