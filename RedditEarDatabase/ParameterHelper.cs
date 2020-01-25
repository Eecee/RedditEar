using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace RedditEarDatabase
{
    public enum DbConnetionType
    {
        Sqlite
    }

    public class ConnectionHelper
    {
        public DbConnection GetDbConnection(string connectionString, DbConnetionType type)
        {
            switch (type)
            {
                case DbConnetionType.Sqlite:
                    return new SqliteConnection(connectionString);

            }
            return null;
        }
    }

    public class ParameterHelper
    {
        public DbParameter GetParameter(string parameterName, object value, DbConnetionType databaseType)
        {
            switch (databaseType)
            {
                case DbConnetionType.Sqlite:
                {
                    var sqliteParameter = new SqliteParameter
                    {
                        ParameterName = parameterName,
                        Value = value
                    };
                    return sqliteParameter;
                }

                default:
                    return null;
            }
        }
    }
}
