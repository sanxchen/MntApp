namespace Carlzhu.Iooin.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveArr3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Apparatus", "CalDate", c => c.String());
            AlterColumn("dbo.Apparatus", "CalCircle", c => c.String());
            AlterColumn("dbo.Apparatus", "CalResult", c => c.String());
            DropColumn("dbo.Apparatus", "FirmNo");
            DropColumn("dbo.Apparatus", "Firm");
            DropColumn("dbo.Apparatus", "Precision");
            DropColumn("dbo.Apparatus", "Range");
            DropColumn("dbo.Apparatus", "BuyDate");
            DropColumn("dbo.Apparatus", "Status");
            DropColumn("dbo.Apparatus", "LastUpdateDate");
            DropColumn("dbo.Apparatus", "LastUpdateEmp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Apparatus", "LastUpdateEmp", c => c.String());
            AddColumn("dbo.Apparatus", "LastUpdateDate", c => c.DateTime());
            AddColumn("dbo.Apparatus", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Apparatus", "BuyDate", c => c.DateTime());
            AddColumn("dbo.Apparatus", "Range", c => c.String());
            AddColumn("dbo.Apparatus", "Precision", c => c.String());
            AddColumn("dbo.Apparatus", "Firm", c => c.String());
            AddColumn("dbo.Apparatus", "FirmNo", c => c.String());
            AlterColumn("dbo.Apparatus", "CalResult", c => c.Int(nullable: false));
            AlterColumn("dbo.Apparatus", "CalCircle", c => c.Int(nullable: false));
            AlterColumn("dbo.Apparatus", "CalDate", c => c.DateTime());
        }
    }
}
