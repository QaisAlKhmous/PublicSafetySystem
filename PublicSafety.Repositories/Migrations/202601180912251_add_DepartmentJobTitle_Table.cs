namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_DepartmentJobTitle_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DepartmentJobTitles",
                c => new
                    {
                        DepartmentId = c.Guid(nullable: false),
                        JobTitleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.DepartmentId, t.JobTitleId })
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .ForeignKey("dbo.JobTitles", t => t.JobTitleId)
                .Index(t => t.DepartmentId)
                .Index(t => t.JobTitleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DepartmentJobTitles", "JobTitleId", "dbo.JobTitles");
            DropForeignKey("dbo.DepartmentJobTitles", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.DepartmentJobTitles", new[] { "JobTitleId" });
            DropIndex("dbo.DepartmentJobTitles", new[] { "DepartmentId" });
            DropTable("dbo.DepartmentJobTitles");
        }
    }
}
