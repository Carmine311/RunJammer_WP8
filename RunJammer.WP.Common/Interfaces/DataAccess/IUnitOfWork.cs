using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RunJammer.WP.Common.Interfaces.DataAccess
{
    public interface IUnitOfWork: IDisposable
    {
        IEnumerable<T> GetData<T>();
        T GetItemById<T>(object id);
        void Add<T>(T item);
        void Delete<T>(T item);
        void Update<T>(T item);
    }
}
