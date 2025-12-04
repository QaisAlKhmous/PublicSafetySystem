namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_matrixItems_table : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Matrices", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.Matrices", "ItemId", "dbo.Items");
            DropIndex("dbo.Matrices", new[] { "ItemId" });
            DropIndex("dbo.Matrices", new[] { "CreatedById" });
            CreateTable(
                "dbo.MatrixItems",
                c => new
                    {
                        MatrixItemId = c.Guid(nullable: false),
                        MatrixId = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Frequency = c.Int(nullable: false),
                        CreatedById = c.Guid(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MatrixItemId)
                .ForeignKey("dbo.Users", t => t.CreatedById)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Matrices", t => t.MatrixId)
                .Index(t => t.MatrixId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatedById);
            
            AddColumn("dbo.Matrices", "IsActive", c => c.Boolean(nullable: false));
            DropColumn("dbo.Matrices", "ItemId");
            DropColumn("dbo.Matrices", "Quantity");
            DropColumn("dbo.Matrices", "Frequency");
            DropColumn("dbo.Matrices", "EffectiveFrom");
            DropColumn("dbo.Matrices", "EffectiveTo");
            DropColumn("dbo.Matrices", "CreatedById");
            DropColumn("dbo.Matrices", "CreatedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Matrices", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Matrices", "CreatedById", c => c.Guid(nullable: false));
            AddColumn("dbo.Matrices", "EffectiveTo", c => c.DateTime());
            AddColumn("dbo.Matrices", "EffectiveFrom", c => c.DateTime());
            AddColumn("dbo.Matrices", "Frequency", c => c.Int(nullable: false));
            AddColumn("dbo.Matrices", "Quantity", c => c.Int(nullable: false));
            AddColumn("dbo.Matrices", "ItemId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.MatrixItems", "MatrixId", "dbo.Matrices");
            DropForeignKey("dbo.MatrixItems", "ItemId", "dbo.Items");
            DropForeignKey("dbo.MatrixItems", "CreatedById", "dbo.Users");
            DropIndex("dbo.MatrixItems", new[] { "CreatedById" });
            DropIndex("dbo.MatrixItems", new[] { "ItemId" });
            DropIndex("dbo.MatrixItems", new[] { "MatrixId" });
            DropColumn("dbo.Matrices", "IsActive");
            DropTable("dbo.MatrixItems");
            CreateIndex("dbo.Matrices", "CreatedById");
            CreateIndex("dbo.Matrices", "ItemId");
            AddForeignKey("dbo.Matrices", "ItemId", "dbo.Items", "ItemId");
            AddForeignKey("dbo.Matrices", "CreatedById", "dbo.Users", "UserId");
        }
    }
}
