using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Carlzhu.Iooin.Business;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Offices;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Carlzhu.Iooin.WebApp.Areas.eSystem.Controllers
{
    public class ProductController : Controller
    {
        // GET: eSystem/Product



        [LoginAuthorize]
        public ActionResult Index()
        {

            ViewData["emp"] = ManageProvider.Provider.Current().UserId;

            var model = new BaseServices<FormPdAbnor>().LoadEntities(
                  c => !c.IsClose).ToList().Where(k => k.Form.FormStatus == (int)Form.StatusEnum.签核完成);
            return View(model);
        }

        public ActionResult EditProAbnormal(int id)
        {
            var model = new BaseServices<FormPdAbnor>().LoadEntities(c => c.RowId == id).First();
            return View(model);
        }


        /// <summary>
        /// 生产异常处理
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dealingMethod"></param>
        /// <param name="planFinishTime"></param>
        /// <returns></returns>
        public ActionResult DealingAbnormal(int id, string dealingMethod, DateTime planFinishTime)
        {
            var model =
                new BaseServices<FormPdAbnor>().LoadEntities(c => c.RowId == id).First();

            model.DealingMethod = dealingMethod;
            model.PlanFinishTime = planFinishTime;
            model.ResponsibilityPeople = ManageProvider.Provider.Current().UserId;
            //处理表单
  

            return
                Content(new BaseServices<FormPdAbnor>().UpdateEntity(model) ? "success" : "error");

        }

        public ActionResult CaseClose(int caseid)
        {
            var model =
            new BaseServices<FormPdAbnor>().LoadEntities(c => c.RowId == caseid).First();

            model.FinishTime = DateTime.Now;
            model.IsClose = true;

            //结案表单
            
            return
                Content(new BaseServices<FormPdAbnor>().UpdateEntity(model) ? "success" : "error");
        }





        public void ErpWip()
        {
            IWorkbook wb = new HSSFWorkbook();

            Dictionary<string, ICellStyle> cellStyles = new Dictionary<string, ICellStyle>
            {
                {"默认", ExcepNpoi.Getcellstyle(wb, ExcepNpoi.Stylexls.默认)},
                {"数字", ExcepNpoi.Getcellstyle(wb, ExcepNpoi.Stylexls.数字)}
            };

            WriteSheet("WIP",
                DataFactory.Database("CysoftContext").FindTableBySql(System.IO.File.ReadAllText(Server.MapPath("/Resource/Sql/erpwip.txt"))),
                wb,
                new List<CellSetting>
                    {
                        new CellSetting(){Name = "品名",CellStyle = cellStyles["默认"],CellWidth = 15*256*2,},
                        new CellSetting(){Name = "品号",CellStyle = cellStyles["默认"],CellWidth = 25*256*2,},
                        new CellSetting(){Name = "规格",CellStyle = cellStyles["默认"],CellWidth = 35*256*2,},
                        new CellSetting(){Name = "总WIP",CellStyle = cellStyles["数字"],CellWidth = 15 * 256,},
                        new CellSetting(){Name = "厂内WIP",CellStyle = cellStyles["数字"],CellWidth = 15 * 256,},
                        new CellSetting(){Name = "厂外WIP",CellStyle = cellStyles["数字"],CellWidth = 15 * 256,},
                        new CellSetting(){Name = "待入库良品",CellStyle = cellStyles["数字"],CellWidth = 15 * 256,},
                        new CellSetting(){Name = "待入库不良品",CellStyle = cellStyles["数字"],CellWidth = 15 * 256,},
                        new CellSetting(){Name = "库存",CellStyle = cellStyles["数字"],CellWidth = 15 * 256,},
                    },
            cellStyles);


            DataTable dt =
                DataFactory.Database("CysoftContext")
                    .FindTableBySql(System.IO.File.ReadAllText(Server.MapPath("/Resource/Sql/Stock.txt")));

            var cells = new List<CellSetting>
            {
                new CellSetting() {Name = "品号大类", CellStyle = cellStyles["默认"], CellWidth = 15*256,},
                new CellSetting() {Name = "品号大类说明", CellStyle = cellStyles["默认"], CellWidth = 15*256*2,},
                new CellSetting() {Name = "品号", CellStyle = cellStyles["默认"], CellWidth = 15*256*2,},
                new CellSetting() {Name = "品名", CellStyle = cellStyles["默认"], CellWidth = 25*256*2,},
                new CellSetting() {Name = "规格", CellStyle = cellStyles["默认"], CellWidth = 35*256*2,},
                new CellSetting() {Name = "仓库代号", CellStyle = cellStyles["默认"], CellWidth = 15*256,},
                new CellSetting() {Name = "仓库名称", CellStyle = cellStyles["默认"], CellWidth = 15*256,},
                new CellSetting() {Name = "数量一", CellStyle = cellStyles["数字"], CellWidth = 15*256,},
                new CellSetting() {Name = "单位一", CellStyle = cellStyles["默认"], CellWidth = 15*256,},
                new CellSetting() {Name = "数量二", CellStyle = cellStyles["数字"], CellWidth = 15*256,},
                new CellSetting() {Name = "单位二", CellStyle = cellStyles["默认"], CellWidth = 15*256,},
            };

            WriteSheet("仓库全部", dt, wb, cells, cellStyles);

            string[] stocks = new string[] { "成品不良仓", "半成品仓", "成品不良仓(客退）", "呆滞料库" };

            foreach (var stock in stocks)
            {
                if (string.IsNullOrEmpty(stock)) continue;

                var dts = dt.Clone();

                foreach (var dataRow in dt.Select($"仓库名称 LIKE '%{stock}%'"))
                {
                    dts.ImportRow(dataRow);
                }
                WriteSheet(stock, dts, wb, cells, cellStyles);
            }


            WriteSheet("未交订单明细",
                    DataFactory.Database("CysoftContext").FindTableBySql(System.IO.File.ReadAllText(Server.MapPath("/Resource/Sql/Order.txt"))),
                    wb,
                    new List<CellSetting>
                        {
                        new CellSetting(){Name = "客户代号",CellStyle = cellStyles["默认"],CellWidth = 15*256,},
                        new CellSetting(){Name = "客户名称",CellStyle = cellStyles["默认"],CellWidth = 15*256,},
                        new CellSetting(){Name = "订单编号",CellStyle = cellStyles["默认"],CellWidth = 15*256,},
                        new CellSetting(){Name = "品号", CellStyle = cellStyles["默认"], CellWidth = 15*256*2,},
                        new CellSetting(){Name = "品名", CellStyle = cellStyles["默认"], CellWidth = 25*256*2,},
                        new CellSetting(){Name = "规格", CellStyle = cellStyles["默认"], CellWidth = 35*256*2,},
                        new CellSetting(){Name = "业务员代号",CellStyle = cellStyles["默认"],CellWidth = 15*256,},
                        new CellSetting(){Name = "预计交期",CellStyle = cellStyles["默认"],CellWidth = 15*256*2,},
                        new CellSetting(){Name = "订单数量1",CellStyle = cellStyles["数字"],CellWidth = 15*256,},
                        new CellSetting(){Name = "累计出货1",CellStyle = cellStyles["数字"],CellWidth = 15*256,},
                        new CellSetting(){Name = "未交数量1",CellStyle = cellStyles["数字"],CellWidth = 15*256,},
                        new CellSetting(){Name = "订单数量2",CellStyle = cellStyles["数字"],CellWidth = 15*256,},
                        new CellSetting(){Name = "累计出货2",CellStyle = cellStyles["数字"],CellWidth = 15*256,},
                        new CellSetting(){Name = "客户采购单号",CellStyle = cellStyles["默认"],CellWidth = 20*256,},
                        new CellSetting(){Name = "接单日期",CellStyle = cellStyles["默认"],CellWidth = 15*256*2,},
                        },
                cellStyles);

            //下载
            HttpContext.Response.ContentType = "application/x-excel";
            string fileName = HttpUtility.UrlEncode($"ERPSTATUS{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls");
            HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);

            wb.Write(HttpContext.Response.OutputStream);





        }

        private void WriteSheet(string sheetName, DataTable dt, IWorkbook wb, List<CellSetting> cellSettings, Dictionary<string, ICellStyle> cellStyles)
        {
            //创建表
            ISheet sh = wb.CreateSheet(sheetName);
            for (int j = 0; j < cellSettings.Count; j++)
            {
                sh.SetColumnWidth(j, cellSettings[j].CellWidth);//设置单元的宽度
            }

            #region 合并单元格

            sh.AddMergedRegion(new CellRangeAddress(0, 0, 0, cellSettings.Count - 1));

            IRow row0 = sh.CreateRow(0);
            row0.Height = 20 * 20;
            ICell icell1Top0 = row0.CreateCell(0);
            icell1Top0.CellStyle = ExcepNpoi.Getcellstyle(wb, ExcepNpoi.Stylexls.头);
            icell1Top0.SetCellValue($"SZMJ-{sheetName}:{DateTime.Now.ToString(CultureInfo.InvariantCulture)}");

            #endregion

            #region 设置表头

            IRow row1 = sh.CreateRow(1);
            row1.Height = 20 * 20;

            for (int j = 0; j < cellSettings.Count; j++)
            {
                ICell icellTop = row1.CreateCell(j);
                icellTop.CellStyle = ExcepNpoi.Getcellstyle(wb, ExcepNpoi.Stylexls.头);
                icellTop.SetCellValue(cellSettings[j].Name);
            }

            #endregion

            #region 写入数据

            int i = 1;

            NumberFormatInfo provider = new NumberFormatInfo
            {
                NumberDecimalDigits = 1
            };

            foreach (var dtRow in dt.Rows)
            {
                //创建行
                IRow row = sh.CreateRow(++i);
                row.Height = 18 * 20;

                for (int j = 0; j < cellSettings.Count; j++)
                {
                    ICell iCells = row.CreateCell(j);
                    iCells.CellStyle = cellSettings[j].CellStyle;

                    if (Equals(iCells.CellStyle, cellStyles["数字"]))
                        iCells.SetCellValue(double.Parse(((DataRow)dtRow).ItemArray[j].ToString(), provider));
                    else
                        iCells.SetCellValue(((DataRow)dtRow).ItemArray[j].ToString());
                }
            }

            #endregion

        }


        public class CellSetting
        {
            public string Name { get; set; }
            public ICellStyle CellStyle { get; set; }
            public int CellWidth { get; set; }
        }
    }
}