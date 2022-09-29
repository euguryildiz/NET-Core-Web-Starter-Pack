using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.FrontEnd
{
    public class Query<T> where T : class
    {
        private List<Expression<Func<T, bool>>> _filters = new List<Expression<Func<T, bool>>>();
        private List<OrderWithDir> _orderBys = new List<OrderWithDir>();
        private List<IncludeData> _includes = new List<IncludeData>();
        private LambdaExpression _select = null;
        private int _commandTimeout;

        public LambdaExpression SelectFields => _select;
        public List<Expression<Func<T, bool>>> Filters => _filters;
        public List<IncludeData> Includes => _includes;
        public List<OrderWithDir> Orders => _orderBys;
        public int CommandTimeout => _commandTimeout;

        public Query<T> FilterByRequest(Request request)
        {
            if (request != null)
            {
                if (request.Columns != null)
                {
                    var fields = string.Join(",", request.Columns.Select(a => a.Field));
                    Select(fields);
                    foreach (var item in request.Columns.Where(a => !string.IsNullOrEmpty(a.Order)))
                    {
                        if ("asc".Equals(item.Order, StringComparison.OrdinalIgnoreCase))
                        {
                            OrderBy(item.Field);
                        }
                        else if ("desc".Equals(item.Order, StringComparison.OrdinalIgnoreCase))
                        {
                            OrderByDescending(item.Field);
                        }
                    }
                }
                if (request.Filters != null)
                {
                    foreach (var item in request.Filters)
                    {
                        var expression = GetFilterExpression(item);
                        if (expression != null)
                        {
                            Where(expression);
                        }
                    }
                }
            }
            return this;
        }
        public Query<T> Where(Expression<Func<T, bool>> filter)
        {
            _filters.Add(filter);
            return this;
        }
        public Query<T> OrderBy<TProperty>(Expression<Func<T, TProperty>> orderBy)
        {
            _orderBys.Add(new OrderWithDir(orderBy, true));
            return this;
        }
        public Query<T> OrderBy(string property)
        {
            var parameterExpression = Expression.Parameter(typeof(T), "a");
            var expression = GetPropertySelector(property, parameterExpression);
            if (expression != null)
            {
                _orderBys.Add(new OrderWithDir(Expression.Lambda(expression, parameterExpression), true));
            }
            return this;
        }
        public Query<T> OrderByDescending<TProperty>(Expression<Func<T, TProperty>> orderBy)
        {
            _orderBys.Add(new OrderWithDir(orderBy, false));
            return this;
        }
        public Query<T> OrderByDescending(string property)
        {
            var parameterExpression = Expression.Parameter(typeof(T), "a");
            var expression = GetPropertySelector(property, parameterExpression);
            if (expression != null)
            {
                _orderBys.Add(new OrderWithDir(Expression.Lambda(expression, parameterExpression), false));
            }
            return this;
        }
        public Query<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            _includes.Add(new IncludeData(navigationPropertyPath));
            return this;
        }
        public Query<T> Include(string navigationPropertyPath)
        {
            _includes.Add(new IncludeData(navigationPropertyPath));
            return this;
        }
        public Query<T> Select(Expression<Func<T, T>> select)
        {
            _select = select;
            return this;
        }
        public Query<T> Select(string properties)
        {
            Func<Expression, string, List<string>, KeyValuePair<System.Reflection.PropertyInfo, Expression>> getExpression = null;
            getExpression = (expParam, propName, propNames) =>
            {
                if (propName.Contains("."))
                {
                    var objPropName = propName.Substring(0, propName.IndexOf("."));
                    var propInfo = expParam.Type.GetProperty(objPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null)
                    {
                        return new KeyValuePair<System.Reflection.PropertyInfo, Expression>();
                    }
                    var propExpParam = Expression.Property(expParam, propInfo);
                    var propBindings = new List<MemberBinding>();
                    var propSubNames = propNames.Where(a => a.StartsWith(objPropName + ".")).Select(a => a.Substring(a.IndexOf(".") + 1)).ToList();
                    foreach (var propSubName in propSubNames)
                    {
                        var property = getExpression(propExpParam, propSubName, propSubNames);
                        if (property.Key != null)
                        {
                            propBindings.Add(Expression.Bind(property.Key, property.Value));
                        }
                    }

                    var newExpression = Expression.MemberInit(Expression.New(propInfo.PropertyType.GetConstructors().Single()), propBindings);
                    return new KeyValuePair<System.Reflection.PropertyInfo, Expression>(propInfo, newExpression);
                }
                else
                {
                    var propInfo = expParam.Type.GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo == null)
                    {
                        return new KeyValuePair<System.Reflection.PropertyInfo, Expression>();
                    }
                    return new KeyValuePair<System.Reflection.PropertyInfo, Expression>(propInfo, Expression.Property(expParam, propInfo));
                }
            };

            var parameter = Expression.Parameter(typeof(T), "a");
            var propertyNames = properties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var addedProps = new List<string>();
            var bindings = new List<MemberBinding>();
            foreach (var item in propertyNames)
            {
                if (addedProps.Contains(item))
                {
                    continue;
                }
                var property = getExpression(parameter, item, propertyNames);
                if (property.Key != null)
                {
                    bindings.Add(Expression.Bind(property.Key, property.Value));
                }

                addedProps.Add(item);
                if (item.Contains("."))
                {
                    addedProps.AddRange(propertyNames.Where(a => item != a && a.StartsWith(item.Substring(0, item.IndexOf(".") + 1))));
                }
            }

            var expression = Expression.MemberInit(Expression.New(typeof(T).GetConstructors().Single()), bindings);
            _select = Expression.Lambda(expression, parameter);

            return this;
        }
        public Query<T> SetCommandTimeout(int timeount)
        {
            _commandTimeout = timeount;
            return this;
        }

        private Expression GetPropertySelector(string propertyName, ParameterExpression arg)
        {
            //arg = Expression.Parameter(typeof(T), "a");
            if (propertyName.Contains("."))
            {
                var properties = propertyName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var type = typeof(T);
                Expression expression = arg;
                foreach (var property in properties)
                {
                    var pInfo = type.GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pInfo == null)
                    {
                        return null;
                    }
                    expression = Expression.Property(expression, pInfo);
                    type = pInfo.PropertyType;
                }
                return expression;
            }
            var propInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propInfo == null)
            {
                return null;
            }
            return Expression.Property(arg, propertyName);
        }
        private Expression<Func<T, bool>> GetFilterExpression(RequestFilter filter)
        {
            var expressions = new List<Expression>();
            var arg = Expression.Parameter(typeof(T), "a");
            if (filter.Field.Contains("|"))
            {
                foreach (var item in filter.Field.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var exp = GetPropertySelector(item, arg);
                    if (exp != null)
                    {
                        expressions.Add(exp);
                    }
                }
            }
            else
            {
                var exp = GetPropertySelector(filter.Field, arg);
                if (exp != null)
                {
                    expressions.Add(exp);
                }
            }
            if (expressions == null || !expressions.Any() || !(expressions.First() is MemberExpression))
            {
                return null;
            }

            var propType = ((MemberExpression)expressions.First()).Type;
            object value = null;
            bool isNullable = (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>));
            if (isNullable)
            {
                if (!string.IsNullOrEmpty(filter.Value))
                {
                    value = Convert.ChangeType(filter.Value, Nullable.GetUnderlyingType(propType));
                }
            }
            else
            {
                if (propType == typeof(string))
                {
                    if (!string.IsNullOrEmpty(filter.Value))
                    {
                        value = filter.Value;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(filter.Value))
                    {
                        return null;
                    }
                    if (propType.IsEnum)
                    {
                        if (int.TryParse(filter.Value, out int intValue))
                        {
                            value = Enum.ToObject(propType, intValue);
                        }
                        else
                        {
                            value = Enum.Parse(propType, filter.Value, true);
                        }
                    }
                    else
                    {
                        value = Convert.ChangeType(filter.Value, propType);
                    }
                }
            }
            if (expressions.Count == 1)
            {
                Expression predicateBody = GetFilterExpression(filter, expressions.First(), propType, value);
                return Expression.Lambda<Func<T, bool>>(predicateBody, arg);
            }
            else
            {
                var expressionList = new List<Expression>();
                foreach (var exp in expressions)
                {
                    Expression predicateBody = GetFilterExpression(filter, exp, propType, value);
                    expressionList.Add(predicateBody);
                }

                Func<List<Expression>, Expression<Func<T, bool>>> getOrExpression = null;
                getOrExpression = (list) =>
                {
                    if (list.Count == 2)
                    {
                        return Expression.Lambda<Func<T, bool>>(Expression.Or(list[0], list[1]), arg);
                    }
                    return Expression.Lambda<Func<T, bool>>(Expression.Or(list[0], getOrExpression(list.Skip(1).ToList())), arg);
                };
                return getOrExpression(expressionList);
            }

        }
        private Expression GetFilterExpression(RequestFilter filter, Expression expression, Type propType, object value)
        {
            var constant = Expression.Constant(value, propType);
            Expression predicateBody;

            switch (filter.Operant)
            {
                case "=":
                    predicateBody = Expression.Equal(expression, constant);
                    break;
                case "!=":
                    predicateBody = Expression.Not(Expression.Equal(expression, constant));
                    break;
                case ">":
                    predicateBody = Expression.GreaterThan(expression, constant);
                    break;
                case ">=":
                    predicateBody = Expression.GreaterThanOrEqual(expression, constant);
                    break;
                case "<":
                    predicateBody = Expression.LessThan(expression, constant);
                    break;
                case "<=":
                    predicateBody = Expression.LessThanOrEqual(expression, constant);
                    break;
                case "+":
                    predicateBody = Expression.Call(expression, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), constant);
                    break;
                case "-":
                    predicateBody = Expression.Call(expression, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), constant);
                    break;
                case "!*":
                    predicateBody = Expression.Not(Expression.Call(expression, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant));
                    break;
                case "?*":
                    predicateBody = Expression.Call(expression, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant);
                    break;
                default:
                    throw new Exception("Not supported operator");
            }

            return predicateBody;
        }

        public class OrderWithDir
        {
            public LambdaExpression Expression { get; set; }
            public bool Asc { get; set; }

            public OrderWithDir(LambdaExpression expression, bool asc)
            {
                Expression = expression;
                Asc = asc;
            }
        }

        public class IncludeData
        {
            public LambdaExpression Expression { get; set; }
            public string PropertyPath { get; set; }

            public IncludeData(LambdaExpression expression)
            {
                Expression = expression;
            }

            public IncludeData(string propertyPath)
            {
                PropertyPath = propertyPath;
            }
        }
    }
}

