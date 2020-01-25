using System;
using System.Data.Common;

namespace RedditEarDatabase
{
    public class Database : IDisposable
    {
        private string _connectionString;
        private DbConnection _connection;
        private DbConnetionType _databaseType;
        private ParameterHelper _parameterHelper;
        private KeywordsUrlTable _kwuTable;

        public KeywordsUrlTable KeywordsUrlTable { get { return _kwuTable; } }

        public Database(string connectionString, DbConnetionType databaseType)
        {
            _connectionString = connectionString;
            _databaseType = databaseType;
            _parameterHelper = new ParameterHelper();
        }

        public void Init()
        {
            var connectionHelper = new ConnectionHelper();
            _connection = connectionHelper.GetDbConnection(_connectionString, _databaseType);

            _kwuTable = new KeywordsUrlTable(_connection, _parameterHelper, _databaseType);
        }

        public void Dispose()
        {
            _connection.CheckClose();
        }
    }
}
