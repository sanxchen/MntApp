using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Runtime.Remoting.Messaging;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.Entity.FORM.f.draw;
using Carlzhu.Iooin.Entity.FORM.f.report;
using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Entity.mes;
using Carlzhu.Iooin.Entity.MANAGER;
using Carlzhu.Iooin.Entity.QUALITY;
using Carlzhu.Iooin.Entity.TPA;

namespace Carlzhu.Iooin.Entity.BaseUtility
{
    public class CarlzhuContext : DbContext
    {


        //public CarlzhuContext()
           
        //{
        //    //是否启用延迟加载:  
        //    //  true:   延迟加载（Lazy Loading）：获取实体时不会加载其导航属性，一旦用到导航属性就会自动加载  
        //    //  false:  直接加载（Eager loading）：通过 Include 之类的方法显示加载导航属性，获取实体时会即时加载通过 Include 指定的导航属性  
        //    this.Configuration.LazyLoadingEnabled = false;

        //    //this.Configuration.AutoDetectChangesEnabled = true;  //自动监测变化，默认值为 true  
        //}




        public static CarlzhuContext CzContext
        {
            get
            {
                var dbContext = CallContext.GetData("DbContext") as CarlzhuContext;

                if (dbContext != null) return dbContext;
                dbContext = new CarlzhuContext(); //如果不存在上下文的话，创建一个EF上下文

                //我们在创建一个，放到数据槽中去
                CallContext.SetData("DbContext", dbContext);

                return dbContext;
            }

        }



        #region RKW
        public virtual DbSet<MesRkwPo> MesRkwPos { get; set; }
        public virtual DbSet<MesRkwSn> MesRkwSns { get; set; }
        public virtual DbSet<MesRkwBox> MesRkwBoxs { get; set; }
        public virtual DbSet<MesRkwPackage> MesRkwPackages { get; set; }

        #endregion


        //EmailLog
        public virtual DbSet<EmailLog> EmailLogs { get; set; }

        //ERP

        public virtual DbSet<TpaCustomer> TpaCustomers { get; set; }
        public virtual DbSet<TpaSupplier> TpaSuppliers { get; set; }

        //HRMS
        public virtual DbSet<FormClass> FormClasses { get; set; }





        public virtual DbSet<HrmsAttendance> HrmsAttendances { get; set; }
        public virtual DbSet<HrmsCalendarEvents> HrmsCalendarEventses { get; set; }
        public virtual DbSet<HrmsShift> HrmsShifts { get; set; }
        public virtual DbSet<HrmsShiftSnaps> HrmsShiftSnapses { get; set; }
        public virtual DbSet<HrmsAttAnalysis> HrmsAttAnalyses { get; set; }

        public virtual DbSet<FormAbnormalAttendance> FormAbnormalAttendances { get; set; }

        


        public virtual DbSet<DoorInout> DoorInouts { get; set; }
        public virtual DbSet<HrmsCar> HrmsCars { get; set; }
        public virtual DbSet<HrmsCarInOut> HrmsCarInOuts { get; set; }






        //MANAGER

        public virtual DbSet<SystemModel> SystemModels { get; set; }
        public virtual DbSet<SystemController> SystemControllers { get; set; }
        public virtual DbSet<SystemAction> SystemActions { get; set; }
        
        public virtual DbSet<SystemPower> SystemPowers { get; set; }
        public virtual DbSet<SystemRecover> SystemRecovers { get; set; }
        
