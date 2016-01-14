using CommandLine;

namespace RedditRip
{
    internal class Options
    {
        [Option('s', "subreddits", Required = true, HelpText = "CSV list of Subreddits to download Imgur images from")]
        public string Subreddits { get; set; }

        [Option('f', "filter", Required = true, HelpText = "Filter to apply to post Titles, supports Regular Expressions")]
        public string Filter { get; set; }

        [Option('d', "destination", Required = true, HelpText = "Destination Directory for downloads")]
        public string Destination { get; set; }

        [Option('u', "username", Required = false, HelpText = "Optional. Reddit username")]
        public string Username { get; set; }

        [Option('p', "password", Required = false, HelpText = "Optional. Reddit password")]
        public string Password { get; set; }

        [Option('n', "nsfw", DefaultValue = true, Required = false, HelpText = "Allow NSFW Posts")]
        public bool NSFW { get; set; }

        [Option('x', "onlynsfw", DefaultValue = false, Required = false, HelpText = "Only download NSFW Posts")]
        public bool OnlyNSFW { get; set; }

        [Option('a', "allAuthorsPosts", DefaultValue = false, Required = false, HelpText = "For every post, download all the imgur posts from that user too!")]
        public bool AllAuthorsPosts { get; set; }

        [Option('v', "verbose", DefaultValue = false, HelpText = "Print more details during execution.")]
        public bool Verbose { get; set; }
    }
}