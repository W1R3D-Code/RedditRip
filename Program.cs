using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedditRip
{
    internal class Program
    {
        private static int allPosts;
        private static int threadLimit = 1;
        private static int errorLimit = 6;

        private static void Main(string[] args)
        {
            var reddit = new Reddit();

            if (args.Count() < 4)
            {
                OutputLine("Invalid arguments");
                return;
            }

            OutputLine($": Logging into Reddit...");
            var user = reddit.LogIn(args[0], args[1]);

            var subReddits = args[2].Split(',');

            var outputPath = args[3].TrimEnd('\\').Trim();

            //int.TryParse(args[4] ?? "1", out threadLimit);

            if (!subReddits.Any() || user == null)
            {
                OutputLine("Failed to log into Reddit.");
                return;
            }

            OutputLine($": Retrieving Subreddit....");

            Task.WaitAll(
                subReddits.Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(
                        sub =>
                            Task.Factory.StartNew(
                                () => DownloadImgurLinksFromSub(reddit, sub, threadLimit, outputPath, errorLimit)))
                    .ToArray());
        }

        private static void DownloadImgurLinksFromSub(Reddit reddit, string sub, int threadLimit, string outputPath,
            int errorLimit)
        {
            var subreddit = reddit.GetSubreddit(sub);

            OutputLine($"Retrieving list of Posts with Imgur links....");

            allPosts = 0;

            //TODO:: Fix hacky hacky way of getting 'as many posts as possible' to simply get them all
            //The first time I did this I was using the search method, searching all time (well launch of reddit to now)
            //  using search like this only ever returned ~950 posts however, so I took this extreamly hacky approach
            //  Since I use the post ID in the file name, I check if we have already downloaded the post before doing so.

            //Top - All Yime
            var listing = subreddit.GetTop(FromTime.All).GetEnumerator();
            DownloadListing(listing, threadLimit, outputPath, errorLimit);

            //Top - Month
            listing = subreddit.GetTop(FromTime.Month).GetEnumerator();
            DownloadListing(listing, threadLimit, outputPath, errorLimit);

            //Top - Week
            listing = subreddit.GetTop(FromTime.Week).GetEnumerator();
            DownloadListing(listing, threadLimit, outputPath, errorLimit);

            //Hot
            listing = subreddit.Hot.GetEnumerator();
            DownloadListing(listing, threadLimit, outputPath, errorLimit);

            //New
            listing = subreddit.New.GetEnumerator();
            DownloadListing(listing, threadLimit, outputPath, errorLimit);
        }

        private static void DownloadListing(IEnumerator<Post> listing, int threadLimit, string outputPath,
            int errorLimit)
        {
            var erroCount = 0;
            try
            {
                while (listing.MoveNext())
                {
                    //Only try imgur links
                    if (!listing.Current.Domain.Contains("imgur"))
                    {
                        OutputLine($"Skipping {listing.Current.Url}");
                        continue;
                    }

                    var posts = new List<Post> { listing.Current };
                    allPosts++;

                    for (var i = 1; i < threadLimit; i++)
                    {
                        if (!listing.MoveNext())
                            break;

                        posts.Add(listing.Current);
                        allPosts++;
                    }

                    OutputLine($": Returned: {posts.Count} posts.");
                    Console.WriteLine();

                    for (var i = 0; i < posts.Count; i = i + threadLimit)
                    {
                        var downloads = new List<Task>();

                        for (var j = 0; j < threadLimit; j++)
                        {
                            if (posts.Count <= (i + j))
                                break;

                            var post = posts[i + j];
                            downloads.Add(Task.Factory.StartNew(() => GetImgurImageOrAlbumFromUrl(post, outputPath)));
                        }
                        Task.WaitAll(downloads.ToArray());
                    }
                    OutputLine($": Running Total: {allPosts} posts.");
                }
            }
            catch (Exception e)
            {
                OutputLine($"Error: {e.Message}");
                erroCount++;
                if (erroCount > errorLimit)
                {
                    throw;
                }
            }
        }

        private static void GetImgurImageOrAlbumFromUrl(Post post, string outputPath)
        {
            var localCount = 1;
            var url = post.Url.ToString().Replace(".gifx", ".gif");
            var name = Path.GetInvalidFileNameChars()
                .Aggregate(post.AuthorName, (current, c) => current.Replace(c, '-'));

            var filepath = outputPath + "\\" + post.SubredditName + "\\" + name;
            //localCount = GetLocalCount(filepath, localCount);

            var dir = new DirectoryInfo(filepath);
            dir.Create();

            if (!Directory.GetFiles(filepath, $"{post.Id}*", SearchOption.TopDirectoryOnly).Any())
            {
                var filename = filepath + $"\\{post.Id}";

                var extention = GetExtention(url);

                if (!string.IsNullOrEmpty(extention))
                {
                    OutputLine($"\tDownloading {url}");
                    if (!DownloadLink(url, filename, localCount))
                    {
                        OutputLine($"\tError downloading {url}");
                        return;
                    }
                }

                string htmlString;
                if (GetHtml(url, out htmlString)) return;

                var regPattern = new Regex(@"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>",
                    RegexOptions.IgnoreCase);
                var matchImageLinks = regPattern.Matches(htmlString);

                OutputLine($"\tFound {matchImageLinks.Count} image(s) from link.");

                foreach (Match m in matchImageLinks)
                {
                    var imgurl = m.Groups[1].Value;
                    if (imgurl.Contains("imgur.com"))
                    {
                        if (!DownloadLink(imgurl, filename, localCount))
                        {
                            OutputLine($"\tError downloading {imgurl}");
                            return;
                        }

                        localCount++;
                    }
                }
            }
            else
            {
                OutputLine($"Skipping Post, already downloaded: {post.Id}");
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
                OutputLine($"\tError loading album {url}: {e.Message}");
                ;
                return true;
            }
            return false;
        }

        private static int GetLocalCount(string filename, int localCount)
        {
            var dir = new DirectoryInfo(filename);
            dir.Create();
            try
            {
                var latestFile = dir.GetFiles()
                    .OrderByDescending(
                        x =>
                            int.Parse(
                                Regex.Match(
                                    Path.GetFileNameWithoutExtension(x.Name)
                                        .Substring(Path.GetFileNameWithoutExtension(x.Name).LastIndexOf('_')), @"\d+")
                                    .Value))
                    .FirstOrDefault();

                if (latestFile != null &&
                    int.TryParse(
                        Regex.Match(
                            Path.GetFileNameWithoutExtension(latestFile.Name)
                                .Substring(Path.GetFileNameWithoutExtension(latestFile.Name).LastIndexOf('_')), @"\d+")
                            .Value, out localCount))
                {
                    localCount++;
                }
            }
            catch
            {
                localCount++;
            }
            return localCount;
        }

        private static bool DownloadLink(string url, string filenameBase, int filenameNumber)
        {
            try
            {
                var domain = url.Contains("i.imgur.com") ? "i.imgur.com" : url.Contains("imgur.com") ? "imgur.com" : "";

                if (string.IsNullOrWhiteSpace(domain))
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
                OutputLine($"Failed to download link {url}: {e.Message}");
                return false;
            }

            return true;
        }

        private static string GetExtention(string imgurl)
        {
            var extention = (imgurl.Contains('.') && imgurl.LastIndexOf('.') > imgurl.LastIndexOf('/') &&
                             imgurl.LastIndexOf('.') < (imgurl.Length - 2))
                ? imgurl.Substring(imgurl.LastIndexOf('.'))
                : ".jpg";

            if (extention.Contains('?'))
                extention = extention.Substring(0, extention.IndexOf('?'));
            if (extention.Contains('#'))
                extention = extention.Substring(0, extention.IndexOf('#'));
            return extention;
        }

        private static void OutputLine(string message)
        {
            Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}: {message}");
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: : {message}");
        }
    }
}