using System;
using System.Collections.Generic;
using System.Linq;
using Carlzhu.Iooin.Entity;


namespace Carlzhu.Iooin.Business
{

    public class BaseServices<T> where T : class, new()
    {
        private readonly BaseRepository<T> _baseRepository = new BaseRepository<T>();
        public T Single(Func<T, bool> lambda)
        {
            return this.LoadEntities(lambda).FirstOrDefault();
        }

        public T AddEntity(T entity)
        {
            return _baseRepository.AddEntity(entity);
        }
        public bool IsAddEntity(T entity)
        {
            return _baseRepository.AddEntity(entity) != null;
        }

        public bool AddEntities(List<T> ts)
        {
            try
            {
                ts.ForEach(t => this.AddEntity(t));
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool UpdateEntity(T entity)
        {
            return _baseRepository.UpdateEntity(entity);
        }

        public bool DeleteEntity(T entity)
        {
            return _baseRepository.DeleteEntity(entity);
        }

        public IQueryable<T> LoadEntities(Func<T, bool> lambda)
        {


            return _baseRepository.LoadEntities(lambda);
        }

        public int Count(Func<T, bool> lambda)
        {
            return this.LoadEntities(lambda).Count();
        }

        public IQueryable<T> LoadPageEntities<TS>(int pageIndex, int pageSize, out int total, Func<T, bool> whereLambda, bool isAsc,
            Func<T, TS> orderByLambda, Func<T, bool> exp2)
        {
            return _baseRepository.LoadPageEntities(pageIndex, pageSize, out total, whereLambda, isAsc, orderByLambda, exp2);
        }



    }
}
