using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DataAccess.Interface;
using Common.Model.Implementation;

namespace Common.DataAccess.Implementation
{
    public abstract class TestDataProvider : IDataProvider<object, object>
    {
        public abstract List<IEnumerable<object>> Lists { get; set; }

        protected TestDataProvider()
        {
            Lists = new List<IEnumerable<object>>();
        }

        public IEnumerable<T> GetData<T>() where T : class
        {
            var list = GetList<T>();

            return list;
        }

        private List<T> GetList<T>() where T : class
        {
            var list = Lists.FirstOrDefault(l => l != null && l.All(o => (o != null) && (o.GetType() == typeof(T)))).Cast<T>();
            return list.ToList();
        }

        public void Create<T>(T item) where T : class
        {
            var entity = item as LocalEntityBase;
            if (entity != null)
            {
                var list = GetList<T>();
                entity.LocalID = list.Count + 1;
                list.Add(entity as T);
            }

            GetList<T>().Add(item);
        }

        public void Create<T>(IEnumerable<T> items) where T : class
        {
            throw new NotImplementedException();
        }

        public T GetItemById<T>(object id) where T : class
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T item) where T : class
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T item) where T : class
        {
            GetList<T>().Remove(item);
        }

        public void SubmitChanges()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetDataAsync<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync<T>(T item) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> GetItemByIdAsync<T>(object id) where T : class
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync<T>(T item) where T : class
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<T>(T item) where T : class
        {
            throw new NotImplementedException();
        }



        public IEnumerable<object> GetData()
        {
            throw new NotImplementedException();
        }

        public void Create(object item)
        {
            throw new NotImplementedException();
        }

        public void Create(IEnumerable<object> items)
        {
            throw new NotImplementedException();
        }

        public object GetItemById(object id)
        {
            throw new NotImplementedException();
        }

        public void Delete(object item)
        {
            throw new NotImplementedException();
        }
    }
}
