using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedditEar
{
    public class KeywordFoundEventArgs : EventArgs
    {
        public string Keyword { get; set; }
        public string Title { get; set; }
        public Uri Url { get; set; }
    }

    public interface IKeywordWorker
    {
        event EventHandler KeywordFound;

        Task Process(string subredditName, HashSet<string> keywords, CancellationToken cancelationToken);
    }
}
