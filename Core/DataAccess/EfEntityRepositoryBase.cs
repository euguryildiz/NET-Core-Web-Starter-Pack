using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core.Entities;
using Core.FrontEnd;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using Core.Utilities.Filter;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Core.DataAccess
{
    public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext, new()
    {

        protected DbContext dbContext;
        protected DbSet<TEntity> dbSet;

        private bool _disposed = false;


        public EfEntityRepositoryBase()
        {
            dbContext = new TContext();
            dbSet = dbContext.Set<TEntity>();
        }


        public void Add(TEntity entity)
        {
            using (TContext context = new TContext())
            {
                var addedTEntity = context.Entry(entity);
                addedTEntity.State = EntityState.Added;
                context.SaveChanges();
            }

        }
        public void Update(TEntity entity)
        {
            using (TContext context = new TContext())
            {
                var updateTEntity = context.Entry(entity);
                updateTEntity.State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public void Delete(TEntity entity)
        {
            using (TContext context = new TContext())
            {

                var deletedTEntity = context.Entry(entity);
                deletedTEntity.State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            using (TContext context = new TContext())
            {
                return context.Set<TEntity>().FirstOrDefault(filter);
            }
        }
        public List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            using (TContext context = new TContext())
            {
                return filter == null ? context.Set<TEntity>().ToList() : context.Set<TEntity>().Where(filter).ToList();
            }
        }
        public TEntity GetQueryInclude(Expression<Func<TEntity, bool>> filter, string include = null)
        {
            using (TContext context = new TContext())
            {
                List<string> includedTables = new List<string>();
                foreach (var item in include.Split(','))
                {
                    includedTables.Add(item);
                }

                IQueryable<TEntity> query = context.Set<TEntity>();
                if (includedTables != null)
                    query = includedTables.Aggregate(query, (current, include) => current.Include(include));

                return filter == null ? query.FirstOrDefault() : query.Where(filter).FirstOrDefault();
            }
        }
        public List<TEntity> GetAllQueryInclude(Expression<Func<TEntity, bool>> filter = null, string include = null)
        {
            using (TContext context = new TContext())
            {
                List<string> includedTables = new List<string>();
                foreach (var item in include.Split(','))
                {
                    includedTables.Add(item);
                }

                IQueryable<TEntity> query = context.Set<TEntity>();
                if (includedTables != null)
                    query = includedTables.Aggregate(query, (current, include) => current.Include(include));
               
                return filter == null ? query.ToList() : query.Where(filter).ToList();
            }
        }
        public IPagedList<TEntity> GetAllQueryIncludePaged(Expression<Func<TEntity, bool>> filter = null, int page = 1, int pageSize = 25, string include = null)
        {
            using (TContext context = new TContext())
            {
                List<string> includedTables = new List<string>();
                foreach (var item in include.Split(','))
                {
                    includedTables.Add(item);
                }

                IQueryable<TEntity> query = context.Set<TEntity>();
                if (includedTables != null)
                    query = includedTables.Aggregate(query, (current, include) => current.Include(include));

                return filter == null ? query.ToPagedList(page,pageSize) : query.Where(filter).ToPagedList(page, pageSize);
            }
        }
        public IPagedList<TEntity> Query(Query<TEntity> query = null, int page = 1, int pageSize = 25)
        {
            IQueryable<TEntity> data = dbSet;
            if (query == null)
            {
                query = new Query<TEntity>();
            }
            // 
            data = Apply(query, data);
            return data.ToPagedList(page, pageSize);
        }
        public IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> filter, int page = 1, int pageSize = 25)
        {
            return Query(new Query<TEntity>().Where(filter));
        }
        public IEnumerable<TEntity> Query(string sql, DbParamCollection parameters = null)
        {
            if (parameters != null && parameters.Count > 0)
            {
                return dbSet.FromSqlRaw(sql, parameters.Cast<SqlParameter>().ToArray()).ToList();
            }

            return dbSet.FromSqlRaw(sql).ToList();
        }
        public int Count(Query<TEntity> query = null)
        {
            IQueryable<TEntity> data = dbSet;
            if (query == null)
            {
                query = new Query<TEntity >();
            }
            data = Apply(query, data);

            return  data.Count();
        }
        public int Count(Expression<Func<TEntity, bool>> filter)
        {
            return  Count(new Query<TEntity>().Where(filter));
        }
        public int Count(string sql, DbParamCollection parameters = null)
        {
            return ExecuteScalar<int>(sql, parameters);
        }
        public bool Any(Query<TEntity> query = null)
        {
            IQueryable<TEntity> data = dbSet;
            if (query == null)
            {
                query = new Query<TEntity>();
            }
            data = Apply(query, data);

            return data.Any();
        }
        public bool Any(Expression<Func<TEntity, bool>> filter)
        {
            return Any(new Query<TEntity>().Where(filter));
        }
        public bool Any(string sql, DbParamCollection parameters = null)
        {
            return Count(sql, parameters) > 0;
        }
        public IPagedList<TEntity> PagedQuery(Request request)
        {
            var query = new Query<TEntity>().FilterByRequest(request);
            return PagedQuery(request.Page, request.PageSize, query);
        }
        public IPagedList<TEntity> PagedQuery(int page, int pageSize = 20, Query<TEntity> query = null, bool isApplyOrder = true)
        {

            IQueryable<TEntity> data = dbSet;
            if (query == null)
            {
                query = new Query<TEntity>();
            }

            var total = 0;
            if (pageSize > 0)
            {
                data = Apply(query, data, isApplyOrder);
                total = data.Count();
            }

            if (page <= 0)
            {
                page = 1;
            }
            if (pageSize > 0)
            {
                var list = data.Skip((page - 1) * pageSize).Take(pageSize);
                return data.ToPagedList(page, pageSize);
            }
            else
            {
                var list =  data;
                return list.ToPagedList(0, 0);
            }
        }
        public TEntity Get(Query<TEntity> query = null)
        {
            IQueryable<TEntity> data = dbSet;
            if (query == null)
            {
                query = new Query<TEntity>();
            }
             
            data = Apply(query, data);

            return data.FirstOrDefault();
        }
        public TEntity Get(string sql, DbParamCollection parameters = null)
        {
            if (parameters != null && parameters.Count > 0)
            {
                return dbSet.FromSqlRaw(sql, parameters.Cast<SqlParameter>().ToArray()).FirstOrDefault();
            }

            return dbSet.FromSqlRaw(sql).FirstOrDefault();
        }
        public int Execute(string sql, DbParamCollection parameters = null, int? commandTimeout = null)
        {
            var connection = dbContext.Database.GetDbConnection();
            var connectionOpened = false;
            var data = default(int);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                //if (_unitOfWork.HasTransaction())
                //{
                //    command.Transaction = _unitOfWork.GetTransaction().GetDbTransaction();
                //}
                if (parameters != null && parameters.Count > 0)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }
                if (commandTimeout.HasValue)
                {
                    command.CommandTimeout = commandTimeout.Value;
                }
                if (connection.State.Equals(System.Data.ConnectionState.Closed))
                {
                    connection.Open();
                    connectionOpened = true;
                }
                data = command.ExecuteNonQuery();
            }
            if (connectionOpened && connection.State.Equals(System.Data.ConnectionState.Open))
            {
                connection.Close();
            }
            return data;
        }
        public TResult ExecuteScalar<TResult>(string sql, DbParamCollection parameters = null)
        {
            var connection = dbContext.Database.GetDbConnection();

            var data = default(object);
            var connectionOpened = false;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                //if (_unitOfWork.HasTransaction())
                //{
                //    command.Transaction = _unitOfWork.GetTransaction().GetDbTransaction();
                //}
                if (parameters != null && parameters.Count > 0)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }
                if (connection.State.Equals(System.Data.ConnectionState.Closed))
                {
                    connection.Open();
                    connectionOpened = true;
                }
                data = command.ExecuteScalar();
            }
            if (connectionOpened && connection.State.Equals(System.Data.ConnectionState.Open))
            {
                connection.Close();
            }
            if (data == null || data == DBNull.Value)
            {
                return default;
            }
            return (TResult)Convert.ChangeType(data, typeof(TResult));
        }
        protected IQueryable<TEntity> Apply(Query<TEntity> query, IQueryable<TEntity> source, bool applyOrder = true)
        {
            foreach (var filter in query.Filters)
            {
                source = Queryable.Where(source, filter);
            }

            foreach (var include in query.Includes)
            {
                if (include.Expression != null)
                {
                    source = source.Include(string.Join(".", include.Expression.Body.ToString().Split('.').Skip(1)));
                }
                else
                {
                    source = source.Include(include.PropertyPath);
                }
            }

            if (applyOrder)
            {
                source = ApplyOrder(query, source);
            }

            if (query.SelectFields != null)
            {
                source = Queryable.Select(source, (dynamic)query.SelectFields);
            }
            if (query.CommandTimeout > 0)
            {
                dbContext.Database.SetCommandTimeout(query.CommandTimeout);
            }
            else
            {
                dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(15));
            }
            return source;
        }
        protected IQueryable<TEntity> ApplyOrder(Query<TEntity> query, IQueryable<TEntity> source)
        {
            if (query.Orders.Any())
            {
                var firsOrder = query.Orders.First();
                if (firsOrder.Asc)
                {
                    source = Queryable.OrderBy(source, (dynamic)firsOrder.Expression);
                }
                else
                {
                    source = Queryable.OrderByDescending(source, (dynamic)firsOrder.Expression);
                }
                foreach (var orderBy in query.Orders.Skip(1))
                {
                    if (orderBy.Asc)
                    {
                        source = Queryable.ThenBy(source, (dynamic)orderBy);
                    }
                    else
                    {
                        source = Queryable.ThenByDescending(source, (dynamic)orderBy);
                    }
                }
            }
            else
            {
                source = Queryable.OrderByDescending(source, a => a.Id);
            }

            return source;
        }
        public List<TResult> ExecuteList<TResult>(string sql, DbParamCollection parameters = null) 
        {
            var connection = dbContext.Database.GetDbConnection();

            var data = new List<TResult>();
            var properties = typeof(TResult).GetProperties();

            var connectionOpened = false;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                //if (_unitOfWork.HasTransaction())
                //{
                //    command.Transaction = _unitOfWork.GetTransaction().GetDbTransaction();
                //}
                if (parameters != null && parameters.Count > 0)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }
                if (connection.State.Equals(System.Data.ConnectionState.Closed))
                {
                    connection.Open();
                    connectionOpened = true;
                }
                //using (var reader = command.ExecuteReader())
                //{
                //    while (reader.Read())
                //    {
                //        var item = new TResult();
                //        for (int i = 0; i < reader.FieldCount; i++)
                //        {
                //            var name = reader.GetName(i);
                //            var property = properties.FirstOrDefault(a => a.Name == name);
                //            if (property != null)
                //            {
                //                if (!reader.IsDBNull(i))
                //                {
                //                    Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                //                    property.SetValue(item, Convert.ChangeType(reader.GetValue(i), convertTo), null);
                //                }
                //            }
                //        }
                //        data.Add(item);
                //    }
                //}
            }
            if (connectionOpened && connection.State.Equals(System.Data.ConnectionState.Open))
            {
                connection.Close();
            }
            return data;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            _disposed = true;
        }
        public virtual void Dispose()
        {
            Dispose(true);
        }


    }
}

