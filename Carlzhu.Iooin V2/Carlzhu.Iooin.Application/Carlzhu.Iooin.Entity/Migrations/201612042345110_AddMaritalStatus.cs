namespace Carlzhu.Iooin.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMaritalStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BaseEmployee", "MaritalStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BaseEmployee", "MaritalStatus");
        }
    }
}
