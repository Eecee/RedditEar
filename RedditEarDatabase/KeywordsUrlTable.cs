using System;
using System.Data.Common;

namespace RedditEarDatabase
{
    public class KeywordsUrlTable
    {
        private DbConnection _connection;
        private ParameterHelper _parameterHelper;
        private DbConnetionType _databaseType;

        public KeywordsUrlTable(DbConnection connection, ParameterHelper parameterHelper, DbConnetionType databaseType)
        {
            _connection = connection;
            _parameterHelper = parameterHelper;
            _databaseType = databaseType;
        }

        public void Select(DateTime date)
        {
            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                if (date == null) command.CommandText = "SELECT Timestamp, Keywords, Url, Title FROM KeywordsUrl";
                else
                {
                    command.CommandText = "SELECT Timestamp, Keywords, Url FROM KeywordsUrl WHERE Date = @Date";

                    var dParam = _parameterHelper.GetParameter("@Date", date.Date.ToString(), _databaseType);
                    command.Parameters.Add(dParam);
                }

                var result = command.ExecuteReader();

                // TODO: repack data
                while (result.Read())
                {
                    Console.WriteLine(string.Format("Date: {0} Keywords: {1} Url: {2} Title: {3}",
                        result.GetString(0), result.GetString(1), result.GetString(2), result.GetString(3)));
                }
            }
        }

        public void Delete(DateTime date)
        {
            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                if (date == null) _connection.ExecuteNonQuery("DELETE FROM KeywordsUrl");
                else
                {
                    var dParam = _parameterHelper.GetParameter("@Date", date.Date.ToString(), _databaseType);
                    command.Parameters.Add(dParam);

                    command.CommandText = "DELETE FROM KeywordsUrl WHERE Date = @Date";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Insert(DateTime date, string keywords, string url, string title)
        {
            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                var dParam = _parameterHelper.GetParameter("@Date", date.Date.ToFileTimeUtc(), _databaseType);
                command.Parameters.Add(dParam);

                var kwParam = _parameterHelper.GetParameter("@Keywords", keywords, _databaseType);
                command.Parameters.Add(kwParam);

                var urlParam = _parameterHelper.GetParameter("@Url", url, _databaseType);
                command.Parameters.Add(urlParam);

                var tParam = _parameterHelper.GetParameter("@Title", title, _databaseType);
                command.Parameters.Add(tParam);

                command.CommandText = "INSERT INTO KeywordsUrl (Date, Keywords, Url, Title) VALUES (@Date, @Keywords, @Url, @Title);";
                command.ExecuteNonQuery();
            }
        }
    }
}
