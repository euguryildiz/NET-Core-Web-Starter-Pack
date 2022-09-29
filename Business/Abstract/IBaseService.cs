using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Entities;
using Core.FrontEnd;
using PagedList.Core;

namespace Business.Abstract
{
    public interface IBaseService<TEntity> where TEntity : class, IEntity, new()
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        TEntity Get(Expression<Func<TEntity, bool>> filter);
        List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null);
        TEntity GetQueryInclude(Expression<Func<TEntity, bool>> filter, string include = null);
        List<TEntity> GetAllQueryInclude(Expression<Func<TEntity, bool>> filter = null, string include = null);
        IPagedList<TEntity> GetAllQueryIncludePaged(Expression<Func<TEntity, bool>> filter = null, int page = 1, int pageSize = 25, string include = null);
        IPagedList<TEntity> Query(Query<TEntity> query = null, int page = 1, int pageSize = 25);
        IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> filter, int page = 1, int pageSize = 25);
        IEnumerable<TEntity> Query(string sql, DbParamCollection parameters = null);
        int Count(Query<TEntity> query = null);
        int Count(Expression<Func<TEntity, bool>> filter);
        int Count(string sql, DbParamCollection parameters = null);
        bool Any(Query<TEntity> query = null);
        bool Any(Expression<Func<TEntity, bool>> filter);
        bool Any(string sql, DbParamCollection parameters = null);
        IPagedList<TEntity> PagedQuery(Request request);
        IPagedList<TEntity> PagedQuery(int page, int pageSize = 20, Query<TEntity> query = null, bool isApplyOrder = true);
        TEntity Get(Query<TEntity> query = null );
        TEntity Get(string sql, DbParamCollection parameters = null);
        int Execute(string sql, DbParamCollection parameters = null, int? commandTimeout = null);

    }
}

