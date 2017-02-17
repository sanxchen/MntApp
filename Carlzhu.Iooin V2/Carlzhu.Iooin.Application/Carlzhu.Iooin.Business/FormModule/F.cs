using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using Carlzhu.Iooin.Business.BaseUtility;

using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.Repository;


namespace Carlzhu.Iooin.Business.FormModule
{
    public class F<T> where T : F, new()
    {

        readonly BaseServices<Form> _formService = new BaseServices<Form>();
        readonly BaseServices<T> _service = new BaseServices<T>();


        //查询实体
        public List<T> LoadFormEntity(string formNo)
        {
            return new BaseServices<T>().LoadEntities(c => c.FormNo == formNo).ToList();
        }


        /// <summary>
        /// 根据表单烦型反射实体类型
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        public Type ReflectionByFormType(FormType formType)
        {
            foreach (var an in Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(an => an.Name.Equals("Carlzhu.Iooin.Entity")))
            {
                foreach (var type in Assembly.Load(an).GetTypes().Where(type => type.FullName.StartsWith("Carlzhu.Iooin.Entity.FORM.f") && type.FullName.EndsWith(formType.Method)))
                {
                    return type;

                }
            }
            
            return Link.ErrorBy(new Exception("表单需加载类型还没在创建，请联系管理员！！！"), this.GetType());
        }


        /// <summary>
        /// 单实体表单保存
        /// </summary>
        /// <param name="t">表单内容实体</param>
        /// <param name="form">表单实体</param>
        /// <returns></returns>
        public bool SaveData(T t, Form form)
        {

            CarlzhuContext carlzhuContext = ContextFactory.ContextHelper;



            using (carlzhuContext)
            {
                try
                {
                    t.FormNo = form.FormNo;
                    carlzhuContext.Forms.Add(form);
                    carlzhuContext.Entry<T>(t).State = EntityState.Added;
                    return carlzhuContext.SaveChanges() >= 2;
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
                    Link.ErrorBy(exception, this.GetType());
                    Console.Write(exception);
                    return false;
                }
            }


        }


        /// <summary>
        /// list表单保存
        /// </summary>
        /// <param name="ts">表单内容实体集合</param>
        /// <param name="form">表单实体</param>
        /// <returns></returns>
        //public bool SaveData(List<T> ts, Form form)
        //{
        //    //创建表单，再新增实体
        //    ts.First().FormCreate += Save;
        //    ts.First().FormCreate += HelloTest;
        //    bool result = true;
        //    if (!ts.First().CreateForm(new FormCreateEventArgs { Form = form, })) return false;
        //    try
        //    {

        //        foreach (var t in ts)
        //        {
        //            t.FormNo = form.FormNo;
        //            if (_service.AddEntity(t) != null) continue;
        //            result = false;
        //            break;
        //        }

        //    }
        //    catch (Exception) { result = false; }
        //    finally
        //    {
        //        if (!result) _formService.DeleteEntity(form);
        //    }
        //    //添加实体
        //    return result;
        //}




        public bool SaveData(List<T> ts, Form form)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            int i = 0;

            //添加主表单
            i += database.Insert(form, isOpenTrans);

            //清加辅表单

            foreach (var t in ts)
            {
                t.FormNo = form.FormNo;
                i += database.Insert(t, isOpenTrans);
            }
            if (i >= 2)
            {
                database.Commit();

                //FormEvent<T> formEvent = new FormEvent<T>();
                //formEvent.FormCreateEventHandler += formEvent.FormEvent_FormCreate;
                //formEvent.OnFormCreate(ManageProvider.Provider.Current().UserId, form, ts.FirstOrDefault());

                return true;
            }

            database.Rollback();
            return false;
        }




        /// <summary>
        /// 生成表单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool Save(object sender, FormCreateEventArgs e)
        {
            return _formService.AddEntity(e.Form) != null;
        }

        public bool HelloTest(object sender, FormCreateEventArgs e)
        {
            return true;
        }

        /// <summary>
        /// 修改单一表单实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public bool EditForm(T entity, string formNo)
        {
            //前台没有传rowid
            return new Forms().FormEdit<T>(entity, formNo);
        }

        /// <summary>
        /// 修改多表单实体
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public bool EditForm(List<T> ts, string formNo)
        {
            return new Forms().FormEdit<T>(ts, formNo);
        }



        public bool DeleteForm(T t, Form form)
        {
            while (true)
            {
                if (_service.DeleteEntity(t) && _formService.DeleteEntity(form)) break;
            }
            return true;

        }
    }
}
