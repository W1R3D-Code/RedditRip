using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine.Text;

namespace RedditRip
{
    internal class Program
    {
        //TODO:: Sort out multiple threads per sub reddit
        //I had issues with this when incrementing file numbers so set to 1 for now
        private const int PerSubThreadLimit = 1; 
        private const int PostQueryErrorLimit = 6;

        private static int countAllPosts;
        private static Options options;

        private static void Main(string[] args)
        {
            var reddit = new Reddit();

            options = new Options();
            var subReddits = new List<string>();
            var destination = string.Empty;


            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                subReddits.AddRange(options.Subreddits.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)));

                if (!subReddits.Any())
                {
                    OutputLine("Invalid Subreddits");
                    return;
                }

                destination = options.Destination.TrimEnd('\\').Trim();
                
                if (!string.IsNullOrWhiteSpace(options.Username))
                {
                    if (!string.IsNullOrWhiteSpace(options.Password))
                    {
                        OutputLine($"Logging into Reddit as {options.Username}");
                        reddit.LogIn(options.Username, options.Password);
                    }
                    else
                    {
                        OutputLine("Password can not be empty when supplying a username.");
                        return;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(options.Password))
                {
                    OutputLine("Username can not be empty when supplying a password.");
                    return;
                }

                try
                {
                    var dir = new DirectoryInfo(destination);

                    if (!dir.Exists)
                    {
                        OutputLine("Destination path not found, creating folder.", true);
                       dir.Create();
                    }
                }
                catch (Exception e)
                {
                    OutputLine($"Invalid Destination Path: {e.Message}. Please enter a valid path.");
                    return;
                }
            }
            else
            {
                Console.WriteLine(HelpText.AutoBuild(options));
                return;
            }

