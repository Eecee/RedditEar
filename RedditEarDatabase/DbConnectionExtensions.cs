using System;
using System.Data.Common;

namespace RedditEarDatabase
{
    public static class DbConnectionExtensions
    {
        public static int ExecuteNonQuery(this DbConnection connection, string commandText, int timeout = 30)
        {
            var command = connection.CreateCommand();
            command.CommandTimeout = timeout;
            command.CommandText = commandText;
            return command.ExecuteNonQuery();
        }

        public static T ExecuteScalar<T>(this DbConnection connection, string commandText, int timeout = 30) =>
        (T)connection.ExecuteScalar(commandText, timeout);

        private static object ExecuteScalar(this DbConnection connection, string commandText, int timeout)
        {
            var command = connection.CreateCommand();
            command.CommandTimeout = timeout;
            command.CommandText = commandText;
            return command.ExecuteScalar();
        }

        public static DbDataReader ExecuteReader(this DbConnection connection, string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            return command.ExecuteReader();
        }

        public static void CheckOpen(this DbConnection connection)
        {
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
        }

        public static void CheckClose(this DbConnection connection)
        {
            if (connection.State != System.Data.ConnectionState.Closed) connection.Close();
        }
    }
}
