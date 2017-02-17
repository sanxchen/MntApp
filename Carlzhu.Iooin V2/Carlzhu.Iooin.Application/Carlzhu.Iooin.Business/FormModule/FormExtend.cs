using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Carlzhu.Iooin.Business.BaseModule;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.QualityModule;
using Carlzhu.Iooin.Entity.CommonModule;

using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.Entity.FORM.f.draw;
using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Extension;
using TpaCustomer = Carlzhu.Iooin.Entity.TPA.TpaCustomer;

namespace Carlzhu.Iooin.Business.FormModule
{

    /// <summary>
    /// 控制签核流程与表单结案时的操作
    /// </summary>
    public class FormExtend
    {
        private static FormType _formType;

        private static string _empNo;



        /// <summary>
        /// 表单创建前要做的事情
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="dictionary"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public static List<BaseEmployee> SetFormStart(int formId, Dictionary<string, object> dictionary, string empNo)
        {
            _empNo = empNo;

            _formType =//new BaseServices<FormType>().LoadEntities(c => c.FormId == formId).First();
            DataFactory.Database().FindEntity<FormType>(formId);
            try
            {

                if (!_formType.IsStart)
                {
                    return GenerateSignList(_formType.RouteOne.Split('|'), empNo);
                }

                Type cls = typeof(FormExtend);
                object instance = Activator.CreateInstance(cls);//实例这个类的对象

                MethodInfo method = cls.GetMethod($"_{formId}Start",
                        BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { dictionary.GetType() },//方法所需的参数类型
                        null);

                return GenerateSignList(method.Invoke(instance, new object[] { dictionary }).ToString().Split('|'), empNo);



            }
            catch (Exception)
            {
                //反回默认方法
                return GenerateSignList(_formType.RouteOne.Split('|'), empNo);
            }

        }

        /// <summary>
        /// 根据Route和提交人生成表单签核人
        /// </summary>
        /// <param name="route"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        private static List<BaseEmployee> GenerateSignList(IList<string> route, string empNo)
        {
            var section1 = route[0];
            var section2 = route[1];
            var section3 = route[2];

            //最终签核人
            var signList = new List<BaseEmployee>();

            var employees = new BaseServices<BaseEmployee>().LoadEntities(c => c.IsDimission != 0).ToList();

            var my = DataFactory.Database().FindEntity<BaseEmployee>(empNo);
            //该设定为自已签核
            if (_formType.IsMyself)
            {
                signList.Add(my);
                return signList;
            }



            var patten = new Regex("\\d{7}");
            //部门内成员
            if (!string.IsNullOrEmpty(section1))
            {
                //T,S,P,M,1109001
                var applypath = section1.ToUpper().Split(',').ToList().RemoveStringNullOrEmpty();

                foreach (var c in applypath)
                {
                    if (patten.IsMatch(c))
                    {
                        signList.Add(employees.Single(k => k.EmpNo == c));
                    }
                    else
                    {
                        signList.Add(GetDepartmentTopOneByPosition(employees, my.DepartmentId, c));
                    }
                }
            }
            //部门外成员
            if (!string.IsNullOrEmpty(section2))
            {
                //HRS00:M,//M0000:M,//HRS00:P

                foreach (var c in section2.ToUpper().Split(',').ToList().RemoveStringNullOrEmpty())
                {
                    if (patten.IsMatch(c))
                    {
                        signList.Add(employees.Single(k => k.EmpNo == c));
                    }
                    else
                    {
                        signList.Add(GetDepartmentTopOneByPosition(employees, c.Split(':')[0], c.Split(':')[1]));
                    }
                }

               
            }
            //审核部门成员
            if (!string.IsNullOrEmpty(section3))
            {
                //M,S,P,1109001

                var operatorpath = section3.ToUpper().Split(',').ToList().RemoveStringNullOrEmpty();
                
                //处理人所属部门
                var departCode = employees.Single(x => x.EmpNo == operatorpath.Last()).DepartmentId;

                foreach (var c in operatorpath)
                {
                    if (patten.IsMatch(c))
                    {
                        signList.Add(employees.Single(k => k.EmpNo == c));
                    }
                    else
                    {
                        signList.Add(GetDepartmentTopOneByPosition(employees, departCode, c));
                    }
                }


               // operatorpath.ForEach(c => signList.Add(patten.IsMatch(c) ? employees.Single(k => k.EmpNo == c) : GetDepartmentTopOneByPosition(employees, departCode, c)));
            }

            signList.RemoveAll(c => c == null);
            return signList;
        }


