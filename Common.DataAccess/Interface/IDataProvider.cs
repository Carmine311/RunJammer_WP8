using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model.Interface;

namespace Common.DataAccess.Interface
{
	public interface IDataProvider<T, TID> where T : class
	{
		IEnumerable<T> GetData();
		void Create(T item);
		void Create(IEnumerable<T> items);
		T GetItemById(TID id);
		void Delete(T item);
	    void SubmitChanges();
	}
}
