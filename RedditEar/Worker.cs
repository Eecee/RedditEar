using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RedditEarDatabase;

namespace RedditEar
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptions<AppSettings> _settings;
        private IKeywordWorker _keywordWorker;
        private Database _db;

        public Worker(ILogger<Worker> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;
            _settings = settings;

            _keywordWorker = new RedditWorker(settings.Value.RedditBotSettings);
            _keywordWorker.KeywordFound += OnKeywordFound;

            _db = new Database(settings.Value.DatabaseSettings.ConnectionString, DbConnetionType.Sqlite);
            _db.Init();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _db.KeywordsUrlTable.Delete(DateTime.UtcNow);

                HashSet<string> keywords = _db.KeywordsTable.Select();
                HashSet<string> sources = _db.SourcesTable.Select();

                foreach (var s in sources)
                {
                    await _keywordWorker.Process(s, keywords, stoppingToken);
                }

                await Task.Delay(100000, stoppingToken);
            }
        }

        private void OnKeywordFound(object sender, EventArgs e)
        {
            KeywordFoundEventArgs kfa = e as KeywordFoundEventArgs;
            if (kfa == null) return;

            // TODO: check urls for multiple keyword hits
            _db.KeywordsUrlTable.Insert(DateTime.UtcNow.Date, kfa.Keyword, kfa.Url.ToString(), kfa.Title);
        }
    }
}
