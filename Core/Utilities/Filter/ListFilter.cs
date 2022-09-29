using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Core.Utilities.Filter
{
    //public static class ListFilter
    //{
    //    public static Expression<Func<T, bool>> FilterDynamic<T>(string fieldName, ICollection<string> values)
    //    {
    //        var param = Expression.Parameter(typeof(T), "e");
    //        var prop = Expression.PropertyOrField(param, fieldName);
    //        var body = Expression.Call(typeof(Enumerable), "Contains", new[] { typeof(string) },
    //            Expression.Constant(values), prop);
    //        var predicate = Expression.Lambda<Func<T, bool>>(body, param);
    //        return predicate;
    //    }

    //    public static IQueryable<T> FilterDynamic<T>(this IQueryable<T> query, string fieldName, ICollection<string> values)
    //    {
    //        var param = Expression.Parameter(typeof(T), "e");
    //        var prop = Expression.PropertyOrField(param, fieldName);
    //        var body = Expression.Call(typeof(Enumerable), "Contains", new[] { typeof(string) },
    //            Expression.Constant(values), prop);
    //        var predicate = Expression.Lambda<Func<T, bool>>(body, param);
    //        return query.Where(predicate);
    //    }


    //    public static IQueryable<T> BuildExpression<T>(this IQueryable<T> source, DbContext context, string columnName, string value, QueryableFilterCompareEnum? compare = QueryableFilterCompareEnum.Equal)
    //    {
    //        var param = Expression.Parameter(typeof(T));

    //        // Get the field/column from the Entity that matches the supplied columnName value
    //        // If the field/column does not exists on the Entity, throw an exception; There is nothing more that can be done
    //        MemberExpression dataField;

    //        var model = context.Model.FindEntityType(typeof(T)); // start with our own entity
    //        var props = model.GetPropertyAccessors(param); // get all available field names including navigations
    //        var reference = props.First(p => Microsoft.EntityFrameworkCore.RelationalPropertyExtensions.GetColumnName(p.Item1) == columnName); // find the filtered column - you might need to handle cases where column does not exist

    //        dataField = reference.Item2 as MemberExpression; // we happen to already have correct property accessors in our Tuples	

    //        ConstantExpression constant = !string.IsNullOrWhiteSpace(value)
    //            ? Expression.Constant(value.Trim(), typeof(string))
    //            : Expression.Constant(value, typeof(string));

    //        BinaryExpression binary = GetBinaryExpression(dataField, constant, compare);
    //        Expression<Func<T, bool>> lambda = (Expression<Func<T, bool>>)Expression.Lambda(binary, param);
    //        return source.Where(lambda);
    //    }

    //    private static IEnumerable<Tuple<IProperty, Expression>> GetPropertyAccessors(this IEntityType model, Expression param)
    //    {
    //        var result = new List<Tuple<IProperty, Expression>>();

    //        result.AddRange(model.GetProperties()
    //                                    .Where(p => !p.IsShadowProperty()) // this is your chance to ensure property is actually declared on the type before you attempt building Expression
    //                                    .Select(p => new Tuple<IProperty, Expression>(p, Expression.Property(param, p.Name)))); // Tuple is a bit clunky but hopefully conveys the idea

    //        foreach (var nav in model.GetNavigations().Where(p => p is Navigation))
    //        {
    //            var parentAccessor = Expression.Property(param, nav.Name); // define a starting point so following properties would hang off there
    //            result.AddRange(GetPropertyAccessors(nav.ForeignKey.PrincipalEntityType, parentAccessor)); //recursively call ourselves to travel up the navigation hierarchy
    //        }

    //        return result;
    //    }


    //}
}

