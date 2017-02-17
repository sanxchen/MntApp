namespace Carlzhu.Iooin.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployeeManager : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BaseEmployee", "ManagerId", c => c.String());
            AddColumn("dbo.BaseEmployee", "Manager", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BaseEmployee", "Manager");
            DropColumn("dbo.BaseEmployee", "ManagerId");
        }
    }
}
