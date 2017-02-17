using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.BaseUtility;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Carlzhu.Iooin.WebApp.Controllers
{
    public class SchedulerController : Controller
    {
        private SchedulerTaskService taskService;



        public SchedulerController()
        {
            this.taskService = new SchedulerTaskService();

        }

        public ActionResult Index()
        {
            return View();
        }

        public virtual JsonResult Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(taskService.GetAll().ToDataSourceResult(request));
        }

        public virtual JsonResult Destroy([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                taskService.Delete(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult Create([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                taskService.Insert(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult Update([DataSourceRequest] DataSourceRequest request, TaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                taskService.Update(task, ModelState);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            taskService.Dispose();

            base.Dispose(disposing);
        }
    }


    public class SchedulerTaskService : ISchedulerEventService<TaskViewModel>
    {

        private CarlzhuContext db;



        public SchedulerTaskService()
        {
            db = CarlzhuContext.CzContext;
        }

        public virtual IQueryable<TaskViewModel> GetAll()
        {
            return db.Tasks.ToList().Select(task => new TaskViewModel
            {
                TaskID = task.TaskID,
                Title = task.Title,
                Start = DateTime.SpecifyKind(task.Start, DateTimeKind.Utc),
                End = DateTime.SpecifyKind(task.End, DateTimeKind.Utc),
                StartTimezone = task.StartTimezone,
                EndTimezone = task.EndTimezone,
                Description = task.Description,
                IsAllDay = task.IsAllDay,
                RecurrenceRule = task.RecurrenceRule,
                RecurrenceException = task.RecurrenceException,
                RecurrenceID = task.RecurrenceID,
                OwnerID = task.OwnerID
            }).AsQueryable();
        }

        public virtual void Insert(TaskViewModel task, ModelStateDictionary modelState)
        {
            if (ValidateModel(task, modelState))
            {
                if (string.IsNullOrEmpty(task.Title))
                {
                    task.Title = "";
                }

                var entity = task.ToEntity();

                db.Tasks.Add(entity);
                db.SaveChanges();

                task.TaskID = entity.TaskID;
            }
        }

        public virtual void Update(TaskViewModel task, ModelStateDictionary modelState)
        {
            if (ValidateModel(task, modelState))
            {
                if (string.IsNullOrEmpty(task.Title))
                {
                    task.Title = "";
                }

                var entity = task.ToEntity();
                db.Tasks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public virtual void Delete(TaskViewModel task, ModelStateDictionary modelState)
        {
            var entity = task.ToEntity();
            db.Tasks.Attach(entity);

            var recurrenceExceptions = db.Tasks.Where(t => t.RecurrenceID == task.TaskID);

            foreach (var recurrenceException in recurrenceExceptions)
            {
                db.Tasks.Remove(recurrenceException);
            }

            db.Tasks.Remove(entity);
            db.SaveChanges();
        }

        //TODO: better naming or refactor
        private bool ValidateModel(TaskViewModel appointment, ModelStateDictionary modelState)
        {
            if (appointment.Start > appointment.End)
            {
                modelState.AddModelError("errors", "End date must be greater or equal to Start date.");
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }

    public interface ISchedulerEventService<T> : IDisposable where T : class, ISchedulerEvent
    {
        void Delete(T appointment, ModelStateDictionary modelState);
        IQueryable<T> GetAll();
        void Insert(T appointment, ModelStateDictionary modelState);
        void Update(T appointment, ModelStateDictionary modelState);
    }


    public class TaskViewModel : ISchedulerEvent
    {
        public int TaskID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        private DateTime start;
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value.ToUniversalTime();
            }
        }

        public string StartTimezone { get; set; }

        private DateTime end;
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value.ToUniversalTime();
            }
        }

        public string EndTimezone { get; set; }

        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        public bool IsAllDay { get; set; }
        public int? OwnerID { get; set; }

        public TaskSch ToEntity()
        {
            return new TaskSch
            {
                TaskID = TaskID,
                Title = Title,
                Start = Start,
                StartTimezone = StartTimezone,
                End = End,
                EndTimezone = EndTimezone,
                Description = Description,
                RecurrenceRule = RecurrenceRule,
                RecurrenceException = RecurrenceException,
                RecurrenceID = RecurrenceID,
                IsAllDay = IsAllDay,
                OwnerID = OwnerID
            };
        }
    }


}