using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Core.FrontEnd
{
    public class DbParamCollection : CollectionBase, ICollection<DbParameter>
    {
        private int _OutputParameterIndex;

        #region ICollection<DbParameter> Members

        public void Add(DbParameter item)
        {
            if (item.Value == null)
            {
                item.Value = DBNull.Value;
            }
            InnerList.Add(item);
        }

        public void Add(string parameterName, object value)
        {
            DbParameter dbParameter = new SqlParameter(parameterName, value);
            if (dbParameter.Value == null)
            {
                dbParameter.Value = DBNull.Value;
            }
            InnerList.Add(dbParameter);
        }
        public void Add(string parameterName, object value, string typeName)
        {
            DbParameter dbParameter = new SqlParameter();
            dbParameter.ParameterName = parameterName;
            if (value == null)
            {
                value = DBNull.Value;
            }
            dbParameter.Value = value;
            ((SqlParameter)dbParameter).TypeName = typeName;
            InnerList.Add(dbParameter);
        }
        public void Add(string parameterName, object value, DbType dbType)
        {
            DbParameter dbParameter = new SqlParameter();
            dbParameter.ParameterName = parameterName;
            if (value == null)
            {
                value = DBNull.Value;
            }
            dbParameter.Value = value;
            dbParameter.DbType = dbType;
            InnerList.Add(dbParameter);
        }

        public void AddOutput(string parameterName, DbType type, int size = 0)
        {
            var dbParameter = new SqlParameter();
            dbParameter.ParameterName = parameterName;
            dbParameter.DbType = type;
            dbParameter.Direction = ParameterDirection.Output;

            if (size > 0)
            {
                dbParameter.Size = size;
            }

            if (type == DbType.Decimal)
            {
                dbParameter.Size = 18;
                dbParameter.Scale = 4;
            }
            _OutputParameterIndex = InnerList.Add(dbParameter);
        }

        public bool Contains(DbParameter item)
        {
            return InnerList.Contains(item);
        }

        public void CopyTo(DbParameter[] array, int arrayIndex)
        {
            InnerList.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get
            {
                return InnerList.IsReadOnly;
            }
        }

        public bool Remove(DbParameter item)
        {
            InnerList.Remove(item);
            return true;
        }
        public Array ToArray()
        {
            return InnerList.ToArray();
        }

        #endregion

        #region IEnumerable<DbParameter> Members

        public new IEnumerator<DbParameter> GetEnumerator()
        {
            return (IEnumerator<DbParameter>)InnerList.GetEnumerator();
        }

        #endregion

        public DbParameter GetOutPutParameter()
        {
            return (DbParameter)InnerList[_OutputParameterIndex];
        }

        public T GetOutput<T>()
        {
            var output = GetOutPutParameter();
            if (output != null && output.Value != null && output.Value != DBNull.Value)
            {
                var type = typeof(T);
                bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
                if (isNullable)
                {
                    return (T)Convert.ChangeType(output.Value, Nullable.GetUnderlyingType(type));
                }
                else
                {
                    return (T)Convert.ChangeType(output.Value, type);
                }
            }
            return default(T);
        }
    }
}
