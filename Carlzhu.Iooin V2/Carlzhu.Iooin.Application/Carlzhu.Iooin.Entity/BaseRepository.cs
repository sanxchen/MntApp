using System;
using System.Data.Entity.Validation;
using System.Linq;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.FORM.f;
using EntityState = System.Data.Entity.EntityState;

namespace Carlzhu.Iooin.Entity
{
    public class BaseRepository<T> where T : class, new()
    {


        public T AddEntity(T entity)
        {
            try
            {

                CarlzhuContext.CzContext.Entry<T>(entity).State = EntityState.Added;
                CarlzhuContext.CzContext.SaveChanges();
                return entity;

            }
            catch (Exception httpException)
            {

                DbEntityValidationException ex = httpException as DbEntityValidationException;

                if (ex != null)
                {
                    var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                    string exceptionMessage = string.Concat(ex.Message, "errors are: ", string.Join("; ", entityError));
                    Console.Write(exceptionMessage);
                }

                return null;
            }
        }

        public bool UpdateEntity(T entity)
        {
            try
            {
                CarlzhuContext.CzContext.Set<T>().Attach(entity);
                CarlzhuContext.CzContext.Entry<T>(entity).State = EntityState.Modified;
                return CarlzhuContext.CzContext.SaveChanges() > 0;
            }
            catch (Exception httpException)
            {

                DbEntityValidationException ex = httpException as DbEntityValidationException;

                if (ex != null)
                {
                    var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                    string exceptionMessage = string.Concat(ex.Message, "errors are: ", string.Join("; ", entityError));
                    Console.Write(exceptionMessage);
                }

                return false;
            }

        }

        public bool DeleteEntity(T entity)
        {
            CarlzhuContext.CzContext.Set<T>().Attach(entity);
            CarlzhuContext.CzContext.Entry<T>(entity).State = EntityState.Deleted;
            return CarlzhuContext.CzContext.SaveChanges() > 0;
        }

        public IQueryable<T> LoadEntities(Func<T, bool> lambda)
        {
            return CarlzhuContext.CzContext.Set<T>().Where<T>(lambda).AsQueryable();
        }

        public IQueryable<T> LoadPageEntities<S>(int pageIndex, int pageSize, out int total, Func<T, bool> whereLambda, bool isAsc,
            Func<T, S> orderByLambda, Func<T, bool> exp2)
        {

            var temp = this.LoadEntities(whereLambda).ToList().Where(exp2).AsQueryable();

            total = temp.Count(); //得到总的条数

            //排序,获取当前页的数据

            if (isAsc)
            {
                temp = temp.OrderBy<T, S>(orderByLambda)
                     .Skip<T>(pageSize * (pageIndex - 1)) //越过多少条
                     .Take<T>(pageSize).AsQueryable(); //取出多少条
            }

            else
            {
                temp = temp.OrderByDescending<T, S>(orderByLambda)
                    .Skip<T>(pageSize * (pageIndex - 1)) //越过多少条
                    .Take<T>(pageSize).AsQueryable(); //取出多少条
            }

            return temp.AsQueryable();
        }


    }



    /// <summary>
    /// 表单公用查询方法，主要用于通用查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FormDatabase<T> where T : F
    {
        //等到这个泛型类型 设定泛型类型的泛型参数
        //Type tType = typeof(DAL.FormDatabase<>).MakeGenericType(type);

        //得到这个类型的无参构造函数 然后调用 得到这个类型的实例
        //var f = tType.GetConstructor(Type.EmptyTypes).Invoke(null);

        //得到这个类型的LoadEntity方法
        //return tType.GetMethod("LoadFormEntity").Invoke(f, new object[] { formNo, formType.IsModel });

        public dynamic LoadFormEntity(string formNo, bool isModel)
        {
            if (isModel)
                return CarlzhuContext.CzContext.Set<T>().Where<T>(c => c.FormNo == formNo).AsQueryable().First();
            return CarlzhuContext.CzContext.Set<T>().Where<T>(c => c.FormNo == formNo).AsQueryable().ToList();
        }
    }
}
