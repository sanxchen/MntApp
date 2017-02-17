using System;
using System.Collections.Generic;
using System.Linq;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Entity.FORM.f.draw;
using Carlzhu.Iooin.Entity.QUALITY;
using Carlzhu.Iooin.Entity.TPA;

namespace Carlzhu.Iooin.Business.BehandModule
{
    public class Home
    {


        public List<Published> HomeSearch<T>(DateTime startTime, DateTime endTime, string[] keys) where T : DrawingsBase
        {
            List<T> entityList;
            List<Published> publisheds = this.GetCenterSearchData(startTime, endTime, out entityList);

            var key = keys.ToList();
            key.RemoveAll(string.IsNullOrEmpty);
            var customers = new TpaModule.TpaCustomerBll().GetList();
            entityList = key.Aggregate(entityList, (current, s) => Selecting(current, s, customers));


            List<Published> newPublisheds = entityList.Select(entity => publisheds.First(c => c.FormNo == entity.FormNo)).ToList();


            return newPublisheds.OrderByDescending(k => k.Identity).ThenBy(m => m.PublishTime).ToList();



        }
        
        private static List<T> Selecting<T>(IEnumerable<T> entityList, string key, List<TpaCustomer> customers) where T : class
        {
            var newEntity = new List<T>();

            foreach (var entity in entityList)
            {
                var properties = (typeof(T)).GetProperties();

                var selectResult = false;
                foreach (var propertyInfo in properties.Where(propertInfo => propertInfo.ToString().StartsWith("System.")))
                {
                    try
                    {
                        var filedValue = propertyInfo.Name == "CustomerNo" ?
                                (customers.Single(c => c.CustomerNo == typeof(T).GetProperty(propertyInfo.Name).GetValue(entity, null).ToString()).CustomerName)
                                : typeof(T).GetProperty(propertyInfo.Name).GetValue(entity, null).ToString();

                        if (filedValue.ToLower().Contains(key.ToLower())) selectResult = true;
                    }

                    catch (Exception exception)
                    {
                        //无意义
                        string msg = exception.Message;
                    }
                    if (selectResult) break;
                }
                //添加到结果序列中
                if (selectResult) newEntity.Add(entity);
            }
            return newEntity;
        }



        List<Published> GetCenterSearchData<T>(DateTime startTime, DateTime endTime, out List<T> entityList)
         where T : DrawingsBase
        {

            using (var context = ContextFactory.ContextHelper)
            {
                var result = (context.Publisheds.Include("Employee").Include("FormType").Include("Form").Where(c => c.PublishTime > startTime && c.PublishTime < endTime)
                    .Join(
                    context.Set<T>(), publish => publish.FormNo, dEntity => dEntity.FormNo, (publish, dEntity)
                        => new { publish, dEntity, })).ToList();


                entityList = new List<T>();
                var publisheds = new List<Published>();

                foreach (var item in result)
                {
                    entityList.Add(item.dEntity);
                    publisheds.Add(item.publish);
                }
                //强制性加载数据。防止内存释放
                var lazy = result.Count == 0 ? "" : publisheds.First().FormType.FormName + publisheds.First().BaseEmployee.RealName + publisheds.First().Form.FormNo;

                return publisheds;
            }

        }

    }
}
