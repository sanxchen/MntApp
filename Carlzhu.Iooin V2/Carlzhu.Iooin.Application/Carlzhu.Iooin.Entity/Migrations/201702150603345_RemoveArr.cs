namespace Carlzhu.Iooin.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveArr : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Apparatus", "UseEmp", "dbo.BaseEmployee");
            DropIndex("dbo.Apparatus", new[] { "UseEmp" });
            AddColumn("dbo.Apparatus", "UseEmpId", c => c.String());
            AlterColumn("dbo.Apparatus", "UseEmp", c => c.String());
            AlterColumn("dbo.Apparatus", "LastUpdateDate", c => c.DateTime());
            AlterColumn("dbo.Apparatus", "LastUpdateEmp", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Apparatus", "LastUpdateEmp", c => c.String(nullable: false));
            AlterColumn("dbo.Apparatus", "LastUpdateDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Apparatus", "UseEmp", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Apparatus", "UseEmpId");
            CreateIndex("dbo.Apparatus", "UseEmp");
            AddForeignKey("dbo.Apparatus", "UseEmp", "dbo.BaseEmployee", "EmpNo", cascadeDelete: true);
        }
    }
}
