using System;
using System.Data.Common;
using System.Collections.Generic;

namespace RedditEarDatabase
{
    public class SourcesTable
    {
        private DbConnection _connection;
        private ParameterHelper _parameterHelper;
        private DbConnetionType _databaseType;

        private const string SOURCE_PARAM = "@Source";

        public SourcesTable(DbConnection connection, ParameterHelper parameterHelper, DbConnetionType databaseType)
        {
            _connection = connection;
            _parameterHelper = parameterHelper;
            _databaseType = databaseType;
        }

        public HashSet<string> Select(int top = -1)
        {
            HashSet<string> res = new HashSet<string>();

            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                if (top == -1) command.CommandText = "SELECT Id, Source FROM Sources";
                else command.CommandText = string.Format("SELECT TOP {0} Id, Keyword FROM Sources", top);

                var result = command.ExecuteReader();

                while (result.Read())
                {
                    res.Add(result.GetString(1));
                }
            }

            return res;
        }

        public void Delete(string source)
        {
            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                var dParam = _parameterHelper.GetParameter(SOURCE_PARAM, source, _databaseType);
                command.Parameters.Add(dParam);

                command.CommandText = string.Format("DELETE FROM Sources WHERE Source = {0}", SOURCE_PARAM);
                command.ExecuteNonQuery();
            }
        }

        public void Insert(string source)
        {
            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                var kwParam = _parameterHelper.GetParameter(SOURCE_PARAM, source, _databaseType);
                command.Parameters.Add(kwParam);

                command.CommandText = string.Format("INSERT INTO Sources (Source) VALUES ({0});", SOURCE_PARAM);
                command.ExecuteNonQuery();
            }
        }
    }
}
