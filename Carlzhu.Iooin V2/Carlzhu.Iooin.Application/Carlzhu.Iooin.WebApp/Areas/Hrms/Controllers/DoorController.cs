using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Carlzhu.Iooin.Business;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.FormModule;
using Carlzhu.Iooin.Business.HrmsModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Extension;

using Carlzhu.Iooin.WebApp.Areas.eForm.Controllers;

namespace Carlzhu.Iooin.WebApp.Areas.Hrms.Controllers
{
    [LoginAuthorize]
    public class DoorController : FormControllerBase
    {
        //
        // GET: /Hrms/Door/


        readonly EmployeeBll _hrEmployee = new EmployeeBll();
        readonly BaseServices<DoorInout> _doorInoutService = new BaseServices<DoorInout>();



        public  ActionResult Index()
        {

            IEnumerable<SelectListItem> formSigns = new Signing().GetSignDataList(base.EmpNo).Where(k => k.Form.FormId == 16).ToList().Select(c => new SelectListItem()
            {
                Text = c.Form.BaseEmployee.RealName,
                Value = c.Form.CreateEmpNo
            });

            formSigns = formSigns.Distinct(new PropertyComparer<SelectListItem>("Value"));

            ViewBag.UserList = formSigns;


            //---------------------------
            //BaseServices<Form> formService = new BaseServices<Form>();

            //IEnumerable<SelectListItem> forms = formService.LoadEntities(c => c.FormId == 16 && c.FormStatus == (int)Carlzhu.Iooin.Entity.FORM.Form.StatusEnum.签核中)
            //    .Distinct(new PropertyComparer<Form>("CreateEmpNo"))
            //    .Select(c => new SelectListItem()
            //    {
            //        Text = c.BaseEmployee.RealName,
            //        Value = c.CreateEmpNo
            //    });
            //ViewBag.UserList = forms;






            return View();

        }
        public JsonResult Inouting(string cardNo, int forward)
        {
            var model = ContextFactory.ContextHelper.BaseEmployees.FirstOrDefault(c => c.IsDimission != 0 && c.CardNo.Equals(cardNo));

            if (model == null) return Json("error");



            if (!_doorInoutService.IsAddEntity(new DoorInout()
            {
                CardNo = cardNo,
                Forward = forward,
                EventTime = DateTime.Now,
                Record = ManageProvider.Provider.Current().UserId
            }))
            {
                return Json("error");
            }
            var employee = new BaseEmployee
            {
                EmpNo = model.EmpNo,
                RealName = model.RealName,
                DepartmentId = model.DepartmentId,
            };
            var serializer = new JavaScriptSerializer();
            return Json(serializer.Serialize(employee));
        }


        public ActionResult InOut(string user = "")
        {
            var date = DateTime.Now;
            DateTime timestart = !string.IsNullOrEmpty(Request["timestart"]) ? DateTime.Parse(Request["timestart"]) :
                DateTime.Parse($"{date.Year}/{date.Month}/{date.Day} 00:00:00");
            DateTime timeend = !string.IsNullOrEmpty(Request["timeend"])
                ? DateTime.Parse($"{Request["timeend"]} 23:59:00")
                : DateTime.Parse($"{date.Year}/{date.Month}/{date.Day} 23:59:00");





            var reports = new List<DoorInoutReport>();
            EmployeeBll hrEmployee = new EmployeeBll();


            var list = _doorInoutService.LoadEntities(c => c.EventTime > timestart && c.EventTime < timeend).GroupBy(c => c.CardNo).ToList();



            list.ForEach(c =>
            {
                //if (c.Key == "0009200775")
                //    Console.Write("s");

                //个人进出记录
                var data = c.ToList().OrderBy(m => m.EventTime).ToList();

                //个人进出次数统计
                int row = 0;
                data.ForEach(k =>
                {
                    var outdoor = data.ToList().FirstOrDefault(d => d.RowId > row && d.Forward == 1);
                    if (outdoor != null) row = outdoor.RowId;
                    var indoor = data.ToList().FirstOrDefault(d => d.RowId > row && d.Forward == 0);
                    if (indoor != null) row = indoor.RowId;

                    if (outdoor == null) { return; }
                    var employee = ContextFactory.ContextHelper.BaseEmployees.Single(s => s.IsDimission != 0 && s.CardNo == outdoor.CardNo);

                    if (indoor != null)
                    {
                        TimeSpan ts = indoor.EventTime - outdoor.EventTime;
                        reports.Add(new DoorInoutReport
                        {
                            Name = employee.RealName,
                            Department = employee.BaseDepartment.FullName,
                            CardNo = indoor.CardNo,
                            OutTime = outdoor.EventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            InTime = indoor.EventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            SumTime = $"{ts.Hours}小时{ts.Minutes}分"
                        });

                    }
                    else
                    {
                        reports.Add(new DoorInoutReport()
                        {
                            Name = employee.RealName,
                            Department = employee.BaseDepartment.FullName,
                            CardNo = outdoor.CardNo,
                            OutTime = outdoor.EventTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            InTime = "",
                            SumTime = "",
                        });

                    }
                });

            });
            var model = reports.Where(c => c.Name.Contains(user)).OrderBy(c => c.OutTime).ThenBy(c => c.InTime);

            return View(model);
        }


        //Car
        #region Car



