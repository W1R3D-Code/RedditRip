namespace RedditRip.Core.Eroshare
{
    public class Item
    {
        public string type { get; set; }
        public string id { get; set; }
        public object description { get; set; }
        public string slug { get; set; }
        public string state { get; set; }
        public int conversion_progress { get; set; }
        public int video_duration { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string url_full_protocol_encoded { get; set; }
        public string url_full_protocol { get; set; }
        public string url_full { get; set; }
        public string url_thumb { get; set; }
        public string url_orig { get; set; }
        public string url_mp4 { get; set; }
        public string url_mp4_lowres { get; set; }
        public int position { get; set; }
        public bool is_portrait { get; set; }
        public bool lowres { get; set; }
    }
}