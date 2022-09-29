using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Business.Abstract;
using Core.DataAccess;
using Core.Entities;
using Core.FrontEnd;
using Core.Utilities.Result;
using DataAccess.Concrete;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace Business.Concrete
{
    public class BaseManager<TEntity> : IBaseService<TEntity>
    where TEntity : class, IEntity, new()
    {
        private EfEntityRepositoryBase<TEntity, BuilderContext> _entityRepository = new EfEntityRepositoryBase<TEntity, BuilderContext>();

        public BaseManager(EfEntityRepositoryBase<TEntity, BuilderContext> entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public void Add(TEntity entity)
        {
            _entityRepository.Add(entity);
        }

        public void Update(TEntity entity)
        {
            _entityRepository.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _entityRepository.Delete(entity);
        }

        public bool Any(Query<TEntity> query = null)
        {
            return _entityRepository.Any(query);
        }

        public bool Any(Expression<Func<TEntity, bool>> filter)
        {
            return _entityRepository.Any(filter);
        }

        public bool Any(string sql, DbParamCollection parameters = null)
        {
            return _entityRepository.Any(sql,parameters);
        }

        public int Count(Query<TEntity> query = null)
        {
            return _entityRepository.Count(query);
        }

        public int Count(Expression<Func<TEntity, bool>> filter)
        {
            return _entityRepository.Count(filter);
        }

        public int Count(string sql, DbParamCollection parameters = null)
        {
            return _entityRepository.Count(sql,parameters);
        }

        public int Execute(string sql, DbParamCollection parameters = null, int? commandTimeout = null)
        {
            return _entityRepository.Execute(sql,parameters,commandTimeout);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            return _entityRepository.Get(filter);
        }

        public TEntity Get(Query<TEntity> query = null)
        {
            return _entityRepository.Get(query);
        }

        public TEntity Get(string sql, DbParamCollection parameters = null)
        {
            return _entityRepository.Get(sql,parameters);
        }

        public List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            return _entityRepository.GetAll(filter);
        }

        public List<TEntity> GetAllQueryInclude(Expression<Func<TEntity, bool>> filter = null, string include = null)
        {
            return _entityRepository.GetAllQueryInclude(filter,include);
        }

        public IPagedList<TEntity> GetAllQueryIncludePaged(Expression<Func<TEntity, bool>> filter = null, int page = 1, int pageSize = 25, string include = null)
        {
            return _entityRepository.GetAllQueryIncludePaged(filter,page,pageSize,include);
        }

        public TEntity GetQueryInclude(Expression<Func<TEntity, bool>> filter, string include = null)
        {
            return _entityRepository.GetQueryInclude(filter, include);
        }

        public IPagedList<TEntity> PagedQuery(Request request)
        {
            return _entityRepository.PagedQuery(request);
        }

        public IPagedList<TEntity> PagedQuery(int page, int pageSize = 20, Query<TEntity> query = null, bool isApplyOrder = true)
        {
            return _entityRepository.PagedQuery(page,pageSize,query,isApplyOrder);
        }

        public IEnumerable<TEntity> Query(string sql, DbParamCollection parameters = null)
        {
            return _entityRepository.Query(sql, parameters);
        }

        public IPagedList<TEntity> Query(Query<TEntity> query = null, int page = 1, int pageSize = 25)
        {
            return _entityRepository.Query(query, page, pageSize);
        }

        public IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> filter, int page = 1, int pageSize = 25)
        {
            return _entityRepository.Query(filter, page, pageSize);
        }
    

    }



}