        public ContentResult CarOut(HrmsCarInOut hrmsCarInOut)
        {
            BaseServices<HrmsCar> hrmsCarService = new BaseServices<HrmsCar>();
            var carmodel = hrmsCarService.LoadEntities(c => c.No == hrmsCarInOut.CarNo).First();
            carmodel.Location = 1;
            carmodel.FormNo = hrmsCarInOut.FormNo;
            carmodel.CurrentKilometers = hrmsCarInOut.OutKilometers;
            if (!hrmsCarService.UpdateEntity(carmodel)) return Content("车辆状态更新失败");

            hrmsCarInOut.OutTime = DateTime.Now;
            hrmsCarInOut.InTime = DateTime.MaxValue;
            hrmsCarInOut.Oil = 0;
            hrmsCarInOut.InKilometers = 0;
            if (new BaseServices<HrmsCarInOut>().IsAddEntity(hrmsCarInOut))
            {
                return
                    Content(new Signing().Agree(hrmsCarInOut.FormNo, 0, "1000000")
                        ? "请求成功"
                        : "表单签核失败！！！");
            }
            return Content("外出失败");
        }

        public ContentResult CarIn(HrmsCarInOut hrmsCarInOut)
        {


            BaseServices<HrmsCar> hrmscarService = new BaseServices<HrmsCar>();
            BaseServices<HrmsCarInOut> hrmsCarInOutService = new BaseServices<HrmsCarInOut>();

            var carmodel = hrmscarService.LoadEntities(c => c.No == hrmsCarInOut.CarNo).FirstOrDefault();
            if (carmodel == null)
            {
                return Content("车辆不存在");
            }

            carmodel.CurrentKilometers = hrmsCarInOut.InKilometers;
            carmodel.Location = 0;

            if (!hrmscarService.UpdateEntity(carmodel)) return Content("系统异常");
            var inoutmodel =
                hrmsCarInOutService.LoadEntities(c => c.FormNo == carmodel.FormNo && c.CarNo == carmodel.No)
                    .FirstOrDefault();
            if (inoutmodel == null) return Content("车辆回来失败");
            inoutmodel.InTime = DateTime.Now;
            inoutmodel.InKilometers = hrmsCarInOut.InKilometers;
            inoutmodel.Oil = hrmsCarInOut.Oil;


            return Content(hrmsCarInOutService.UpdateEntity(inoutmodel) ? "请求成功" : "请求失败，请重试");
        }



        #endregion


        public JsonResult GetApplyUserInfo(string empNo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<tr><td>单号</td><td>客户</td><td>目的地</td></tr>");
            BaseServices<FormCar> formcarService = new BaseServices<FormCar>();



            var allData = //formcarService.LoadEntities(c => true).ToList().Where(c => c.Form.CreateEmpNo == empNo && c.Form.FormStatus == (int)Form.StatusEnum.签核中).ToList();
                          // new Business.BaseServices<FormCar>().LoadEntities(c => c.Form.CreateEmpNo == empNo && c.Form.FormStatus == (int)Form.StatusEnum.签核中).ToList();
            ContextFactory.ContextHelper.FormCars.Where(c => c.Form.CreateEmpNo == empNo && c.Form.FormStatus == (int)Form.StatusEnum.签核中).ToList();

            List<FormSign> formSigns = new Signing().GetSignDataList(base.EmpNo).Where(m => m.Form.FormId == 16).ToList();







            var listCar = new List<FormCar>();
            formSigns.ForEach(k => listCar.AddRange(allData.Where(c => c.FormNo == k.FormNo)));


            listCar.ForEach(
                  k =>
                  {
                      sb.AppendLine("<tr>");
                      sb.AppendLine("<td><input type='radio' name='form' />" + k.FormNo + "</td>");
                      //sb.AppendLine("<td>" + k.Type + "</td>");
                      sb.AppendFormat("<td>{0}/{1}</td>", k.CustomerNo, k.Customer.CustomerName);
                      sb.AppendLine("<td>" + k.Addr + "</td>");
                      sb.AppendLine("</tr>");

                  });
            var user = listCar.FirstOrDefault();
            string[] result = { user != null ? $"{user.Form.BaseEmployee.DepartmentId}/{user.Form.CreateEmpNo}" : "", sb.ToString() };

            return Json(result);
        }

        public JsonResult GetCar(string carNo)
        {
            var model = DataFactory.Database().FindEntity<HrmsCar>("No", carNo);
            //var model = new BaseServices<HrmsCar>().LoadEntities(c => c.No.Equals(carNo)).FirstOrDefault();
            return Json(model?.ToJson());
            //return Json(model == null ? null : new { Nameplate = model.Nameplate, Name = model.Name, Location = model.Location, CurrentKilometers = model.CurrentKilometers });
        }


        public ActionResult CarInoutQuery(string extend)
        {

            var date = DateTime.Now;
            DateTime timestart = !string.IsNullOrEmpty(Request["timestart"]) ? DateTime.Parse(Request["timestart"]) :
                DateTime.Parse($"{date.Year}/{date.Month}/{date.Day} 00:00:00");
            DateTime timeend = !string.IsNullOrEmpty(Request["timeend"])
                ? DateTime.Parse($"{Request["timeend"]} 23:59:00")
                : DateTime.Parse($"{date.Year}/{date.Month}/{date.Day} 23:59:00");


            BaseServices<HrmsCarInOut> hrmscarinoutService = new BaseServices<HrmsCarInOut>();
            var models = hrmscarinoutService.LoadEntities(c => c.OutTime > timestart && c.OutTime < timeend).ToList();

            if (!string.IsNullOrEmpty(extend))
            {
                models =
                    models.Where(
                        c =>
                            c.FormNo.Contains(extend) || c.HrmsCar.Nameplate.Contains(extend) ||
                            c.BaseEmployee.RealName.Contains(extend)).ToList();

            }

            return View(models);
        }


    }
}
