using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Entities;
using Core.FrontEnd;
using PagedList.Core;

namespace Core.DataAccess
{
    public interface IEntityRepository<T> where T:class,IEntity,new()
    {
        //List<T> GetAll(Expression<Func<T, bool>> filter = null);
        //List<T> GetAllQueryInclude(Expression<Func<T, bool>> filter = null, string includ = null);
        //IPagedList<T> GetAllQueryIncludePaged(Expression<Func<T, bool>> filter = null, int page = 1, int pageSize = 25, string includ = null);
        //IPagedList<T> GetAllQueryIncludePagedFilter(Expression<Func<T, bool>> filter = null, int page = 1, int pageSize = 25, string includ = null, List<ColumnFilter> columnFilters = null);
        //T Get(Expression<Func<T, bool>> filter);
        //T GetQueryInclude(Expression<Func<T, bool>> filter, string includ = null);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        T Get(Expression<Func<T, bool>> filter);
        List<T> GetAll(Expression<Func<T, bool>> filter = null);
        T GetQueryInclude(Expression<Func<T, bool>> filter, string include = null);
        List<T> GetAllQueryInclude(Expression<Func<T, bool>> filter = null, string include = null);
        IPagedList<T> GetAllQueryIncludePaged(Expression<Func<T, bool>> filter = null, int page = 1, int pageSize = 25, string include = null);
        IPagedList<T> Query(Query<T> query = null, int page = 1, int pageSize = 25);
        IEnumerable<T> Query(Expression<Func<T, bool>> filter, int page = 1, int pageSize = 25);
        IEnumerable<T> Query(string sql, DbParamCollection parameters = null);
        int Count(Query<T> query = null);
        int Count(Expression<Func<T, bool>> filter);
        int Count(string sql, DbParamCollection parameters = null);
        bool Any(Query<T> query = null);
        bool Any(Expression<Func<T, bool>> filter);
        bool Any(string sql, DbParamCollection parameters = null);
        IPagedList<T> PagedQuery(Request request);
        IPagedList<T> PagedQuery(int page, int pageSize = 20, Query<T> query = null, bool isApplyOrder = true);
        T Get(Query<T> query = null);
        T Get(string sql, DbParamCollection parameters = null);
        int Execute(string sql, DbParamCollection parameters = null, int? commandTimeout = null);
        TResult ExecuteScalar<TResult>(string sql, DbParamCollection parameters = null);
        List<TResult> ExecuteList<TResult>(string sql, DbParamCollection parameters = null);




    }
}

