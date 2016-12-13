namespace RedditRip.Core.Eroshare
{
    public class RedditSubmission
    {
        public int album_id { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
        public string subreddit { get; set; }
        public int score { get; set; }
        public string author { get; set; }
        public string permalink { get; set; }
        public string url { get; set; }
        public object item_id { get; set; }
        public string title { get; set; }
        public int ups { get; set; }
        public int downs { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int num_comments { get; set; }
    }
}