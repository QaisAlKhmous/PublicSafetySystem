namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ItemLogs_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemLogs",
                c => new
                    {
                        ItemLogId = c.Guid(nullable: false),
                        EmployeeId = c.Guid(),
                        ItemId = c.Guid(nullable: false),
                        MatrixItemId = c.Guid(),
                        IssuanceId = c.Guid(),
                        ActionType = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        EntitlementYear = c.Int(),
                        Notes = c.String(),
                        CreatedById = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ItemLogId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Issuances", t => t.IssuanceId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.MatrixItems", t => t.MatrixItemId)
                .Index(t => t.EmployeeId)
                .Index(t => t.ItemId)
                .Index(t => t.MatrixItemId)
                .Index(t => t.IssuanceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemLogs", "MatrixItemId", "dbo.MatrixItems");
            DropForeignKey("dbo.ItemLogs", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemLogs", "IssuanceId", "dbo.Issuances");
            DropForeignKey("dbo.ItemLogs", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.ItemLogs", new[] { "IssuanceId" });
            DropIndex("dbo.ItemLogs", new[] { "MatrixItemId" });
            DropIndex("dbo.ItemLogs", new[] { "ItemId" });
            DropIndex("dbo.ItemLogs", new[] { "EmployeeId" });
            DropTable("dbo.ItemLogs");
        }
    }
}
