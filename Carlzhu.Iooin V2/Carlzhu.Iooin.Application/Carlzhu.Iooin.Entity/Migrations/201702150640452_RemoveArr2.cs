namespace Carlzhu.Iooin.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveArr2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Apparatus", "UseAdd", c => c.String());
            AddColumn("dbo.Apparatus", "NextCalDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Apparatus", "CertificateResults", c => c.String());
            AlterColumn("dbo.Apparatus", "Name", c => c.String());
            AlterColumn("dbo.Apparatus", "FirmNo", c => c.String());
            AlterColumn("dbo.Apparatus", "Firm", c => c.String());
            AlterColumn("dbo.Apparatus", "Model", c => c.String());
            AlterColumn("dbo.Apparatus", "Precision", c => c.String());
            AlterColumn("dbo.Apparatus", "Range", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Apparatus", "Range", c => c.String(nullable: false));
            AlterColumn("dbo.Apparatus", "Precision", c => c.String(nullable: false));
            AlterColumn("dbo.Apparatus", "Model", c => c.String(nullable: false));
            AlterColumn("dbo.Apparatus", "Firm", c => c.String(nullable: false));
            AlterColumn("dbo.Apparatus", "FirmNo", c => c.String(nullable: false));
            AlterColumn("dbo.Apparatus", "Name", c => c.String(nullable: false));
            DropColumn("dbo.Apparatus", "CertificateResults");
            DropColumn("dbo.Apparatus", "NextCalDate");
            DropColumn("dbo.Apparatus", "UseAdd");
        }
    }
}
