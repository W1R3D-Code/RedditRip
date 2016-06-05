using CommandLine;

namespace RedditRip
{
    internal class Options
    {
        [Option('d', "destination", Required = true, HelpText = "Destination Directory for downloads")]
        public string Destination { get; set; }

        [Option('s', "subreddits", Required = false, HelpText = "Restrict search to a comma seperated list of Subreddits")]
        public string Subreddits { get; set; }

        [Option('f', "filter", Required = false, HelpText = "Filter to apply to post Titles, supports Regular Expressions and is case insensitive")]
        public string Filter { get; set; }

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

        [Option('e', "exportLinks", DefaultValue = false, Required = false, HelpText = "Do not download links, just export them to file.")]
        public bool ExportLinks { get; set; }

        [Option('i', "import", Required = false, HelpText = "Import a previous list of links to download. Will ignore all filters")]
        public string ImportFrom { get; set; }

        [Option('v', "verbose", DefaultValue = false, HelpText = "Print more details during execution.")]
        public bool Verbose { get; set; }
    }
}