        //FORM
        public virtual DbSet<FormType> FormTypes { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormSign> FormSigns { get; set; }
        public virtual DbSet<FormProxy> FormProxies { get; set; }

        public virtual DbSet<FormPath> FormPaths { get; set; }

        public virtual DbSet<Files> Fileses { get; set; }
        public virtual DbSet<FileGroup> FileGroups { get; set; }
        public virtual DbSet<FilesFileGroup> FilesFileGroups { get; set; }

        public virtual DbSet<FormInvolvingUser> FormInvolvingUsers { get; set; }

        //f


        //test
        public virtual DbSet<FormTest> FormTests { get; set; }


        //draw
        public virtual DbSet<FormDrawingsBom> FormDrawingsBoms { get; set; }
        public virtual DbSet<FormDrawingsCustomer> FormDrawingsCustomers { get; set; }
        public virtual DbSet<FormDrawingsExternal> FormDrawingsExternals { get; set; }
        public virtual DbSet<FormDrawingsFqc> FormDrawingsFqcs { get; set; }
        public virtual DbSet<FormDrawingsProgram> FormDrawingsPrograms { get; set; }
        public virtual DbSet<FormDrawingsInside> FormDrawingsInsides { get; set; }
        public virtual DbSet<FormDrawingsProfile> FormDrawingsProfiles { get; set; }
        public virtual DbSet<FormDrawingsPackage> FormDrawingsPackages { get; set; }
        public virtual DbSet<FormDrawingsProcess> FormDrawingsProcesses { get; set; }
        public virtual DbSet<FormDrawingsSop> FormDrawingsSops { get; set; }
        public virtual DbSet<FormDrawingsEcn> FormDrawingsEcns { get; set; }

        public virtual DbSet<FormDrawingsControlPlan> FormDrawingsControlPlans { get; set; }
        public virtual DbSet<FormDrawingsFmea> FormDrawingsFmeas { get; set; }
        public virtual DbSet<FormReplaceDrawings> FormReplaceDrawingses { get; set; }

        public virtual DbSet<FormItEquipment> FormItEquipments { get; set; }
        public virtual DbSet<FormEquipmentSop> FormEquipmentSops { get; set; }

        public virtual DbSet<FormDrawingsSupplierSampleAck> FormDrawingsSupplierSampleAcks { get; set; }


        public virtual DbSet<FormDrawingsSopDewell> FormDrawingsSopDewells { get; set; }



        public virtual DbSet<FormWorkshopInspection> FormWorkshopInspections { get; set; }
        public virtual DbSet<FormJumpTheQueue> FormJumpTheQueues { get; set; }


        //quality
        public virtual DbSet<Published> Publisheds { get; set; }

        public virtual DbSet<Apparatus> Apparatuses { get; set; }

        public virtual DbSet<FileReplaceRecords> FileReplaceRecordses { get; set; }


        public virtual DbSet<FormDrawingsEngineeringChange> FormDrawingsEngineeringChanges { get; set; }



        //
        //  public virtual DbSet<FormPdAbnormal> FormPdAbnormals { get; set; }

        public virtual DbSet<FormPdAbnor> FormPdAbnors { get; set; }



        #region DewellReport

        public virtual DbSet<FormDewellReportAssembling> FormDewellReportAssemblings { get; set; }
        public virtual DbSet<FormDewellReportBending> FormDewellReportBendings { get; set; }
        public virtual DbSet<FormDewellReportFitter> FormDewellReportFitters { get; set; }
        public virtual DbSet<FormDewellReportFullInspectionPackage> FormDewellReportFullInspectionPackages { get; set; }
        public virtual DbSet<FormDewellReportLaser> FormDewellReportLasers { get; set; }
        public virtual DbSet<FormDewellReportNumberPunching> FormDewellReportNumberPunchings { get; set; }
        public virtual DbSet<FormDewellReportWelding> FormDewellReportWeldings { get; set; }

        #endregion



        public virtual DbSet<FormToGoout> FormToGoouts { get; set; }

        public virtual DbSet<FormCar> FormCars { get; set; }
        public virtual DbSet<FormPurchase> FormPurchases { get; set; }
        public virtual DbSet<FormStamps> FormStampses { get; set; }

        public virtual DbSet<FormBll> FormBlls { get; set; }

        public virtual DbSet<FormPassEntry> FormPassEntries { get; set; }



        public virtual DbSet<FormWorkLeave> FormWorkLeaves { get; set; }

        public virtual DbSet<FormTransactionDepartment> FormTransactionDepartments { get; set; }

        public virtual DbSet<FormNcr> FormNcrs { get; set; }
        //public virtual DbSet<Model.FORM.f.FormNcrReview> FormNcrReviews { get; set; }

        public virtual DbSet<FormNcrPart> FormNcrParts { get; set; }



        #region sec


        public DbSet<TaskSch> Tasks { get; set; }

        #endregion


        //public virtual DbSet<TpaGoodType> TpaGoodTypes { get; set; }
        //public virtual DbSet<TpaGoodWarehouse> TpaGoodWarehouses { get; set; }
        public virtual DbSet<TpaGoods> TpaGoodses { get; set; }
        //public virtual DbSet<TpaHouseType> TpaHouseTypes { get; set; }

        //public virtual DbSet<TpaUse> TpaUses { get; set; }

        #region BaseEntity

        public DbSet<BaseBackupJob> BaseBackupJobs { get; set; }
        public DbSet<BaseButton> BaseButtons { get; set; }
        public DbSet<BaseButtonPermission> BaseButtonPermissions { get; set; }

        public DbSet<BaseCompany> BaseCompanies { get; set; }
        public DbSet<BaseDataDictionary> BaseDataDictionaries { get; set; }
        public DbSet<BaseDataDictionaryDetail> BaseDataDictionaryDetails { get; set; }
        public DbSet<BaseDataScopePermission> BaseDataScopePermissions { get; set; }
        public DbSet<BaseDepartment> BaseDepartments { get; set; }
      
        public DbSet<BaseEmployee> BaseEmployees { get; set; }
        public DbSet<BaseExcelImport> BaseExcelImports { get; set; }
        public DbSet<BaseExcelImportDetail> BaseExcelImportDetails { get; set; }
        public DbSet<BaseFormAttribute> BaseFormAttributes { get; set; }
        public DbSet<BaseFormAttributeValue> BaseFormAttributeValues { get; set; }
        public DbSet<BaseGroupUser> BaseGroupUsers { get; set; }
        public DbSet<BaseInterfaceManage> BaseInterfaceManages { get; set; }
        public DbSet<BaseInterfaceManageParameter> BaseInterfaceManageParameters { get; set; }
        //Language
        public DbSet<BaseModule> BaseModules { get; set; }
        public DbSet<BaseModulePermission> BaseModulePermissions { get; set; }
       
        public DbSet<BaseObjectUserRelation> BaseObjectUserRelations { get; set; }

    
        public DbSet<BasePost> BasePosts { get; set; }
        public DbSet<BaseProvinceCity> BaseProvinceCities { get; set; }
        public DbSet<BaseQueryRecord> BaseQueryRecords { get; set; }
        public DbSet<BaseRoles> BaseRoleses { get; set; }
        public DbSet<BaseShortcuts> BaseShortcutses { get; set; }

        public DbSet<BaseSysLog> BaseSysLogs { get; set; }
        public DbSet<BaseSysLogDetail> BaseSysLogDetails { get; set; }
        public DbSet<BaseUser> BaseUsers { get; set; }
        public DbSet<BaseView> BaseViews { get; set; }
        public DbSet<BaseViewPermission> BaseViewPermissions { get; set; }
        public DbSet<BaseViewWhere> BaseViewWheres { get; set; }


        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Entity<Files>().HasMany(a => a.FileGroups).WithMany(t => t.Fileses).Map(m =>
            //{
            //    m.ToTable("FilesFileGroup");
            //    m.MapLeftKey("Md5");//对应Trip的主键
            //    m.MapRightKey("GroupGuid");
            //});

            base.OnModelCreating(modelBuilder);
        }


    }
}
