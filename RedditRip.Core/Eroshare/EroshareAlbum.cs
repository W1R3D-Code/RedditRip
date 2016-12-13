using System.Collections.Generic;

namespace RedditRip.Core.Eroshare
{
    public class EroshareAlbum
    {
        public string id { get; set; }
        public object title { get; set; }
        public string slug { get; set; }
        public string created_at { get; set; }
        public int views { get; set; }
        public int gender_male { get; set; }
        public int gender_female { get; set; }
        public bool secret { get; set; }
        public RedditSubmission reddit_submission { get; set; }
        public string url { get; set; }
        public List<Item> items { get; set; }
    }
}
