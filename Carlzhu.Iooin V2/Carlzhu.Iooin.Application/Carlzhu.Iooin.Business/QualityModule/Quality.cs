using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.FormModule;

using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.Entity.FORM.f.draw;
using Carlzhu.Iooin.Entity.QUALITY;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.QualityModule
{
    public class Quality
    {





        /// <summary>
        /// 取得待发行表单
        /// </summary>
        /// <returns></returns>

        public IEnumerable<FormSign> GetPublishingList(string empNo)
        {
            //所有待签记录
            //从特签记录中找出最后签核人是自己的所有需要发行（PDM）的记录供发行
            return new Forms().GetSignDataList(empNo).Where(c => c.Form.FormType.Os == "PDM" && string.IsNullOrEmpty(c.Form.SignPath)).ToList();

        }







        /// <summary>
        /// 表单发行
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="empNo"></param>
        /// <param name="publishTime"></param>
        /// <param name="isPass"></param>
        /// <returns></returns>
        public bool Published(string formNo, int item, string empNo, DateTime? publishTime, bool isPass)
        {
            //将表单改为 已发行
            var formType = new Applying().GetFormByFormNo(formNo).FormType;

            bool result;



            switch (formType.Method)
            {
                case "DrawingsEcn":
                    var ecnEntity = new BaseServices<FormDrawingsEcn>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(ecnEntity, c =>
                        c.IsPublished
                        && c.CustomerNo == ecnEntity.CustomerNo
                        && c.ProductNo == ecnEntity.ProductNo
                        , item, empNo, formType, publishTime, isPass);


                    break;
                case "DrawingsCustomer":
                    var customerEntity = new BaseServices<FormDrawingsCustomer>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(customerEntity, c =>
                        c.IsPublished
                        && c.CustomerNo == customerEntity.CustomerNo
                        && c.ProductNo == customerEntity.ProductNo
                , item, empNo, formType, publishTime, isPass);
                    break;
                case "DrawingsInside":
                    var insideEntity = new BaseServices<FormDrawingsInside>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(insideEntity, c =>
                        c.IsPublished
                        && c.CustomerNo == insideEntity.CustomerNo
                        && c.ProductNo == insideEntity.ProductNo
                        , item, empNo, formType, publishTime, isPass);
                    break;
                case "DrawingsProfile":
                    var profileEntity = new BaseServices<FormDrawingsProfile>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(profileEntity, c =>
                        c.IsPublished
                        && c.CustomerNo == profileEntity.CustomerNo
                        && c.ProfileNo == profileEntity.ProfileNo
                        //  && c.ProductNo == profileEntity.ProductNo
                        , item, empNo, formType, publishTime, isPass);
                    break;
                case "DrawingsProcess":
                    var processEntity = new BaseServices<FormDrawingsProcess>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(processEntity, c =>
                        c.IsPublished
                        && c.CustomerNo == processEntity.CustomerNo
                        && c.ProductNo == processEntity.ProductNo
                        , item, empNo, formType, publishTime, isPass);
                    break;
                case "DrawingsPackage":
                    var packageEntity = new BaseServices<FormDrawingsPackage>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(packageEntity, c =>
                c.IsPublished
                && c.CustomerNo == packageEntity.CustomerNo
                && c.ProductNo == packageEntity.ProductNo
                , item, empNo, formType, publishTime, isPass);
                    break;
                case "DrawingsBom":
                    var bomEntity = new BaseServices<FormDrawingsBom>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(bomEntity, c =>
                        c.IsPublished
                        && c.CustomerNo == bomEntity.CustomerNo
                        && c.ProductNo == bomEntity.ProductNo,
                        item, empNo, formType, publishTime, isPass); break;
                case "DrawingsSop":
                    var sopEntity = new BaseServices<FormDrawingsSop>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(sopEntity, c =>
                c.IsPublished
                && c.CustomerNo == sopEntity.CustomerNo
                && c.DrawPartNo == sopEntity.DrawPartNo
                && c.Tag == sopEntity.Tag
                , item, empNo, formType, publishTime, isPass);
                    break;
                case "DrawingsFqc":
                    var fqcEntity = new BaseServices<FormDrawingsFqc>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(fqcEntity, c =>
                c.IsPublished
                && c.CustomerNo == fqcEntity.CustomerNo
                && c.ProductNo == fqcEntity.ProductNo
                && c.DrawPartNo == fqcEntity.DrawPartNo
                , item, empNo, formType, publishTime, isPass);
                    break;
                case "DrawingsProgram":

                    var programEntity = new BaseServices<FormDrawingsProgram>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(programEntity, c =>
                c.IsPublished
                && c.FileType == programEntity.FileType
                && c.FileName == programEntity.FileName
                && c.FileCode == programEntity.FileCode

                , item, empNo, formType, publishTime, isPass);
                    break;
                case "DrawingsExternal":
                    var externalEntity = new BaseServices<FormDrawingsExternal>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(externalEntity, c =>
                c.IsPublished
                && c.CustomerNo == externalEntity.CustomerNo
                && c.FileCode == externalEntity.FileCode
                && c.MinicutCode == externalEntity.MinicutCode
                , item, empNo, formType, publishTime, isPass);
                    break;
                case "DrawingsControlPlan":
                    var controlPlanEntity = new BaseServices<FormDrawingsControlPlan>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(controlPlanEntity, c =>
                c.IsPublished
                && c.CustomerNo == controlPlanEntity.CustomerNo
                && c.ProductNo == controlPlanEntity.ProductNo
                , item, empNo, formType, publishTime, isPass);
                    break;
                case "DrawingsFmea":
                    var fmeaEntity = new BaseServices<FormDrawingsFmea>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(fmeaEntity, c =>
                c.IsPublished
                && c.CustomerNo == fmeaEntity.CustomerNo
                && c.ProductNo == fmeaEntity.ProductNo
                , item, empNo, formType, publishTime, isPass);
                    break;

                case "DrawingsSopDewell":

                    var sopDewellEntity = new BaseServices<FormDrawingsSopDewell>().LoadEntities(c => c.FormNo == formNo).First();
                    result = PublishPara(sopDewellEntity, c =>
                c.IsPublished
                && c.CustomerNo == sopDewellEntity.CustomerNo
                && c.ProductNo == sopDewellEntity.ProductNo
                && c.Tag == sopDewellEntity.Tag
                , item, empNo, formType, publishTime, isPass);
                    break;
                default:
                    throw new Exception("没有发现卡类型");
            }


            ////发邮件
            //if (result)
            //{
            //    //   new Applying().GetFormEntityByFormNo(formNo, formType.Method);

            //    //通知签核人
            //    var modelTwo = new Model.Email();
            //    modelTwo.SdEmail += new BLL.EmailDetails().Pdm;
            //    modelTwo.Sending(new SendEmailEventArgs()
            //    {
            //        To = new Dictionary<string, string>
            //        {
            //            { "ywb@minicut.com.cn", "Sales" }, 
            //            { "cgk@minicut.com.cn", "Purchasing" },
            //            { "gcb@minicut.com.cn", "Engineering" },
            //            { "product@minicut.com.cn", "Production" },
            //            { "pzb@minicut.com.cn", "Quality" },
            //        },
            //        Subject = "文件发行通知[" + formType.FormName + "]",
            //        NickName = "all",
            //        Title = "文件发行通知[" + formType.FormName + "]",
            //        From = "PDM",
            //        Content = "<p>文件已正式发行：" + formNo + "</p>" +
            //        "<p>客户名称:six</p>" +
            //        "<p>产品编号:" + productNo + "</p>",
            //        Date = DateTime.Now,
            //        Link = "点击查看表单详细内容：<a href='" + HttpContext.Current.Request.Url.Authority + "/Default/Index/' >查图纸</a>",

            //    });
            //}

            return result;
        }

        //表单发行
        private bool PublishPara<T>(T formEntity, Func<T, bool> whereLambda, int item, string empNo, FormType formType, DateTime? publishTime, bool isPass) where T : DrawingsBase, new()
        {

            //待发行的表单
            //  T formEntity = DALFactory.ContextHelperFactory<T>.Helper.LoadEntities(c => c.FormNo == formNo).First();

            string formNo = formEntity.FormNo;


            //已发行直接返回失败
            if (formEntity.IsPublished) return false;

            //发行次数
            var publishedList = new BaseServices<T>().LoadEntities(whereLambda).ToList();

            //转换成版本
            var array = new byte[1];
            array[0] = (byte)(Convert.ToInt32(65 + publishedList.Count)); //ASCII码强制转换二进制

            //生成发行记录
            var published = new Published()
            {
                PubishedGuid = Guid.NewGuid(),
                FormNo = formEntity.FormNo,
                PublishTime = publishTime ?? DateTime.Now,
                PublishType = formType.FormId,

                CustomerNo = formEntity.CustomerNo,
                FileGroup = formEntity.FileGroup,
                ProductNo = formEntity.ProductNo,



                EmpNo = empNo,
                IsDel = false,
                IsPass = isPass,
                Identity = Guid.NewGuid(),
                PublishVer = Convert.ToString(System.Text.Encoding.ASCII.GetString(array))

            };

            //Copy文件至发行目录

            string sourcePath = System.IO.Path.GetFullPath(BaseHelper.UpPath);
            string targetPath = System.IO.Path.GetFullPath(BaseHelper.PublishedPath);
            var files = formEntity.FileGroup;

            foreach (var file in new Carlzhu.Iooin.Business.BaseModule.FilesFileGroupBll().GetFileListByGroupGuid(files))
            {
                var ff = DataFactory.Database().FindEntity<Files>(file.Md5);
                BaseHelper.CopyFile(sourcePath + ff.Md5, targetPath + ff.Md5);
            }



            //添加标识
            if (publishedList.Count > 0)
            {
                this.UpdatePublishIdentity<T>(publishedList, published.Identity);
            }



            //发行
            if (item == 0)//记录导入，不用签核
                return
                    this.Publishing<T>
                        (formEntity, published);
            return
                new Signing().Agree(formNo, item, empNo)
                &&
                this.Publishing<T>(formEntity, published);
        }

        public bool PdfView(Guid publishKey, string md5)
        {
            using (var context = BaseUtility.ContextFactory.ContextHelper)
            {
                Published published = context.Publisheds.Find(publishKey);
                published.Visit = published.Visit + 1;
                return context.SaveChanges() > 0;

            }
        }


        #region 导入


        public int OldImport(DataSet ds)
        {
            var customers = new TpaModule.TpaCustomerBll().GetList();
            var formTyps = new BaseServices<FormType>().LoadEntities(c => true);

            DataTable dt = ds.Tables[0];
            string publishType = dt.ToString();


            int success = 0;
            foreach (var row in dt.Rows)
            {
                try
                {

                    string fileName = ((DataRow)(row)).ItemArray[0].ToString();
                    string ver = ((DataRow)(row)).ItemArray[1].ToString();
                    int pageSize = int.Parse(((DataRow)(row)).ItemArray[2].ToString());
                    string filePath = @"\\192.168.0.4\Minicut\公司文件\文控\图纸电子档\勿动\" + ((DataRow)(row)).ItemArray[3] + ".pdf";

                    string reason = ((DataRow)(row)).ItemArray[4].ToString();
                    DateTime publishDate = DateTime.Parse(((DataRow)(row)).ItemArray[5].ToString());
                    const string publishEmp = "1109004";
                    bool isPass = int.Parse(((DataRow)(row)).ItemArray[7].ToString()) == 1;

                    string fileType = Path.GetExtension(filePath);


                    Guid fileGroup = Guid.NewGuid();
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    {
                        var md5 = DirFileHelper.GetMd5StringByStream(fs);

                        string targetPath = Path.GetFullPath(BaseHelper.UpPath);

                        //将文什上传至文档临时目录
                        BaseHelper.CopyFile(filePath, targetPath + md5);
                        using (var context = ContextFactory.ContextHelper)
                        {
                            //组建文件群组
                            context.FileGroups.Add(new FileGroup()
                            {
                                GroupGuid = fileGroup,
                                CreateTime = DateTime.Now,
                                CreateEmpNo = "1109001"
                            });

                            //上传文件

                            if (!context.Fileses.Any(c => c.Md5 == md5))
                                context.Fileses.Add(new Files()
                                {
                                    Md5 = md5,
                                    FileName = fileName,
                                    FileType = fileType,
                                    ContentType = "application/pdf",
                                });

                            context.FilesFileGroups.AddRange(new List<FilesFileGroup> { new FilesFileGroup() { Md5 = md5, GroupGuid = fileGroup } });
                            context.SaveChanges();
                        }


                        //组建文件关系


                    }

                    int formId = formTyps.Single(c => c.FormName.Contains(publishType)).FormId;

                    //创建表单内容
                    var form = new Form()
                    {
                        FormNo = new Applying().CreateFormNo(new object()),
                        FormId = formId,
                        CreateEmpNo = "1109001",
                        CreateTime = publishDate,
                        SignPath = null,
                        FormStatus = (int)Form.StatusEnum.签核完成,
                        IsEmergents = false,
                        CloseTime = DateTime.Now
                    };

                    //生成表单
                    new BaseServices<Form>().AddEntity(form);

                    #region insert
                    //生成表单详细页
                    switch (publishType)
                    {
                        case "BOM":
                            new BaseServices<FormDrawingsBom>().AddEntity(new FormDrawingsBom()
                            {

                                CustomerNo = customers.Single(c => c.CustomerName.Contains(((DataRow)(row)).ItemArray[8].ToString())).CustomerNo,
                                DrawPartNo = ((DataRow)(row)).ItemArray[9].ToString(),
                                ProductNo = ((DataRow)(row)).ItemArray[10].ToString(),
                                ProductName = ((DataRow)(row)).ItemArray[11].ToString(),
                                FileCode = ((DataRow)(row)).ItemArray[12].ToString(),
                                Author = ((DataRow)(row)).ItemArray[13].ToString(),

                                FormNo = form.FormNo,
                                Mark = "系统自动导入",

                                DrawVer = ver,
                                PageSize = pageSize,
                                FileGroup = fileGroup,
                                Reason = reason,
                                IsPublished = false,
                            });
                            break;
                        case "ECN":
                            new BaseServices<FormDrawingsEcn>().AddEntity(new FormDrawingsEcn()
                            {
                                CustomerNo = customers.Single(c => c.CustomerName.Contains(((DataRow)(row)).ItemArray[8].ToString())).CustomerNo,
                                ProductNo = ((DataRow)(row)).ItemArray[9].ToString(),
                                ProductName = ((DataRow)(row)).ItemArray[10].ToString(),
                                EcnNo = ((DataRow)(row)).ItemArray[11].ToString(),
                                PublishEmp = ((DataRow)(row)).ItemArray[12].ToString(),



                                FormNo = form.FormNo,
                                Mark = "系统自动导入",

                                DrawVer = ver,
                                PageSize = pageSize,
                                FileGroup = fileGroup,
                                Reason = reason,
                                IsPublished = false,
                            });
                            break;
                        case "SOP":
                            new BaseServices<FormDrawingsSop>().AddEntity(new FormDrawingsSop()
                            {
                                CustomerNo = customers.Single(c => c.CustomerName.Contains(((DataRow)(row)).ItemArray[8].ToString())).CustomerNo,
                                DrawPartNo = ((DataRow)(row)).ItemArray[9].ToString(),
                                Tag = ((DataRow)(row)).ItemArray[10].ToString(),
                                Author = ((DataRow)(row)).ItemArray[11].ToString(),

                                FormNo = form.FormNo,
                                Mark = "系统自动导入",

                                DrawVer = ver,
                                PageSize = pageSize,
                                FileGroup = fileGroup,
                                Reason = reason,
                                IsPublished = false,
                            });
                            break;
                        case "包装工艺卡":
                            new BaseServices<FormDrawingsPackage>().AddEntity(new FormDrawingsPackage()
                            {
                                CustomerNo = customers.Single(c => c.CustomerName.Contains(((DataRow)(row)).ItemArray[8].ToString())).CustomerNo,
                                ProductNo = ((DataRow)(row)).ItemArray[9].ToString(),
                                ProductName = ((DataRow)(row)).ItemArray[10].ToString(),
                                Author = ((DataRow)(row)).ItemArray[11].ToString(),

                                FormNo = form.FormNo,
                                Mark = "系统自动导入",

                                DrawVer = ver,
                                PageSize = pageSize,
                                FileGroup = fileGroup,
                                Reason = reason,
                                IsPublished = false,
                            });
                            break;
                        case "工艺流程图":
                            new BaseServices<FormDrawingsProcess>().AddEntity(new FormDrawingsProcess()
                            {

                                CustomerNo = customers.Single(c => c.CustomerName.Contains(((DataRow)(row)).ItemArray[8].ToString())).CustomerNo,
                                DrawPartNo = ((DataRow)(row)).ItemArray[9].ToString(),
                                ProductNo = ((DataRow)(row)).ItemArray[10].ToString(),
                                ProductName = ((DataRow)(row)).ItemArray[11].ToString(),
                                Drafter = ((DataRow)(row)).ItemArray[12].ToString(),



                                FormNo = form.FormNo,
                                Mark = "系统自动导入",

                                DrawVer = ver,
                                PageSize = pageSize,
                                FileGroup = fileGroup,
                                Reason = reason,
                                IsPublished = false,
                            });
                            break;
                        case "检规指导书":
                            new BaseServices<FormDrawingsFqc>().AddEntity(new FormDrawingsFqc()
                            {
                                CustomerNo = customers.Single(c => c.CustomerName.Contains(((DataRow)(row)).ItemArray[8].ToString())).CustomerNo,
                                DrawPartNo = ((DataRow)(row)).ItemArray[9].ToString(),
                                ProductNo = ((DataRow)(row)).ItemArray[10].ToString(),
                                ProductName = ((DataRow)(row)).ItemArray[11].ToString(),
                                Author = ((DataRow)(row)).ItemArray[12].ToString(),


                                FormNo = form.FormNo,
                                Mark = "系统自动导入",

                                DrawVer = ver,
                                PageSize = pageSize,
                                FileGroup = fileGroup,
                                Reason = reason,
                                IsPublished = false,
                            });
                            break;
                        case "客户图纸":
                            new BaseServices<FormDrawingsCustomer>().AddEntity(new FormDrawingsCustomer()
                            {
                                CustomerNo = customers.Single(c => c.CustomerName.Contains(((DataRow)(row)).ItemArray[8].ToString())).CustomerNo,
                                DrawPartNo = ((DataRow)(row)).ItemArray[9].ToString(),
                                ProductNo = ((DataRow)(row)).ItemArray[10].ToString(),
                                ProductName = ((DataRow)(row)).ItemArray[11].ToString(),
                                ManagerHead = ((DataRow)(row)).ItemArray[12].ToString(),

                                FormNo = form.FormNo,
                                Mark = "系统自动导入",

                                DrawVer = ver,
                                PageSize = pageSize,
                                FileGroup = fileGroup,
                                Reason = reason,
                                IsPublished = false,
                            });
                            break;
                        case "内部图纸":
                            new BaseServices<FormDrawingsInside>().AddEntity(new FormDrawingsInside()
                            {
                                CustomerNo = customers.Single(c => c.CustomerName.Contains(((DataRow)(row)).ItemArray[8].ToString())).CustomerNo,
                                DrawPartNo = ((DataRow)(row)).ItemArray[9].ToString(),
                                ProductNo = ((DataRow)(row)).ItemArray[10].ToString(),
                                ProductName = ((DataRow)(row)).ItemArray[11].ToString(),
                                Drafter = ((DataRow)(row)).ItemArray[12].ToString(),



                                FormNo = form.FormNo,
                                Mark = "系统自动导入",

                                DrawVer = ver,
                                PageSize = pageSize,
                                FileGroup = fileGroup,
                                Reason = reason,
                                IsPublished = false,
                            });
                            break;
                        case "内部型材图":
                            new BaseServices<FormDrawingsProfile>().AddEntity(new FormDrawingsProfile()
                            {
                                CustomerNo = customers.Single(c => c.CustomerName.Contains(((DataRow)(row)).ItemArray[8].ToString())).CustomerNo,
                                ProfileNo = ((DataRow)(row)).ItemArray[9].ToString(),
                                ProductNo = ((DataRow)(row)).ItemArray[10].ToString(),
                                ProductName = ((DataRow)(row)).ItemArray[11].ToString(),
                                Drafter = ((DataRow)(row)).ItemArray[12].ToString(),



                                FormNo = form.FormNo,
                                Mark = "系统自动导入",

                                DrawVer = ver,
                                PageSize = pageSize,
                                FileGroup = fileGroup,
                                Reason = reason,
                                IsPublished = false,
                            });
                            break;
                        case "外来文件":
                            new BaseServices<FormDrawingsExternal>().AddEntity(new FormDrawingsExternal()
                            {

                                CustomerNo = customers.Single(c => c.CustomerName.Contains(((DataRow)(row)).ItemArray[8].ToString())).CustomerNo,
                                FileName = ((DataRow)(row)).ItemArray[9].ToString(),
                                FileCode = ((DataRow)(row)).ItemArray[10].ToString(),
                                MinicutCode = ((DataRow)(row)).ItemArray[11].ToString(),
                                ReciveEmpNo = ((DataRow)(row)).ItemArray[12].ToString(),



                                FormNo = form.FormNo,
                                Mark = "系统自动导入",

                                DrawVer = ver,
                                PageSize = pageSize,
                                FileGroup = fileGroup,
                                Reason = reason,
                                IsPublished = false,
                            });
                            break;

                        case "控制计划":
                            new BaseServices<FormDrawingsControlPlan>().AddEntity(new FormDrawingsControlPlan()
                            {

                                CustomerNo = customers.Single(c => c.CustomerName.Contains(((DataRow)(row)).ItemArray[8].ToString())).CustomerNo,
                                ProductNo = ((DataRow)(row)).ItemArray[9].ToString(),
                                Author = ((DataRow)(row)).ItemArray[10].ToString(),



                                FormNo = form.FormNo,
                                Mark = "系统自动导入",

                                DrawVer = ver,
                                PageSize = pageSize,
                                FileGroup = fileGroup,
                                Reason = reason,
                                IsPublished = false,
                            });
                            break;
                    }
                    #endregion



                    //发行
                    if (this.Published(form.FormNo, 0, publishEmp, publishDate, isPass))
                        success++;

                }
                catch (Exception exception)
                {
                    return Link.ErrorBy(new Exception($"成功导入{success}:Error:{exception.Message}"), this.GetType());
                }
            }

            return success;
        }

        public bool DrawReplace(string formNo, string empNo)
        {
            string newMd5 = this.DrawReplace2(formNo, empNo);

            if (string.IsNullOrEmpty(newMd5)) return false;
            string sourcePath = System.IO.Path.GetFullPath(BaseHelper.UpPath);
            string targetPath = System.IO.Path.GetFullPath(BaseHelper.PublishedPath);
            return BaseHelper.CopyFile(sourcePath + newMd5, targetPath + newMd5);
        }

        #endregion

        #region Apparatus




        #endregion




        #region q2
        bool UpdatePublishIdentity<T>(List<T> entitys, Guid identity) where T : DrawingsBase
        {
            var oldIdentity = entitys.First().Identity;
            try
            {
                using (var carlzhu = BaseUtility.ContextFactory.ContextHelper)
                {
                    //更新表单标识
                    foreach (var entity in entitys)
                    {
                        carlzhu.Set<T>().Single<T>(c => c.FormNo == entity.FormNo).Identity = identity;
                    }
                    //更新发行标识
                    carlzhu.Publisheds.Where(c => c.Identity == oldIdentity).ToList().ForEach(d =>
                    {
                        d.Identity = identity;
                        d.IsDel = true;
                    });

                    return carlzhu.SaveChanges() > 0;

                }
            }
            catch
            {
                return false;
            }
        }


        string DrawReplace2(string formNo, string empNo)
        {
            using (var context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    //取得替换申请表单
                    FormReplaceDrawings formReplaceDrawings = context.FormReplaceDrawingses.Single(c => c.FormNo == formNo);

                    //取得发行文件
                    var publish = context.Publisheds.Single(c => c.FormNo == formReplaceDrawings.OldFormNo && c.PubishedGuid == formReplaceDrawings.PublishedCode);

                    //取得新文件组
                    FilesFileGroup newGroup = context.FilesFileGroups.FirstOrDefault(c => c.GroupGuid.Equals(formReplaceDrawings.FileGroup));

                    //旧文件组
                    FilesFileGroup oldGroup = context.FilesFileGroups.FirstOrDefault(c => c.GroupGuid == publish.FileGroup && c.Md5 == formReplaceDrawings.OldMd5);

                    if (oldGroup == null || newGroup == null) return null;

                    //更新旧文件组中的文件
                    oldGroup.Md5 = newGroup.Md5;
                    //添加替换记录
                    context.FileReplaceRecordses.Add(new FileReplaceRecords()
                    {
                        FormNo = formReplaceDrawings.FormNo,
                        OldFileGroupGuid = publish.FileGroup,
                        OldMd5 = oldGroup.Md5,
                        NewFileGroupGuid = newGroup.GroupGuid,
                        NewMd5 = newGroup.Md5,
                        CreateDate = DateTime.Now,
                        EmpNo = empNo
                    });
                    return (context.SaveChanges() > 0) ? newGroup.Md5 : null;

                }
                catch (Exception)
                {
                    return null;
                }
            }
        }



        public bool Publishing<T>(DrawingsBase entity, Published published) where T : DrawingsBase
        {
            try
            {
                using (var carlzhu = BaseUtility.ContextFactory.ContextHelper)
                {
                    //将图纸状态改为已发行，并添加标识
                    var model = carlzhu.Set<T>().Single<T>(c => c.FormNo == entity.FormNo);
                    model.IsPublished = true;
                    model.Identity = published.Identity;
                    carlzhu.Entry(model).State = EntityState.Modified;
                    //published.CustomerNo = entity.CustomerNo;

                    //增加发行记录
                    carlzhu.Publisheds.Add(published);

                    return carlzhu.SaveChanges() > 0;
                }
            }
            catch (Exception exception)
            {
                DbEntityValidationException ex = exception as DbEntityValidationException;

                if (ex != null)
                {
                    var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                    string exceptionMessage = string.Concat(ex.Message, "errors are: ", string.Join("; ", entityError));
                    Console.Write(exceptionMessage);
                }

                return false;
            }

        }


        #endregion
    }
}
