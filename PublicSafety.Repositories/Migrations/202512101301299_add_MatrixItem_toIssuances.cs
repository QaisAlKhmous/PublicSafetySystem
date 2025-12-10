namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_MatrixItem_toIssuances : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Issuances", "MatrixItemId", c => c.Guid());
            CreateIndex("dbo.Issuances", "MatrixItemId");
            AddForeignKey("dbo.Issuances", "MatrixItemId", "dbo.MatrixItems", "MatrixItemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Issuances", "MatrixItemId", "dbo.MatrixItems");
            DropIndex("dbo.Issuances", new[] { "MatrixItemId" });
            DropColumn("dbo.Issuances", "MatrixItemId");
        }
    }
}
