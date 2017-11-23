using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GameDataAccess.BaseDAL
{
    public interface IRepository  <T> where T : class
    {
        /// <summary>
        /// IRespository保存接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Insert(T entity);


        /// <summary>
        /// IRespository修改接口
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update(T entity);

        /// <summary>
        /// IRespository删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Delete( Expression<Func<T, bool>> where);


        bool DeleteAllEntity();

        /// <summary>
        /// 根据主键id查询
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        T GetOneByIndexKey(Expression<Func<T, bool>> where);

        /// <summary>
        /// 带条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        T GetOneByWhere(Expression<Func<T, bool>> where);


        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        List<T> GetALLEntity();

        /// <summary>
        /// 这里也可以用IEnumerable类型，带条件查询所有
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        List<T> GetAllEntityWhere(Expression<Func<T, bool>> where);


        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<T> GetPageEntities( int pageIndex, int PageSize);

        /// <summary>
        /// 分页带查询条件
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        List<T> GetPageEntities( int pageIndex, int PageSize, Expression<Func<T, bool>> wheresearch);




    }
}
