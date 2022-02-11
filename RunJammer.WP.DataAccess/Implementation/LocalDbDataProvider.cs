using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Common.DataAccess.Interface;
using RunJammer.WP.Model;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.DataAccess.Implementation
{
    public class LocalDbDataProvider : IDataProvider<object, object>
    {
        #region Interface

        public IEnumerable<T> GetData<T>() where T : class
        {
            try
            {
                return _dataContext.GetTable<T>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting data of type: " + typeof(T).Name, ex);
            }
        }

        public void Create<T>(T item) where T : class
        {
            _dataContext.GetTable<T>().InsertOnSubmit(item);
        }

        public void Create<T>(IEnumerable<T> items) where T : class
        {
            try
            {
                _dataContext.GetTable<T>().InsertAllOnSubmit(items);
            }
            catch (Exception ex)
            {
                
            }
        }

        public T GetItemById<T>(object id) where T : class
        {
            throw new NotImplementedException();
        }

        public void Update<T>(T item) where T : class
        {
            _dataContext.SubmitChanges();
        }

        public void Delete<T>(T item) where T : class
        {
            _dataContext.GetTable<T>().DeleteOnSubmit(item);
            if (item is RunSession)
            {
                var session = item as RunSession;
                _dataContext.GetTable<RunSessionWaypoint>().DeleteAllOnSubmit(session.Waypoints);
            }
            SubmitChanges();
        }

        public void SubmitChanges()
        {
            try
            {
                _dataContext.SubmitChanges();
            }
            catch (ChangeConflictException ex)
            {
                StringBuilder builder = new StringBuilder();

                using (StringWriter sw = new StringWriter(builder))
                {
                    sw.WriteLine("Optimistic concurrency error:");
                    sw.WriteLine(ex.Message);

                    foreach (ObjectChangeConflict occ in _dataContext.ChangeConflicts)
                    {
                        Type objType = occ.Object.GetType();
                        MetaTable metatable = _dataContext.Mapping.GetTable(objType);
                        object entityInConflict = occ.Object;

                        sw.WriteLine("Table name: {0}", metatable.TableName);

                        var noConflicts =
                            from property in objType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            where property.CanRead &&
                                  property.CanWrite &&
                                  property.GetIndexParameters().Length == 0 &&
                                  !occ.MemberConflicts.Any(c => c.Member.Name != property.Name)
                            orderby property.Name
                            select property;

                        foreach (var property in noConflicts)
                        {
                            sw.WriteLine("\tMember: {0}", property.Name);
                            sw.WriteLine("\t\tCurrent value: {0}",
                                property.GetGetMethod().Invoke(occ.Object, new object[0]));
                        }

                        sw.WriteLine("\t-- Conflicts Start Here --", metatable.TableName);

                        foreach (MemberChangeConflict mcc in occ.MemberConflicts)
                        {
                            sw.WriteLine("\tMember: {0}", mcc.Member.Name);
                            sw.WriteLine("\t\tCurrent value: {0}", mcc.CurrentValue);
                            sw.WriteLine("\t\tOriginal value: {0}", mcc.OriginalValue);
                            sw.WriteLine("\t\tDatabase value: {0}", mcc.DatabaseValue);
                        }
                    }

                    sw.WriteLine();
                    sw.WriteLine("Attempted SQL: ");

                    TextWriter tw = _dataContext.Log;

                    try
                    {
                        _dataContext.Log = sw;
                        _dataContext.SubmitChanges();
                    }
                    catch (ChangeConflictException)
                    {
                        // This is what we wanted.
                    }
                    catch
                    {
                        sw.WriteLine("Unable to recreate SQL!");
                    }
                    finally
                    {
                        _dataContext.Log = tw;
                    }
                }
                throw new Exception(builder.ToString());
            }
        }



        #endregion

        #region Construction

        public
        LocalDbDataProvider()
        {

        }

        public LocalDbDataProvider(RunJammerDataContext dataContext)
        {
            if (!dataContext.DatabaseExists())
            {
                dataContext.CreateDatabase();
            }
            _dataContext = dataContext;
        }

        #endregion

        #region Implementation

        private readonly RunJammerDataContext _dataContext;

        #endregion


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
