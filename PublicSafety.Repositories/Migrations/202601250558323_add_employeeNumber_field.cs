namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_employeeNumber_field : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "EmployeeNumber", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "EmployeeNumber");
        }
    }
}