            try
            {
                Task.WaitAll(
                subReddits.Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(sub => Task.Factory.StartNew(() => DownloadImgurLinksFromSub(reddit, sub, destination)))
                    .ToArray());
            }
            catch (Exception e)
            {
                OutputLine($"An Unexpected Error Occured: {e.Message}");
            }
        }

        private static void DownloadImgurLinksFromSub(Reddit reddit, string sub, string outputPath)
        {
            Subreddit subreddit;

            try
            {
                subreddit = reddit.GetSubreddit(sub);
            }
            catch (Exception e)
            {
                OutputLine($"Error connecting to reddit: {e.Message}");
                return;
            }

            OutputLine($"{subreddit.Name}: Begining Search...");

            countAllPosts = 0;

            //TODO:: Fix hacky hacky way of getting 'as many posts as possible' to simply get them all
            //The first time I did this I was using the search method, searching all time (well launch of reddit to now)
            //  using search like this only ever returned ~950 posts however, so I took this extreamly hacky approach
            //  Since I use the post ID in the file name, I check if we have already downloaded the post before doing so.

            //Top - All Time
            var listing = subreddit.GetTop(FromTime.All).GetEnumerator();
            DownloadListing(listing, outputPath);

            //Top - Month
            listing = subreddit.GetTop(FromTime.Month).GetEnumerator();
            DownloadListing(listing, outputPath);

            //Top - Week
            listing = subreddit.GetTop(FromTime.Week).GetEnumerator();
            DownloadListing(listing, outputPath);

            //Hot
            listing = subreddit.Hot.GetEnumerator();
            DownloadListing(listing, outputPath);

            //New
            listing = subreddit.New.GetEnumerator();
            DownloadListing(listing, outputPath);
        }

        private static void DownloadListing(IEnumerator<Post> listing, string outputPath)
        {
            var erroCount = 0;
            try
            {
                while (listing.MoveNext())
                {
                    Console.WriteLine();
                    var posts = new List<Post>();
                    for (var i = 0; i < PerSubThreadLimit; i++)
                    {
                        if (options.AllAuthorsPosts)
                        {
                            OutputLine($"Getting all posts for user: {listing.Current.AuthorName}", true);
                            var userPosts =
                                listing.Current.Author.Posts.OrderByDescending(post => post.Score)
                                    .Where(
                                        post =>
                                            post.Id != listing.Current.Id && post.Url.DnsSafeHost.Contains("imgur.com")).ToList();

                            if (userPosts.Any()) i--;
                            foreach (var userPost in userPosts)
                            {
                                if ((userPost.NSFW && !options.NSFW) || (!userPost.NSFW && options.OnlyNSFW))
                                {
                                    var suffix = userPost.NSFW ? "NSFW" : "non-NSFW";
                                    OutputLine($"Skipping {userPost.Url} ({suffix})", true);
                                    continue;
                                }

                                posts.Add(userPost);
                                countAllPosts++;
                                i++;
                            }
                        }
                        else
                        {
                            if (!listing.Current.Domain.Contains("imgur.com") || (!options.NSFW && listing.Current.NSFW) ||
                                (options.OnlyNSFW && !listing.Current.NSFW))
                            {
                                var suffix = listing.Current.NSFW ? " NSFW" : listing.Current.Url.DnsSafeHost;
                                OutputLine($"Skipping {listing.Current.Url} ({suffix})", true);
                                continue;
                            }

                            posts.Add(listing.Current);
                            countAllPosts++;
                        }

                        if (!listing.MoveNext())
                            break;
                    }

                    OutputLine($"Query returned: {posts.Count} posts.", true);

                    for (var i = 0; i < posts.Count; i = i + PerSubThreadLimit)
                    {
                        var downloads = new List<Task>();

                        for (var j = 0; j < PerSubThreadLimit; j++)
                        {
                            if (posts.Count <= (i + j))
                                break;

                            var post = posts[i + j];
                            downloads.Add(Task.Factory.StartNew(() => GetImgurImageOrAlbumFromUrl(post, outputPath)));
                        }
                        Task.WaitAll(downloads.ToArray());
                    }
                    OutputLine($"Running Total: {countAllPosts} posts processed.");
                }
            }
            catch (Exception e)
            {
                OutputLine($"Error: {e.Message}");
                erroCount++;
                if (erroCount > PostQueryErrorLimit)
                {
                    throw;
                }
            }
        }

        private static void GetImgurImageOrAlbumFromUrl(Post post, string outputPath)
        {
            var localCount = 1;
            var url = new Uri(post.Url.ToString()).GetLeftPart(UriPartial.Path);

            var name = Path.GetInvalidFileNameChars()
                .Aggregate(post.AuthorName, (current, c) => current.Replace(c, '-'));

            var filepath = outputPath + "\\";

            if (options.AllAuthorsPosts)
            {
                filepath += post.AuthorName;
            }
            else
            {
                filepath += post.SubredditName + "\\" + name;
            }

            var dir = new DirectoryInfo(filepath);
            dir.Create();

            if (!Directory.GetFiles(filepath, $"*{post.Id}*", SearchOption.TopDirectoryOnly).Any())
            {
                var filename = filepath + $"\\{name}_{post.SubredditName}_{post.Id}";
                
                var extention = GetExtention(url);

                if (!string.IsNullOrEmpty(extention))
                {
                    OutputLine($"\tDownloading {url}", true);
                    if (!DownloadLink(url, filename, localCount))
                    {
                        OutputLine($"\tError downloading {url}", true);
                        return;
                    }
                }

                string htmlString;
                if (!GetHtml(url, out htmlString)) return;

                var caroselAlbum = htmlString.Contains(@"data-layout=""h""");
                bool gridAlbum;

                if (caroselAlbum)
                    if (!GetHtml(url + "/all", out htmlString)) return;

                gridAlbum = htmlString.Contains(@"data-layout=""g""");

                if (caroselAlbum && !gridAlbum) return;

                var regPattern = new Regex(@"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>",
                    RegexOptions.IgnoreCase);

                var matchImageLinks = regPattern.Matches(htmlString);

                OutputLine($"\tFound {matchImageLinks.Count} image(s) from link.", true);

                foreach (Match m in matchImageLinks)
                {
                    var imgurl = m.Groups[1].Value;
                    if (imgurl.Contains("imgur.com"))
                    {
                        if (gridAlbum)
                            imgurl = imgurl.Remove(imgurl.LastIndexOf('.') - 1,1);

                        if (!DownloadLink(imgurl, filename, localCount))
                        {
                            OutputLine($"\tError downloading {imgurl}", true);
                            return;
                        }

                        localCount++;
                    }
                }
            }
            else
            {
                OutputLine($"Skipping Post, already downloaded: {post.Id}", true);
            }
        }

        private static bool GetHtml(string url, out string htmlString)
        {
            htmlString = string.Empty;

            try
            {
                using (var wchtml = new WebClient())
                {
                    htmlString = wchtml.DownloadString(url);
                }
            }
            catch (Exception e)
            {
                OutputLine($"\tError loading album {url}: {e.Message}", true);
                return false;
            }
            return true;
        }

        private static bool DownloadLink(string url, string filenameBase, int filenameNumber)
        {
            try
            {
                var uri = new Uri(url);
                var domain = uri.DnsSafeHost;

                // if the image was removed and was linked directly, removed.png is served up instead
                if (string.IsNullOrWhiteSpace(domain) || url.Contains("removed.png"))
                    return false;

                var link = url.StartsWith("http://")
                    ? url
                    : "http://" +
                      url.Substring(url.IndexOf(domain));

                link = link.Replace(".gifv", ".gif");

                var extention = GetExtention(link);

                var filename = filenameBase + "_" + filenameNumber.ToString().PadLeft(4, '0') + extention;

                using (var wc = new WebClient())
                {
                    OutputLine($"\tDownloading: {url} to {filename}");
                    wc.DownloadFile(link, filename);
                }
            }
            catch (Exception e)
            {
                OutputLine($"Failed to download link {url}: {e.Message}", true);
                return false;
            }

            return true;
        }

        private static string GetExtention(string imgurl)
        {
            var extention = (imgurl.Contains('.') && imgurl.LastIndexOf('.') > imgurl.LastIndexOf('/') &&
                             imgurl.LastIndexOf('.') < (imgurl.Length - 2))
                ? imgurl.Substring(imgurl.LastIndexOf('.'))
                : string.Empty;

            if (extention.Contains('?'))
                extention = extention.Substring(0, extention.IndexOf('?'));
            return extention;
        }
        
        private static void OutputLine(string message, bool verboseMessage = false)
        {
            if (verboseMessage && !options.Verbose) return;
            Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}: {message}");
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: : {message}");
        }
    }
}