        /// <summary>
        /// 流程生成
        /// </summary>
        /// <param name="employees"></param>
        /// <param name="department"></param>
        /// <param name="positionCode"></param>
        /// <returns></returns>
        private static BaseEmployee GetDepartmentTopOneByPosition(IEnumerable<BaseEmployee> employees, string department, string positionCode)
        {
            var my = DataFactory.Database().FindEntity<BaseEmployee>(_empNo);

            //如果是M取部门长官并加入公司编码
            department = positionCode.ToUpper().Contains("M") ? (department.Substring(0, 2) + my.DepartmentId.Substring(2, 1)).PadRight(5, '0') : department;


            try
            {
                //代理主管
              //  if (department == "PTA00") department = "PDA00";
                if (my.DepartmentId == "PTA20") department = "PEA00"; //设备课改成工程
                if (my.DepartmentId == "PTA10") department = "PDA00"; //工艺课改为生产

                return employees.Where(c => c.DepartmentId == department && c.Position.Contains(positionCode.Substring(0, 1))).OrderBy(m => m.Position).First();
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 表单结案时要做的事情
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        public static Func<string, string, int, bool> SetFormFinish(FormType formType)
        {
            _formType = formType;
            try
            {
                //static
                return (Func<string, string, int, bool>)Delegate.CreateDelegate(typeof(Func<string, string, int, bool>), typeof(FormExtend).GetMethod(string.Format("_{0}Finish", _formType.FormId), BindingFlags.Static | BindingFlags.NonPublic));

                //new，method behind add newname
                //return (Func<string, bool>)Delegate.CreateDelegate(typeof(Func<FormExtend,string, bool>), typeof(FormExtend).GetMethod(instance,string.Format("_{0}Finish", _formType.FormId)));

                //exec
                //func.DynamicInvoke(new object[] { formNo }).ToString().ToLower() == "true";
            }
            catch (Exception)
            {
                //未成功找到对应结案方法则反回结案成功
                //参数：表单号操作人和记录号
                return (formno, empNo, item) => false;
            }
        }


        #region Start

        private string _1Start(Dictionary<string, object> model)
        {
            return _formType.RouteOne;
        }

        private string _48Start(Dictionary<string, object> model)
        {
            BaseServices<BaseEmployee> employeeService = new BaseServices<BaseEmployee>();
            var firstOrDefault = employeeService.LoadEntities(c => c.EmpNo == _empNo).FirstOrDefault();

            if (firstOrDefault == null) return _formType.RouteOne;

            var position = firstOrDefault.Position.Substring(0, 1);

            return position == "M" ? _formType.RouteTwo : _formType.RouteOne;
        }


        #endregion

        #region Finish

        /// <summary>
        /// 系统测试结案
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool _1Finish(string formNo, string empNo, int item) { return false; }


        /// <summary>
        /// 图纸替换结案
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool _19Finish(string formNo, string empNo, int item)
        {
            return new Quality().DrawReplace(formNo, empNo);
        }

        /// <summary>
        /// IT设备申请结案
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool _23Finish(string formNo, string empNo, int item) { return false; }

        /// <summary>
        /// 印章申请结案
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool _24Finish(string formNo, string empNo, int item) { return false; }


        /// <summary>
        /// 异常出勤
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool _25Finish(string formNo, string empNo, int item)
        {

            FormAbnormalAttendance formEntity = new Applying().GetFormEntityByFormNo(formNo, _formType);



            //上班卡
            StringBuilder sb = new StringBuilder($"INSERT INTO EASTRIVER.DBO.TIMERECORDS (clock_id,emp_id,card_id,sign_time,mark,flag,bill_id,dcollecttime,eventName) " +
                                                 $"VALUES " +
                                                 $"(2,'{formEntity.EmpNo}','{formEntity.BaseEmployee.CardNo}','{formEntity.TimeStart}','{formEntity.AbnormalType}','0','{formNo}','{DateTime.Now}','');");

            //下班卡
            sb.Append(
                $"INSERT INTO EASTRIVER.DBO.TIMERECORDS (clock_id,emp_id,card_id,sign_time,mark,flag,bill_id,dcollecttime,eventName) " +
                $"VALUES " +
                $"(2,'{formEntity.EmpNo}','{formEntity.BaseEmployee.CardNo}','{formEntity.TimeEnd}','{formEntity.AbnormalType}','0','{formNo}','{DateTime.Now}','');");

            int r = DataFactory.Database().ExecuteBySql(sb);

            return r > 0;

        }

        #region NCR



        /// <summary>
        /// NCR表单结案
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool _30Finish(string formNo, string empNo, int item)
        {
            //发送异常邮件
            var model = new BaseServices<FormNcr>().LoadEntities(c => c.FormNo == formNo).First();
            model.AuditEmp = empNo;
            model.AuditTime = DateTime.Now;

            if (!new BaseServices<FormNcr>().UpdateEntity(model)) return false;

            //  CreatePath(formNo, model.Form.CreateEmpNo, 31, "临时对策", GetHolder(model.FormNo));

            var emailForm = new QualityAbnormalBll(formNo, empNo);
            emailForm.NoticeA();
            return TemplateBll.SendBpm(emailForm);
        }

        private static bool _31Finish(string formNo, string empNo, int item)
        {
            var model = new BaseServices<FormNcrPart>().LoadEntities(c => c.FormNo == formNo && c.ReplyType == 31).First();
            model.AuditEmp = empNo;
            model.AuditTime = DateTime.Now;

            return new BaseServices<FormNcrPart>().UpdateEntity(model);

        }
        private static bool _35Finish(string formNo, string empNo, int item)
        {
            var model = new BaseServices<FormNcrPart>().LoadEntities(c => c.FormNo == formNo && c.ReplyType == 35).First();
            model.AuditEmp = empNo;
            model.AuditTime = DateTime.Now;

            return new BaseServices<FormNcrPart>().UpdateEntity(model);

        }
        private static bool _36Finish(string formNo, string empNo, int item)
        {
            var model = new BaseServices<FormNcrPart>().LoadEntities(c => c.FormNo == formNo && c.ReplyType == 36).First();
            model.AuditEmp = empNo;
            model.AuditTime = DateTime.Now;

            return new BaseServices<FormNcrPart>().UpdateEntity(model);

        }


        private static bool _37Finish(string formNo, string empNo, int item)
        {
            var model = new BaseServices<FormNcrPart>().LoadEntities(c => c.FormNo == formNo && c.ReplyType == 37).First();
            model.AuditEmp = empNo;
            model.AuditTime = DateTime.Now;
            return new BaseServices<FormNcrPart>().UpdateEntity(model);

        }


        //是否可以结案
        private static bool _45Finish(string formNo, string empNo, int item)
        {
            BaseServices<FormPath> pathService = new BaseServices<FormPath>();
            var model = pathService.LoadEntities(c => c.FormNo == formNo).First();


            //检查记录是否签核完成
            return new BaseServices<FormNcrPart>().Count(
                m =>
                    m.ParentFormNo == model.ParentFormNo && m.ReplyType == model.FormId &&
                    m.Form.FormStatus == (int)Form.StatusEnum.签核完成) > 0;
        }






        private static bool _49Finish(string formNo, string empNo, int item)
        {
            //发布新异常

            return true;
        }


        /// <summary>
        /// 创建责任路径
        /// </summary>
        /// <param name="parentFormNo"></param>
        /// <param name="createEmp"></param>
        /// <param name="partId"></param>
        /// <param name="partMark"></param>
        /// <param name="signEmp"></param>
        /// <returns></returns>
        private static bool CreatePath(string parentFormNo, string createEmp, int partId, string partMark, string signEmp)
        {
            var formpath = new FormPath { FormId = partId, ParentFormNo = parentFormNo, Mark = "NCR异常单-" + partMark };
            var form = new Form
            {
                FormNo = new Applying().CreateFormNo(new object()),
                FormId = 45,
                CreateEmpNo = createEmp,
                CreateTime = DateTime.Now,
                SignPath = signEmp + ",",
                FormStatus = 0,
                IsEmergents = true,
            };
            return new F<FormPath>().SaveData(new List<FormPath> { formpath }, form) && new Applying().Send(form.FormNo, createEmp);
        }


        private static string GetHolder(string ncrFormNo)
        {
            string sign = "1109001,";
            var model = new BaseServices<FormNcr>().LoadEntities(c => c.FormNo == ncrFormNo).First();

            var customer = new BaseServices<TpaCustomer>().LoadEntities(c => c.CustomerNo == model.CustomerNo).First();
            switch (model.AbnormalPoint)
            {
                case (int)FormNcr.AbnormalPointEnum.ReceivingInspection:
                    sign = "1509002,";//陈英-->李秋霞
                    break;
                case (int)FormNcr.AbnormalPointEnum.SemiFinishedProductsInspection:
                    sign = string.IsNullOrEmpty(customer.EmpQuality) ? "1108001," : customer.EmpQuality + ",";
                    break;
                case (int)FormNcr.AbnormalPointEnum.FqCtest:
                    sign = "0810001,";//小吴
                    break;
                case (int)FormNcr.AbnormalPointEnum.ProcessAbnormality:
                    switch (model.AbnormalPointWorkshop)
                    {
                        case (int)FormNcr.AbnormalPointWorkshopEnum.Product:
                            sign = "0908001,";//昌明
                            break;
                        case (int)FormNcr.AbnormalPointWorkshopEnum.ProductOne:
                            sign = "0810001,";//小吴
                            break;
                        case (int)FormNcr.AbnormalPointWorkshopEnum.ProductTwo:
                            sign = "0908001,";//昌明
                            break;
                        case (int)FormNcr.AbnormalPointWorkshopEnum.ProductThree:
                            sign = "1011001,";//小陆
                            break;
                        case (int)FormNcr.AbnormalPointWorkshopEnum.ProductFour:
                            sign = "1212011,";//施喜红
                            break;
                    }
                    break;
                case (int)FormNcr.AbnormalPointEnum.Customer:
                    sign = string.IsNullOrEmpty(customer.EmpQuality) ? "1108001," : customer.EmpQuality + ",";
                    break;
            }
            return sign;
        }


        private static bool _50Finish(string formNo, string empNo, int item)
        {
            var current = ContextFactory.ContextHelper.FormDrawingsSopDewells.First(c => c.FormNo == formNo);


            BaseServices<FormDrawingsSopDewell> database = new BaseServices<FormDrawingsSopDewell>();
            var updates = database.LoadEntities(c => c.DrawPartNo == current.DrawPartNo && c.Tag == current.Tag && c.CustomerNo == current.CustomerNo).OrderByDescending(c => c.RowId).ToList();
            if (updates.Count <= 1) return true;

            for (int i = 1; i < updates.Count; i++)
            {
                var update = updates[i];
                update.IsDel = true;
                return database.UpdateEntity(update);
            }


            return false;
        }



        /// <summary>
        /// 更新插队结果
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool _53Finish(string formNo, string empNo, int item)
        {
            BaseServices<FormWorkshopInspection> wService = new BaseServices<FormWorkshopInspection>();

            var jumformNo = ContextFactory.ContextHelper.FormJumpTheQueues.First(c => c.FormNo == formNo).JumpForm;
            var jumpform = wService.LoadEntities(c => c.FormNo == jumformNo).First();


            //生成当前最大时间
            DateTime oldTime = new DateTime(2016, 1, 1);
            double milliseconds = (DateTime.Now - oldTime).TotalMilliseconds;
            jumpform.Order = milliseconds;


            return wService.UpdateEntity(jumpform);
        }


        #endregion
























        #endregion
    }
}
