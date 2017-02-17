using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Business.FormModule
{
    public class Forms
    {
        // readonly CarlzhuContext _context = BaseUtility.ContextFactory.ContextHelper;

        #region Tracking
        /// <summary>
        /// 撤消表单
        /// </summary>
        /// <param name="formNo">表单号</param>
        /// <param name="empNo">表单创建人</param>
        /// <returns></returns>
        public bool Cancel(string formNo, string empNo)
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    var forms = _context.Forms.Single(c => c.FormNo == formNo && c.CreateEmpNo == empNo);

                    _context.Entry(forms).State = EntityState.Modified;
                    forms.FormStatus = (int)Carlzhu.Iooin.Entity.FORM.Form.StatusEnum.已撤消;
                    forms.CloseTime = DateTime.Now;
                    return _context.SaveChanges() > 0;
                }
                catch (Exception)
                {
                    return false;

                }
            }
        }


        /// <summary>
        /// 软删除表单
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool Delete(string formNo, string empNo)
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    var model = _context.Forms.Find(formNo);
                    if (model.CreateEmpNo != empNo && model.CloseTime != null) return false;

                    model.IsDel = true;
                    return _context.SaveChanges() > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }


        #endregion

        #region Person


        #endregion


        #region Applying


        /// <summary>
        /// 更新用户名单
        /// </summary>
        /// <param name="newemp">新的名单</param>
        /// <param name="guid">名单对应索引</param>
        /// <param name="empNo">更新人</param>
        /// <returns></returns>
        public bool UpdateInvolvingUser(List<string> newemp, Guid guid, string empNo)
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                List<string> oldemp = _context.FormInvolvingUsers.Where(c => c.Guid == guid).Select(c => c.EmpNo).ToList();

                //整合新的集合
                var addList = new List<string>();
                var delList = new List<string>();

                addList.AddRange(newemp.Where(p => !oldemp.Contains(p)).ToList());
                delList.AddRange(oldemp.Where(p => !newemp.Contains(p)).ToList());

                //删除原来的
                _context.FormInvolvingUsers.RemoveRange(_context.FormInvolvingUsers.Where(c => delList.Contains(c.EmpNo)));

                //添加新增的
                List<FormInvolvingUser> wattingAdd = new List<FormInvolvingUser>();
                addList.ForEach(c => wattingAdd.Add(new FormInvolvingUser() { EmpNo = c, Guid = guid, UpdateEmp = empNo }));


                _context.FormInvolvingUsers.AddRange(wattingAdd);

                return _context.SaveChanges() > 0;
            }

        }




        ///// <summary>
        ///// 生成表单号
        ///// </summary>
        ///// <param name="o"></param>
        ///// <returns></returns>
        //public string TestCreateFormNo(object o)
        //{
        //    using (_context)
        //    {

        //        lock (o)
        //        {
        //            string now = DateTime.Now.ToString("yyyyMMdd");
        //            string formNo;







        //            try
        //            {
        //                var model = _context.Forms.Where(c => c.FormNo.Contains(now))
        //                    .OrderByDescending(c => c.CreateTime)
        //                    .First();

        //                formNo =
        //              string.Format("{0}{1}", now, model == null ? "1".PadLeft(5, '0') :
        //              (int.Parse(model.FormNo.Substring(9)) + 1).ToString(CultureInfo.InvariantCulture).PadLeft(5, '0'));
        //            }
        //            catch (Exception exception)
        //            {
        //                formNo = string.Format("{0}00001", now);
        //            }

        //            return formNo;

        //        }

        //    }
        //}


        public string CreateFormNo(object o)
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                string now = DateTime.Now.ToString("yyyyMMdd");
                string formNo;
                lock (o)
                {
                    var forms = _context.Forms.Where(c => c.FormNo.Contains(now)).ToList();
                    int f = 0;
                    if (forms.Any())
                    {
                        forms.ForEach(c =>
                        {
                            var max = int.Parse(c.FormNo.Substring(9));
                            if (max > f) f = max;
                        });

                        formNo = $"{now}{(f + 1).ToString(CultureInfo.InvariantCulture).PadLeft(5, '0')}";
                    }
                    else
                    {
                        formNo = $"{now}00001";
                    }
                }
                return formNo;
            }
        }

        #region 修改表单

        /// <summary>
        /// 单记录修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public bool FormEdit<T>(T entity, string formNo) where T : F, new()
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                entity.RowId = _context.Set<T>().Single<T>(c => c.FormNo == formNo).RowId;
                return new BaseRepository<T>().UpdateEntity(entity);
            }
        }


        /// <summary>
        /// 多记录同时修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newList"></param>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public bool FormEdit<T>(List<T> newList, string formNo) where T : F
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    var oldList = _context.Set<T>().Where<T>(c => string.Equals(c.FormNo, formNo)).ToList();

                    foreach (var list in oldList)
                    {
                        _context.Set<T>().Attach(list);
                        _context.Entry<T>(list).State = EntityState.Deleted;
                    }
                    foreach (var list in newList)
                    {
                        list.FormNo = formNo;
                        _context.Entry<T>(list).State = EntityState.Added;
                    }
                    return _context.SaveChanges() > 0;
                }
                catch (Exception)
                {

                    return false;
                }
                //旧列表



                //整合新的集合
                //var addList = new List<T>();
                //var delList = new List<T>();
                //var updateList = new List<T>();
                //addList.AddRange(newList.Where(p => !oldList.Contains(p)).ToList());
                //delList.AddRange(oldList.Where(p => !newList.Contains(p)).ToList());
                //updateList.AddRange(newList.Where(oldList.Contains).ToList());



            }
        }



        #endregion

        /// <summary>
        /// 发签表单
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <param name="firstEmpNo"></param>
        /// <returns></returns>
        public bool Send(string formNo, string empNo, out string firstEmpNo)
        {
            const int empNoLength = HrmsConfig.NoLength;

            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    var form = _context.Forms.Single(c => c.FormNo == formNo && c.CreateEmpNo == empNo);
                    
                    firstEmpNo = form.SignPath.Substring(0, empNoLength);

                    //如果已经送签则送签失改
                    if (form.FormStatus != (int)Form.StatusEnum.未送出) return false;

                    _context.FormSigns.Add(new FormSign
                    {
                        FormNo = formNo,
                        SignEmpNo = firstEmpNo,
                        SourceType = (int)FormSign.SourceTypeEnum.Auto,
                        CreateTime = DateTime.Now,
                        Grade = 0,
                        Tag = 0,
                        IsDel = false,
                    });

                    form.FormStatus = (int)Form.StatusEnum.签核中;//改变签核状态为签核中
                    form.SignPath = form.SignPath.Substring(empNoLength + 1);//删除第一个签核人,工号加","


                    _context.Entry(form).State = EntityState.Modified;

                    return _context.SaveChanges() == 2;

                }
                catch (Exception)
                {
                    firstEmpNo = "";
                    return false;
                }
            }
        }

        #endregion


        #region Signing

        /// <summary>
        /// 取得待签记录
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public List<FormSign> GetSignDataList(string empNo)
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    var formSigns = new List<FormSign>();

                    //本人代签记录
                    var proxyList = _context.FormProxies.Where(
                           c => c.StarTime < DateTime.Now && c.EndTime > DateTime.Now && c.EmpNo == empNo).ToList();

                    //根据代签找出所有要签记录
                    proxyList.ForEach(c =>
                    {
                        formSigns.AddRange(_context.FormSigns.Include("Form").Include("Form.FormType").Include("Form.BaseEmployee").Where(d => d.SignEmpNo == c.SourceEmpNo && d.Form.FormType.FormId == c.FormId && !d.IsDel && d.SignResult == (int)FormSign.SignResultEnum.Watting)); ;
                    });


                    //将代签记录一起放入记录集中
                    formSigns.AddRange(_context.FormSigns.Include("Form").Include("Form.FormType").Include("Form.BaseEmployee").Where(c => c.SignEmpNo == empNo && !c.IsDel && c.SignResult == (int)FormSign.SignResultEnum.Watting).ToList());

                    return formSigns.Where(d => d.Form.FormStatus == (int)Carlzhu.Iooin.Entity.FORM.Form.StatusEnum.签核中).ToList();
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    return null;
                }
            }
        }


        /// <summary>
        /// 更新签核记录标记
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool IsUpdateTags(int item, int tag)
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    FormSign formSign = _context.FormSigns.Find(item);
                    formSign.Tag = tag;

                    _context.Entry(formSign).State = EntityState.Modified;

                    return _context.SaveChanges() > 0;

                }
                catch
                {
                    return false;
                }
            }


        }

        /// <summary>
        /// 转签
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="redirectempno"></param>
        /// <param name="reason"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool IsRedirectSign(string formNo, int item, string redirectempno, string reason, string empNo, out string msg)
        {
            msg = "转签失败!";
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                //确认表单是否可以转签
                if (!_context.Forms.Find(formNo).FormType.IsRedirect)
                {
                    msg = "表单不允许转签!";
                    return false;
                }
                _context.FormSigns.Add(new FormSign()
                {
                    FormNo = formNo,
                    SignEmpNo = redirectempno,
                    SourceType = (int)FormSign.SourceTypeEnum.Redirect,
                    SourceEmpNo = empNo,
                    SourceReason = reason,
                    CreateTime = DateTime.Now,
                    Grade = 0,
                    Tag = 0,
                    IsDel = false,
                });

                //取出原有记录并软删除
                var signModel = _context.FormSigns.Single(c => c.RowId == item && c.FormNo == formNo);
                signModel.IsDel = true;
                signModel.SignResult = (int)FormSign.SignResultEnum.Rdirect;
                signModel.SignTime = DateTime.Now;

                _context.Entry(signModel).State = EntityState.Modified;


                if (_context.SaveChanges() < 2) return false;

                msg = "转签成功!";
                return true;
            }
        }

        /// <summary>
        /// 加签前
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="addempno"></param>
        /// <param name="reason"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool IsAddSignBefore(string formNo, int item, string addempno, string reason, string empNo)
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    //记录已失效
                    if (_context.FormSigns.Find(item).IsDel) return false;


                    //新增新的
                    _context.FormSigns.Add(new FormSign()
                    {
                        FormNo = formNo,
                        SignEmpNo = addempno,
                        SourceType = (int)FormSign.SourceTypeEnum.AddBefore,
                        SourceEmpNo = empNo,
                        SourceReason = reason,
                        CreateTime = DateTime.Now,
                        Grade = 0,
                        Tag = 0,
                        IsDel = false,
                    });

                    //取出原有记录并软删除
                    var signModel = _context.FormSigns.Single(c => c.RowId == item && c.FormNo == formNo);
                    signModel.IsDel = true;
                    signModel.SignResult = (int)FormSign.SignResultEnum.Add;
                    signModel.SignTime = DateTime.Now;
                    _context.Entry(signModel).State = EntityState.Modified;


                    //将现在的添加到签核路径中
                    var formModel = _context.Forms.Find(formNo);
                    formModel.SignPath = empNo + "," + formModel.SignPath;
                    _context.Entry(formModel).State = EntityState.Modified;


                    return _context.SaveChanges() == 3;
                }
                catch
                {
                    return false;

                }
            }
        }

        /// <summary>
        ///平行签
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="addempno"></param>
        /// <param name="reason"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool IsAddSignParallel(string formNo, int item, string addempno, string reason, string empNo)
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                //记录已失效
                if (_context.FormSigns.Find(item).IsDel) return false;

                try
                {
                    _context.FormSigns.Add(new FormSign()
                    {
                        FormNo = formNo,
                        SignEmpNo = addempno,
                        SourceType = (int)FormSign.SourceTypeEnum.AddParallel,
                        SourceEmpNo = empNo,
                        SourceReason = reason,
                        CreateTime = DateTime.Now,
                        Grade = 0,
                        Tag = 0,
                        IsDel = false,
                    });
                    return _context.SaveChanges() == 1;
                }
                catch
                {
                    return false;

                }
            }
        }

        /// <summary>
        /// 加签后
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="addempno"></param>
        /// <param name="reason"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool IsAddSignAfter(string formNo, int item, string addempno, string reason, string empNo)
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                //记录已失效
                if (_context.FormSigns.Find(item).IsDel) return false;

                try
                {
                    if (_context.FormSigns.Single(c => c.RowId == item && c.FormNo == formNo) != null)
                    {
                        //将现在的添加到签核路径中
                        var formModel = _context.Forms.Find(formNo);
                        formModel.SignPath = "@" + addempno + "," + formModel.SignPath;//@表示加签在后记录
                        formModel.Reason = reason;
                        formModel.SourceEmpNo = empNo;
                        _context.Entry(formModel).State = EntityState.Modified;
                        return _context.SaveChanges() == 1;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }

            }

        }


        /// <summary>
        /// 根据表单号生成签核记录
        /// </summary>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public List<FormSign> GetSignRecoredsByFormNo(string formNo)
        {
            var records = new List<FormSign>();




            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    //添加已经生成的记录
                    records.AddRange(_context.FormSigns.Include("BaseEmployee").Include("BaseEmployee.BasePost").Where(c => c.FormNo == formNo && !c.IsDel).ToList());

                    //添加未生成的
                    var formModel = _context.Forms.Find(formNo);
                    var signPath = formModel.SignPath.Split(',').ToList();
                    signPath.RemoveAll(string.IsNullOrEmpty);
                    signPath.ForEach(c =>
                    {
                        if (!c.Contains("@"))
                        {
                            records.Add(new FormSign()
                            {
                                SignEmpNo = c,
                            });
                        }
                        else
                        {
                            records.Add(new FormSign()
                            {
                                SignEmpNo = c.Substring(1),
                                SourceType = (int)FormSign.SourceTypeEnum.AddAfter,
                                SourceReason = formModel.Reason,
                                SourceEmpNo = formModel.SourceEmpNo,
                            });
                        }
                    });
                }
                catch
                {
                    return null;
                }

            }

            


            return records;
        }


        /// <summary>
        /// 根据表单号和签核序号更新签核意见
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public bool IsUpdateSignMarkByItemAndFormNo(string formNo, int item, string mark)
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    var signModel = _context.FormSigns.Single(c => c.FormNo == formNo && c.RowId == item);
                    signModel.SignMark = mark;
                    _context.Entry(signModel).State = EntityState.Modified;
                    return _context.SaveChanges() == 1;
                }
                catch
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// 根据表单号取出上位签核人
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public BaseEmployee UpSignEmployee(string formNo, int item)
        {
            using (var _context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    if (_context.FormSigns.Single(c => c.FormNo == formNo && c.RowId == item) == null) return null;
                    var signs = _context.FormSigns.Where(c => c.FormNo == formNo && !c.IsDel).OrderByDescending(c => c.RowId).ToList();
                    return signs.Count() > 1 ? _context.BaseEmployees.Find(signs[1].ActualSignEmpNo) : null;
                }
                catch
                {
                    return null;
                }
            }
        }


        /// <summary>
        ///  //0：失败 //1：成功 //2签核完成
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="empNo"></param>
        /// <param name="nextEmpNo"></param>
        /// <param name="sourceEmpNo"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public bool Agree(string formNo, int item, string empNo, out string nextEmpNo, out string sourceEmpNo, Func<string, string, int, bool> func)
        {
            nextEmpNo = null;
            sourceEmpNo = null;

            using (var context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {

                    //如果被签记录号为0，则手动查找记录号
                    if (item == 0) item = context.FormSigns.Single(c => c.FormNo == formNo && c.SignResult == (int)FormSign.SignResultEnum.Watting).RowId;

                    //确认记录是否存在
                    if (context.FormSigns.Single(c => c.FormNo == formNo && c.RowId == item && c.SignResult == (int)FormSign.SignResultEnum.Watting) == null) return false;

                    //将记录更改为签核完成
                    var signs = context.FormSigns.Where(c => c.FormNo == formNo && c.SignResult == (int)FormSign.SignResultEnum.Watting).ToList();

                    signs.ForEach(d => { d.SignResult = (int)FormSign.SignResultEnum.Agree; d.ActualSignEmpNo = empNo; d.SignTime = DateTime.Now; });

                    //查找表单实体
                    var formModel = context.Forms.Find(formNo);
                    //查找还未产生的签核人
                    var noCheck = formModel.SignPath.Split(',').ToList();
                    noCheck.RemoveAll(string.IsNullOrEmpty);

                    //还未产生的签核人
                    if (noCheck.Count > 0)
                    {
                        FormSign sign = (noCheck.First().Substring(0, 1) == "@")
                            ? new FormSign()
                            {
                                FormNo = formNo,
                                SignEmpNo = noCheck.First().Substring(1),
                                SourceType = (int)FormSign.SourceTypeEnum.AddAfter,
                                SourceEmpNo = formModel.SourceEmpNo,
                                SourceReason = formModel.Reason,
                                CreateTime = DateTime.Now,
                                Grade = 0,
                                Tag = 0,
                                IsDel = false,
                            }
                            : new FormSign() { FormNo = formNo, SignEmpNo = noCheck.First(), SourceType = (int)FormSign.SourceTypeEnum.Auto, CreateTime = DateTime.Now, Grade = 0, Tag = 0, IsDel = false, };
                        context.FormSigns.Add(sign);

                        //下位签核人
                        nextEmpNo = sign.SignEmpNo;

                        //重组未签核顺序
                        noCheck.Remove(noCheck.First());
                        var sb = new StringBuilder();
                        noCheck.ForEach(k => sb.Append(k + ","));

                        formModel.SignPath = sb.ToString();
                        formModel.SourceEmpNo = null;
                        formModel.Reason = null;

                        context.Entry(formModel).State = EntityState.Modified;
                        context.SaveChanges();

                        sourceEmpNo = sign.SourceEmpNo;

                        return true;
                    }
                    //没有签核人啦，将表单状态改为签核完成
                    //结案表单

                    if (!func(formNo, empNo, item)) return false;
                    //签核完成
                    formModel.FormStatus = (int)Form.StatusEnum.签核完成;
                    formModel.CloseTime = DateTime.Now;
                    context.SaveChanges();
                    return true;
                }
                catch (Exception exception)
                {
                    Console.Write(exception.Message);
                    return false;
                }
            }
        }


        //0否决失败,1否决成功,
        public bool Reject(string formNo, int item, string empNo)
        {
            using (var context = BaseUtility.ContextFactory.ContextHelper)
            {
                try
                {
                    if (
                        context.FormSigns.Single(c => c.RowId == item && c.FormNo == formNo && c.SignResult == (int)FormSign.SignResultEnum.Watting) != null)
                    {

                        var signModels = context.FormSigns.Where(c => c.FormNo == formNo && c.SignResult == (int)FormSign.SignResultEnum.Watting).ToList();
                        signModels.ForEach(c =>
                        {
                            c.SignResult = (int)FormSign.SignResultEnum.Reject;
                            c.SignTime = DateTime.Now;
                            c.ActualSignEmpNo = empNo;
                        });

                        var formModel = context.Forms.Find(formNo);
                        formModel.CloseTime = DateTime.Now;
                        formModel.FormStatus = (int)Form.StatusEnum.已否决;

                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    return false;
                }
            }
        }
        #endregion
    }
}
