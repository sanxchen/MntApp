using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.FORM.f;

namespace Carlzhu.Iooin.WebApp.Areas.eSystem.Controllers
{
    public class QualityController : Controller
    {

        private readonly CarlzhuContext _context = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper;


        // GET: eSystem/Quality
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult WattingTest()
        {

            var models = _context.FormWorkshopInspections.Where(c => c.Form.FormStatus == 3 && c.EndDateTime == null).OrderByDescending(c => c.Order).ThenBy(c => c.RowId);

            return View(models);
        }

        public ActionResult Test(int id)
        {

            Carlzhu.Iooin.Business.BaseServices<FormWorkshopInspection> wBaseServices = new Carlzhu.Iooin.Business.BaseServices<FormWorkshopInspection>();
            var models = wBaseServices.LoadEntities(c => c.RowId == id).First();
            return View(models);
        }

        public ActionResult TestStart(int id, string emp)
        {
            Carlzhu.Iooin.Business.BaseServices<FormWorkshopInspection> wBaseServices = new Carlzhu.Iooin.Business.BaseServices<FormWorkshopInspection>();
            var models = wBaseServices.LoadEntities(c => c.RowId == id).First();
            models.StartDateTime = DateTime.Now;
            models.Checker = emp;
            return Json(wBaseServices.UpdateEntity(models) ? "success" : "fail");
        }


        public ActionResult SaveTesting(FormWorkshopInspection formWorkshopInspection)
        {
            Carlzhu.Iooin.Business.BaseServices<FormWorkshopInspection> wBaseServices = new Carlzhu.Iooin.Business.BaseServices<FormWorkshopInspection>();
            var models = wBaseServices.LoadEntities(c => c.RowId == formWorkshopInspection.RowId).First();

            models.EndDateTime = DateTime.Now;
            models.Explan = formWorkshopInspection.Explan;
            models.DetectionResult = formWorkshopInspection.DetectionResult;
            models.Picture = formWorkshopInspection.Picture;
            models.CanTakeit = true;
            wBaseServices.UpdateEntity(models);
            return RedirectToAction("WattingTest", "Quality", new { area = "eSystem" });

        }


        public ActionResult Display()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult Kanban(string method, string style)
        {
            ViewBag.Style = style;
            switch (method)
            {

                case "0":
                    return PartialView("/Areas/eSystem/Views/quality/_DisplayItem.cshtml", _context.FormWorkshopInspections.Where(c => !c.CanTakeit && c.StartDateTime == null && c.Form.FormStatus == 3).ToList());
                case "1":
                    return PartialView("/Areas/eSystem/Views/quality/_DisplayItem.cshtml", _context.FormWorkshopInspections.Where(c => !c.CanTakeit && c.StartDateTime != null && c.Form.FormStatus == 3).ToList());
                case "2":
                    return PartialView("/Areas/eSystem/Views/quality/_DisplayItem.cshtml", _context.FormWorkshopInspections.Where(c => c.CanTakeit).ToList());
                default:
                    return Link.ErrorBy(new Exception("参数异常"), this.GetType());
            }

        }


        public ActionResult HistoryRecords(string extend)
        {
            var date = DateTime.Now;
            DateTime timestart = !string.IsNullOrEmpty(Request["timestart"]) ? DateTime.Parse(Request["timestart"]) :
                DateTime.Parse($"{date.Year}/{date.Month}/{date.Day} 00:00:00");
            DateTime timeend = !string.IsNullOrEmpty(Request["timeend"])
                ? DateTime.Parse($"{Request["timeend"]} 23:59:00")
                : DateTime.Parse($"{date.Year}/{date.Month}/{date.Day} 23:59:00");


            List<FormWorkshopInspection> models;

            if (!string.IsNullOrEmpty(extend))
            {
                models = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper.FormWorkshopInspections.Where(
                      c => (c.CanTakeit == true) &&
                          (c.EndDateTime > timestart && c.EndDateTime < timeend) &&
                          (c.TransferMachine.Contains(extend) || c.ProductNo.Contains(extend) ||
                           c.BaseEmployee.RealName.Contains(extend))).ToList();

            }
            else
            {

                models = Carlzhu.Iooin.Business.BaseUtility.ContextFactory.ContextHelper.FormWorkshopInspections.Where(c => c.CanTakeit && (c.EndDateTime > timestart && c.EndDateTime < timeend)).ToList();
            }

            return View(models);
        }

    }
}