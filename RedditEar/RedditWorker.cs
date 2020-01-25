using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using RedditSharp;
using RedditSharp.Things;

namespace RedditEar
{
    public class RedditWorker : IKeywordWorker
    {
        public event EventHandler KeywordFound;
        private BotWebAgent _botWebAgent;
        private RedditBotSettings _settings;

        public RedditWorker(RedditBotSettings settings)
        {
            _settings = settings;
            _botWebAgent = new BotWebAgent(_settings.Username, _settings.Password, _settings.ClientId, _settings.ClientSecret, _settings.RedirectUri);
        }

        public async Task Process(string subredditName, HashSet<string> keywords, CancellationToken cancelationToken)
        {
            var reddit = new Reddit(_botWebAgent, false);
            var subreddit = await reddit.GetSubredditAsync(subredditName);
            var topForDay = subreddit.GetTop(FromTime.Day, _settings.MaxPosts);

            await topForDay.ForEachAsync(post =>
            {
                if (cancelationToken.IsCancellationRequested) return;
                if (string.IsNullOrEmpty(post.SelfText)) return;

                foreach (var kw in keywords)
                {
                    if (cancelationToken.IsCancellationRequested) return;
                    if (post.SelfText.IndexOf(kw, StringComparison.OrdinalIgnoreCase) == -1) continue;

                    KeywordFoundEventArgs args = new KeywordFoundEventArgs { Keyword = kw, Title = post.Title, Url = post.Url };
                    EventHandler handler = KeywordFound;
                    handler?.Invoke(this, args);
                }
            });
        }
    }
}
