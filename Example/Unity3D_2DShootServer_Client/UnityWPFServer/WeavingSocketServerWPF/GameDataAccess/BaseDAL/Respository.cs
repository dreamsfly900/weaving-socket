
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GameDataAccess.BaseDAL
{
    public class Respository<T> : IRepository<T> where T : class
    {

        public string conStr;
        public Respository()
        {
            conStr = LiteDBHelper.GetConString();
        }

        public bool Insert(T entity)
        {
           try
            {
                using (var db = new LiteRepository(conStr))
                {
                    // simple access to Insert/Update/Upsert/Delete
                    var result = db.Upsert( entity );

                    //if( result.AsString == entity. )
                    return result;
                }

               
            }
            catch
            {
                return false;
            }
        }

        public bool Update(T entity)
        {
            try
            {
                
                using (var db = new LiteRepository(conStr))
                {
                    // simple access to Insert/Update/Upsert/Delete
                    bool result = db.Update(entity);

                    return result;

                }

               
            }
            catch
            {
                return false;
            }
        }
        public bool Delete(Expression<Func<T, bool>> where)
        {
            try
            {
                using (var db = new LiteRepository(conStr))
                {
                    // simple access to Insert/Update/Upsert/Delete
                    int result= db.Delete<T>( where );

                    return result > 0 ? true : false;
                }
                
            }
            catch
            {
                return false;
            }
        }

        public bool  DeleteAllEntity()
        {
            try
            {
                using (var db = new LiteRepository(conStr))
                {
                    // simple access to Insert/Update/Upsert/Delete
                    int result = db.Delete<T>(Query.All());

                    return result > 0 ? true : false;
                }

            }
            catch
            {
                return false;
            }
        }


        public List<T> GetALLEntity()
        {
            try
            {
                //int result;
                using (var db = new LiteRepository(conStr))
                {
                    // simple access to Insert/Update/Upsert/Delete
                    var result = db.Query<T>().ToList();
                    return result;

                }

                //return result > 0 ? true : false;
            }
            catch
            {
                return null;
            }
        }

        public List<T> GetAllEntityWhere(Expression<Func<T, bool>> where)
        {
            try
            {
                //int result;
                using (var db = new LiteRepository(conStr))
                {
                    // simple access to Insert/Update/Upsert/Delete
                    var result = db.Query<T>(  ).Where(where)  .ToList();
                    return result;

                }

                //return result > 0 ? true : false;
            }
            catch
            {
                return null;
            }
        }

        public T GetOneByIndexKey(Expression<Func<T, bool>> where)
        {
            try
            {
                //int result;
                using (var db = new LiteRepository(conStr))
                {
                    // simple access to Insert/Update/Upsert/Delete
                    var result = db.Query<T>().Where(where).Single();
                    return result;

                }

                //return result > 0 ? true : false;
            }
            catch
            {
                return null;
            }
        }

        public T GetOneByWhere(Expression<Func<T, bool>> where)
        {
            try
            {
                //int result;
                using (var db = new LiteRepository(conStr))
                {
                    // simple access to Insert/Update/Upsert/Delete
                    var result = db.Query<T>().Where(where).Single();
                    return result;

                }

                //return result > 0 ? true : false;
            }
            catch
            {
                return null;
            }
        }

        public List<T> GetPageEntities(int pageIndex, int PageSize)
        {
            try
            {
                //int result;
                using (var db = new LiteRepository(conStr))
                {

                    int totalCount = db.Query<T>().Count();
                    //计算页码
                    int pages = (int)Math.Ceiling((double)totalCount / (double)PageSize);

                    if (pageIndex > pages)
                        return null;
                    else
                    {
                        //Reverse 倒序排列
                      // var result = db.Query<T>().ToList().Skip(  (pageIndex -1)*PageSize    ).Take(PageSize).Reverse().ToList() ;

                        var result = db.Query<T>().Skip((pageIndex - 1) * PageSize).ToList().Take(PageSize).Reverse().ToList();

                        return result;
                    }
                    

                }
                
            }
            catch
            {
                return null;
            }
        }

        //by serach
        public List<T> GetPageEntities( int pageIndex, int PageSize, Expression<Func<T, bool>> wheresearch)
        {
            try
            {
                //int result;
                using (var db = new LiteRepository(conStr))
                {

                    int totalCount = db.Query<T>().Count();
                    //计算页码
                    int pages = (int)Math.Ceiling((double)totalCount / (double)PageSize);

                    if (pageIndex > pages)
                        return null;
                    else
                    {
                        //Reverse 倒序排列
                        //var result = db.Query<T>().Where(wheresearch).ToList().Skip((pageIndex - 1) * PageSize).Take(PageSize).Reverse().ToList();

                        var result = db.Query<T>().Where(wheresearch).Skip((pageIndex - 1) * PageSize).ToList().Take(PageSize).Reverse().ToList();


                        return result;
                    }


                }

            }
            catch
            {
                return null;
            }
        }

       
    }
}
