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
                HashSet<string> keywords = new HashSet<string>();
                keywords.Add("Trump");
                keywords.Add("war");

                await _keywordWorker.Process("/r/wallstreetbets", keywords, stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void OnKeywordFound(object sender, EventArgs e)
        {
            KeywordFoundEventArgs kfa = e as KeywordFoundEventArgs;
            if (kfa == null) return;

            // TODO: check urls for multiple keyword hits
            _logger.LogInformation("Keyword found: Keyword = {Keyword} Title = {Title} Url = {Url}", kfa.Keyword, kfa.Title, kfa.Url);
            _db.KeywordsUrlTable.Insert(DateTime.UtcNow.Date, kfa.Keyword, kfa.Url.ToString(), kfa.Title);
        }
    }
}
