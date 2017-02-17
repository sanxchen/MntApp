namespace Carlzhu.Iooin.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTpa : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TpaGoods", "Emp", "dbo.BaseEmployee");
            DropForeignKey("dbo.TpaGoods", "Type", "dbo.TpaGoodType");
            DropForeignKey("dbo.TpaGoodWarehouse", "Good", "dbo.TpaGoods");
            DropForeignKey("dbo.TpaGoodWarehouse", "WareHouse", "dbo.TpaHouseType");
            DropForeignKey("dbo.TpaUse", "Emp", "dbo.BaseEmployee");
            DropForeignKey("dbo.TpaUse", "Good", "dbo.TpaGoods");
            DropForeignKey("dbo.TpaGoods", "SupplierCode", "dbo.TpaSupplier");
            DropIndex("dbo.TpaGoods", new[] { "Type" });
            DropIndex("dbo.TpaGoods", new[] { "SupplierCode" });
            DropIndex("dbo.TpaGoods", new[] { "Emp" });
            DropIndex("dbo.TpaGoodWarehouse", new[] { "Good" });
            DropIndex("dbo.TpaGoodWarehouse", new[] { "WareHouse" });
            DropIndex("dbo.TpaUse", new[] { "Good" });
            DropIndex("dbo.TpaUse", new[] { "Emp" });
            RenameColumn(table: "dbo.TpaGoods", name: "SupplierCode", newName: "TpaSupplier_Code");
            CreateTable(
                "dbo.FormTransactionDepartment",
                c => new
                    {
                        RowId = c.Int(nullable: false, identity: true),
                        TransactionType = c.Int(nullable: false),
                        TransactionEmp = c.String(),
                        CurrentDepatment = c.String(),
                        EnableDateTime = c.DateTime(nullable: false),
                        BeforedDepart = c.String(),
                        BeforePosition = c.String(),
                        AfterDepart = c.String(),
                        AfterPosition = c.String(),
                        BeforeDepartmentMark = c.String(),
                        AfterDepartmentMark = c.String(),
                        HrDepartmentMark = c.String(),
                        ManagerDepartmentMark = c.String(),
                        FormNo = c.String(maxLength: 128),
                        Mark = c.String(),
                    })
                .PrimaryKey(t => t.RowId)
                .ForeignKey("dbo.Form", t => t.FormNo)
                .Index(t => t.FormNo);
            
            AlterColumn("dbo.TpaGoods", "TpaSupplier_Code", c => c.String(maxLength: 128));
            CreateIndex("dbo.TpaGoods", "TpaSupplier_Code");
            AddForeignKey("dbo.TpaGoods", "TpaSupplier_Code", "dbo.TpaSupplier", "Code");
            DropColumn("dbo.TpaGoods", "Name");
            DropColumn("dbo.TpaGoods", "Format");
            DropColumn("dbo.TpaGoods", "Describe");
            DropColumn("dbo.TpaGoods", "Type");
            DropColumn("dbo.TpaGoods", "Unit");
            DropColumn("dbo.TpaGoods", "Price");
            DropColumn("dbo.TpaGoods", "Warning");
            DropColumn("dbo.TpaGoods", "Emp");
            DropColumn("dbo.TpaGoods", "CreateTime");
            DropTable("dbo.TpaGoodType");
            DropTable("dbo.TpaGoodWarehouse");
            DropTable("dbo.TpaHouseType");
            DropTable("dbo.TpaUse");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TpaUse",
                c => new
                    {
                        RowId = c.Int(nullable: false, identity: true),
                        Good = c.String(nullable: false, maxLength: 128),
                        Emp = c.String(nullable: false, maxLength: 128),
                        Location = c.Int(nullable: false),
                        AssetsNo = c.String(),
                        Qty = c.Int(nullable: false),
                        IsReturn = c.Boolean(nullable: false),
                        BorrowDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(),
                        Mark = c.String(),
                    })
                .PrimaryKey(t => t.RowId);
            
            CreateTable(
                "dbo.TpaHouseType",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Describe = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.TpaGoodWarehouse",
                c => new
                    {
                        RowId = c.Int(nullable: false, identity: true),
                        Good = c.String(nullable: false, maxLength: 128),
                        Qty = c.Int(nullable: false),
                        WareHouse = c.String(nullable: false, maxLength: 128),
                        CreateTime = c.DateTime(nullable: false),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RowId);
            
            CreateTable(
                "dbo.TpaGoodType",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Describe = c.String(nullable: false),
                        IsImportant = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Code);
            
            AddColumn("dbo.TpaGoods", "CreateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.TpaGoods", "Emp", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.TpaGoods", "Warning", c => c.Int(nullable: false));
            AddColumn("dbo.TpaGoods", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TpaGoods", "Unit", c => c.String(nullable: false));
            AddColumn("dbo.TpaGoods", "Type", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.TpaGoods", "Describe", c => c.String());
            AddColumn("dbo.TpaGoods", "Format", c => c.String(nullable: false));
            AddColumn("dbo.TpaGoods", "Name", c => c.String(nullable: false));
            DropForeignKey("dbo.TpaGoods", "TpaSupplier_Code", "dbo.TpaSupplier");
            DropForeignKey("dbo.FormTransactionDepartment", "FormNo", "dbo.Form");
            DropIndex("dbo.FormTransactionDepartment", new[] { "FormNo" });
            DropIndex("dbo.TpaGoods", new[] { "TpaSupplier_Code" });
            AlterColumn("dbo.TpaGoods", "TpaSupplier_Code", c => c.String(nullable: false, maxLength: 128));
            DropTable("dbo.FormTransactionDepartment");
            RenameColumn(table: "dbo.TpaGoods", name: "TpaSupplier_Code", newName: "SupplierCode");
            CreateIndex("dbo.TpaUse", "Emp");
            CreateIndex("dbo.TpaUse", "Good");
            CreateIndex("dbo.TpaGoodWarehouse", "WareHouse");
            CreateIndex("dbo.TpaGoodWarehouse", "Good");
            CreateIndex("dbo.TpaGoods", "Emp");
            CreateIndex("dbo.TpaGoods", "SupplierCode");
            CreateIndex("dbo.TpaGoods", "Type");
            AddForeignKey("dbo.TpaGoods", "SupplierCode", "dbo.TpaSupplier", "Code", cascadeDelete: true);
            AddForeignKey("dbo.TpaUse", "Good", "dbo.TpaGoods", "Code", cascadeDelete: true);
            AddForeignKey("dbo.TpaUse", "Emp", "dbo.BaseEmployee", "EmpNo", cascadeDelete: true);
            AddForeignKey("dbo.TpaGoodWarehouse", "WareHouse", "dbo.TpaHouseType", "Code", cascadeDelete: true);
            AddForeignKey("dbo.TpaGoodWarehouse", "Good", "dbo.TpaGoods", "Code", cascadeDelete: true);
            AddForeignKey("dbo.TpaGoods", "Type", "dbo.TpaGoodType", "Code", cascadeDelete: true);
            AddForeignKey("dbo.TpaGoods", "Emp", "dbo.BaseEmployee", "EmpNo", cascadeDelete: true);
        }
    }
}
