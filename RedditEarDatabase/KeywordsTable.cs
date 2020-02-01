using System;
using System.Data.Common;
using System.Collections.Generic;

namespace RedditEarDatabase
{
    public class KeywordsTable
    {
        private DbConnection _connection;
        private ParameterHelper _parameterHelper;
        private DbConnetionType _databaseType;

        private const string KEYWORD_PARAM = "@Keyword";

        public KeywordsTable(DbConnection connection, ParameterHelper parameterHelper, DbConnetionType databaseType)
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
                if (top == -1) command.CommandText = "SELECT Id, Keyword FROM Keywords";
                else command.CommandText = string.Format("SELECT TOP {0} Id, Keyword FROM Keywords", top);

                var result = command.ExecuteReader();

                while (result.Read())
                {
                    res.Add(result.GetString(1));
                }
            }

            return res;
        }

        public void Delete(string keyword)
        {
            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                var dParam = _parameterHelper.GetParameter(KEYWORD_PARAM, keyword, _databaseType);
                command.Parameters.Add(dParam);

                command.CommandText = string.Format("DELETE FROM Keywords WHERE Keyword = {0}", KEYWORD_PARAM);
                command.ExecuteNonQuery();
            }
        }

        public void Insert(string keyword)
        {
            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                var kwParam = _parameterHelper.GetParameter(KEYWORD_PARAM, keyword, _databaseType);
                command.Parameters.Add(kwParam);

                command.CommandText = string.Format("INSERT INTO Keywords (Keyword) VALUES ({0});", KEYWORD_PARAM);
                command.ExecuteNonQuery();
            }
        }
    }
}
