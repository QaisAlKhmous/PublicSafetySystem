namespace PublicSafety.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modified_employees_name : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "FullName", c => c.String());
            AddColumn("dbo.Employees", "FirstName", c => c.String());
            AddColumn("dbo.Employees", "SecondName", c => c.String());
            AddColumn("dbo.Employees", "LastName", c => c.String());
            DropColumn("dbo.Employees", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "Name", c => c.String());
            DropColumn("dbo.Employees", "LastName");
            DropColumn("dbo.Employees", "SecondName");
            DropColumn("dbo.Employees", "FirstName");
            DropColumn("dbo.Employees", "FullName");
        }
    }
}
