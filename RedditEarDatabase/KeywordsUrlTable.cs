using System;
using System.Data.Common;
using System.Collections.Generic;

namespace RedditEarDatabase
{
    public class KeywordsUrlTable
    {
        public class Record
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Keywords { get; set; }
            public string Url { get; set; }
            public string Title { get; set; }
        }

        private DbConnection _connection;
        private ParameterHelper _parameterHelper;
        private DbConnetionType _databaseType;

        private const string DATE_PARAM = "@Date";
        private const string KEYWORDS_PARAM = "@Keywords";
        private const string URL_PARAM = "@Url";
        private const string TITLE_PARAM = "@Title";

        public KeywordsUrlTable(DbConnection connection, ParameterHelper parameterHelper, DbConnetionType databaseType)
        {
            _connection = connection;
            _parameterHelper = parameterHelper;
            _databaseType = databaseType;
        }

        public List<Record> Select(DateTime date)
        {
            List<Record> records = new List<Record>();

            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                if (date == null) command.CommandText = "SELECT Timestamp, Keywords, Url, Title FROM KeywordsUrl";
                else
                {
                    command.CommandText = string.Format("SELECT Timestamp, Keywords, Url FROM KeywordsUrl WHERE Date = {0}", DATE_PARAM);

                    var dParam = _parameterHelper.GetParameter(DATE_PARAM, date.Date.ToString(), _databaseType);
                    command.Parameters.Add(dParam);
                }

                var result = command.ExecuteReader();

                while (result.Read())
                {
                    Record rec = new Record();
                    rec.Id = result.GetInt32(0);
                    rec.Date = DateTime.FromFileTimeUtc(result.GetInt64(1));
                    rec.Keywords = result.GetString(2);
                    rec.Url = result.GetString(3);
                    rec.Title = result.GetString(4);

                    records.Add(rec);
                }
            }

            return records;
        }

        public void Delete(DateTime date)
        {
            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                if (date == null) _connection.ExecuteNonQuery("DELETE FROM KeywordsUrl");
                else
                {
                    var dParam = _parameterHelper.GetParameter(DATE_PARAM, date.Date.ToFileTimeUtc(), _databaseType);
                    command.Parameters.Add(dParam);

                    command.CommandText = string.Format("DELETE FROM KeywordsUrl WHERE Date = {0}", DATE_PARAM);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteBefore(DateTime date)
        {
            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                if (date == null) _connection.ExecuteNonQuery("DELETE FROM KeywordsUrl");
                else
                {
                    var dParam = _parameterHelper.GetParameter(DATE_PARAM, date.Date.ToFileTimeUtc(), _databaseType);
                    command.Parameters.Add(dParam);

                    command.CommandText = string.Format("DELETE FROM KeywordsUrl WHERE Date < {0}", DATE_PARAM);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Insert(DateTime date, string keywords, string url, string title)
        {
            _connection.CheckOpen();

            using (var command = _connection.CreateCommand())
            {
                var dParam = _parameterHelper.GetParameter(DATE_PARAM, date.Date.ToFileTimeUtc(), _databaseType);
                command.Parameters.Add(dParam);

                var kwParam = _parameterHelper.GetParameter(KEYWORDS_PARAM, keywords, _databaseType);
                command.Parameters.Add(kwParam);

                var urlParam = _parameterHelper.GetParameter(URL_PARAM, url, _databaseType);
                command.Parameters.Add(urlParam);

                var tParam = _parameterHelper.GetParameter(TITLE_PARAM, title, _databaseType);
                command.Parameters.Add(tParam);

                command.CommandText = string.Format("INSERT INTO KeywordsUrl (Date, Keywords, Url, Title) VALUES ({0}, {1}, {2}, {3});",
                    DATE_PARAM, KEYWORDS_PARAM, URL_PARAM, TITLE_PARAM);
                command.ExecuteNonQuery();
            }
        }
    }
}
