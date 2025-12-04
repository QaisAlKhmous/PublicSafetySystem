namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeId = c.Guid(nullable: false),
                        Name = c.String(),
                        Active = c.Boolean(nullable: false),
                        JobTitleId = c.Guid(nullable: false),
                        Notes = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeId)
                .ForeignKey("dbo.JobTitles", t => t.JobTitleId)
                .Index(t => t.JobTitleId);
            
            CreateTable(
                "dbo.Issuances",
                c => new
                    {
                        IssuanceId = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        IssuanceDate = c.DateTime(nullable: false),
                        ExceptionFormPath = c.String(),
                        SignedReceiptPath = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedById = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.IssuanceId)
                .ForeignKey("dbo.Users", t => t.CreatedById)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .Index(t => t.EmployeeId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedById);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        Username = c.String(),
                        PasswordHash = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Disposals",
                c => new
                    {
                        DisposalId = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        DisposalDate = c.DateTime(nullable: false),
                        DisposalFormPath = c.String(),
                        CreatedById = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ApprovedById = c.Guid(),
                    })
                .PrimaryKey(t => t.DisposalId)
                .ForeignKey("dbo.Users", t => t.ApprovedById)
                .ForeignKey("dbo.Users", t => t.CreatedById)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedById)
                .Index(t => t.ApprovedById);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ItemId = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ItemId);
            
            CreateTable(
                "dbo.Matrices",
                c => new
                    {
                        MatrixId = c.Guid(nullable: false),
                        CategoryId = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Frequency = c.Int(nullable: false),
                        EffectiveFrom = c.DateTime(nullable: false),
                        EffectiveTo = c.DateTime(nullable: false),
                        Version = c.Int(nullable: false),
                        CreatedById = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MatrixId)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.Users", t => t.CreatedById)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .Index(t => t.CategoryId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedById);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.JobTitleCategories",
                c => new
                    {
                        JobTitleCategoryId = c.Guid(nullable: false),
                        CategoryId = c.Guid(nullable: false),
                        JobTitleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.JobTitleCategoryId)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.JobTitles", t => t.JobTitleId)
                .Index(t => t.CategoryId)
                .Index(t => t.JobTitleId);
            
            CreateTable(
                "dbo.JobTitles",
                c => new
                    {
                        JobTitleId = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.JobTitleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employees", "JobTitleId", "dbo.JobTitles");
            DropForeignKey("dbo.Issuances", "ItemId", "dbo.Items");
            DropForeignKey("dbo.Issuances", "EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Issuances", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.Disposals", "ItemId", "dbo.Items");
            DropForeignKey("dbo.Matrices", "ItemId", "dbo.Items");
            DropForeignKey("dbo.Matrices", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.Matrices", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.JobTitleCategories", "JobTitleId", "dbo.JobTitles");
            DropForeignKey("dbo.JobTitleCategories", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Disposals", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.Disposals", "ApprovedById", "dbo.Users");
            DropIndex("dbo.JobTitleCategories", new[] { "JobTitleId" });
            DropIndex("dbo.JobTitleCategories", new[] { "CategoryId" });
            DropIndex("dbo.Matrices", new[] { "CreatedById" });
            DropIndex("dbo.Matrices", new[] { "ItemId" });
            DropIndex("dbo.Matrices", new[] { "CategoryId" });
            DropIndex("dbo.Disposals", new[] { "ApprovedById" });
            DropIndex("dbo.Disposals", new[] { "CreatedById" });
            DropIndex("dbo.Disposals", new[] { "ItemId" });
            DropIndex("dbo.Issuances", new[] { "CreatedById" });
            DropIndex("dbo.Issuances", new[] { "ItemId" });
            DropIndex("dbo.Issuances", new[] { "EmployeeId" });
            DropIndex("dbo.Employees", new[] { "JobTitleId" });
            DropTable("dbo.JobTitles");
            DropTable("dbo.JobTitleCategories");
            DropTable("dbo.Categories");
            DropTable("dbo.Matrices");
            DropTable("dbo.Items");
            DropTable("dbo.Disposals");
            DropTable("dbo.Users");
            DropTable("dbo.Issuances");
            DropTable("dbo.Employees");
        }
    }
}
