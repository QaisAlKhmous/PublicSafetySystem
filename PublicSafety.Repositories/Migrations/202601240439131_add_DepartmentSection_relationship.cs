namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_DepartmentSection_relationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sections", "DepartmentId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Sections", "DepartmentId");
            AddForeignKey("dbo.Sections", "DepartmentId", "dbo.Departments", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sections", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.Sections", new[] { "DepartmentId" });
            DropColumn("dbo.Sections", "DepartmentId");
        }
    }
}
