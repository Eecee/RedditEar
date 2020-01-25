using System;

namespace RedditEar
{
    public class RedditBotSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public int MaxPosts { get; set; }
    }

    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
    }

    public class AppSettings
    {
        public RedditBotSettings RedditBotSettings { get; set; }
        public DatabaseSettings DatabaseSettings { get; set; }
    }
}
