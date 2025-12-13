namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_EmployeeJobTitleHistories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeJobTitleHistories",
                c => new
                    {
                        EmployeeJobTitleHistoryId = c.Guid(nullable: false),
                        EmployeeId = c.Guid(nullable: false),
                        JobTitleId = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeJobTitleHistoryId)
                .ForeignKey("dbo.Employees", t => t.EmployeeId)
                .ForeignKey("dbo.JobTitles", t => t.JobTitleId)
                .Index(t => t.EmployeeId)
                .Index(t => t.JobTitleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeJobTitleHistories", "JobTitleId", "dbo.JobTitles");
            DropForeignKey("dbo.EmployeeJobTitleHistories", "EmployeeId", "dbo.Employees");
            DropIndex("dbo.EmployeeJobTitleHistories", new[] { "JobTitleId" });
            DropIndex("dbo.EmployeeJobTitleHistories", new[] { "EmployeeId" });
            DropTable("dbo.EmployeeJobTitleHistories");
        }
    }
}
