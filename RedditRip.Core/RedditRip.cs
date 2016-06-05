using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using log4net.Core;

namespace RedditRip.Core
{
    public class RedditRip
    {
        public ILog Log { get; set; }

        private int _countAllPosts;
        private int _perSubThreadLimit = 5;
        private const int PostQueryErrorLimit = 6;
        private readonly bool _allAuthorsPosts;
        private string _filter;
        private readonly bool _onlyNsfw;
        private readonly bool _verboseLogging;
        private readonly bool _nsfw;

        private const string SearchUrl = "/r/{0}/search.json?q={1}&restrict_sr=on&sort={2}&t={3}";
        private const string SearchUrlDate = "/r/{0}/search.json?q=timestamp:{1}..{2}&restrict_sr=on&sort={3}&syntax=cloudsearch";

        public RedditRip(string filter, bool allAuthorsPosts, bool nsfw, bool onlyNsfw, bool verboseLogging)
        {
            _verboseLogging = verboseLogging;
            _allAuthorsPosts = allAuthorsPosts;
            _onlyNsfw = onlyNsfw;
            _filter = filter;
            _nsfw = nsfw;
            Log = LogManager.GetLogger(Assembly.GetEntryAssembly().ManifestModule.Name);
        }

        public List<ImageLink> GetImgurLinksFromSubReddit(Reddit reddit, string sub, string outputPath)
        {
            Subreddit subreddit = null;
            var links = new List<ImageLink>();

            if (!string.IsNullOrWhiteSpace(sub))
            {
                try
                {
                    subreddit = reddit.GetSubreddit(sub);
                    OutputLine($"{subreddit.Name}: Begining Search...");
                }
                catch (Exception e)
                {
                    OutputLine($"Error connecting to reddit: {e.Message}");
                    return links;
                }
            }

            _countAllPosts = 0;

            if (_filter == null) _filter = string.Empty;

            var search = !string.IsNullOrWhiteSpace(sub)
                ? subreddit?.Search(_filter)
                : reddit.Search<Post>(_filter);

            var listings = search?.GetEnumerator();

            links = CombineLinkLists(GetImagesFromListing(reddit, listings, outputPath), links);

            return links;
        }

        private List<ImageLink> GetImagesFromListing(Reddit reddit, IEnumerator<Post> listing, string outputPath)
        {
            var erroCount = 0;
            var posts = new List<Post>();
            var links = new List<ImageLink>();
            var users = new HashSet<string>();
            var processedUsers = new HashSet<string>();
            try
            {
                while (listing.MoveNext())
                {
                    Console.WriteLine();
                    for (var i = 0; i < _perSubThreadLimit; i++)
                    {
                        if (_allAuthorsPosts)
                        {
                            if (!users.Contains(listing.Current.AuthorName) && !processedUsers.Contains(listing.Current.AuthorName))
                            {
                                OutputLine($"Adding user to batch: {listing.Current.AuthorName}", true);
                                users.Add(listing.Current.AuthorName);
                                i = users.Count - 1;
                            }
                        }
                        else
                        {
                            if (!listing.Current.Domain.Contains("imgur.com") || (!_nsfw && listing.Current.NSFW) ||
                                (_onlyNsfw && !listing.Current.NSFW))
                            {
                                var suffix = listing.Current.NSFW ? " NSFW" : listing.Current.Url.DnsSafeHost;
                                OutputLine($"Skipping non-imgur link: {listing.Current.Url} ({suffix})", true);
                                continue;
                            }

                            posts.Add(listing.Current);
                            _countAllPosts++;
                        }

                        if (!listing.MoveNext())
                            break;
                    }

                    foreach (var user in users)
                    {
                        OutputLine($"Getting all posts for user: {user}", true);
                        var userPosts =
                            reddit.GetUser(user).Posts.OrderByDescending(post => post.Score)
                                .Where(post => post.Url.DnsSafeHost.Contains("imgur.com"));

                        if (_onlyNsfw)
                            userPosts = userPosts.Where(x => x.NSFW);

                        if (!_nsfw)
                            userPosts = userPosts.Where(x => !x.NSFW);

                        foreach (var userPost in userPosts)
                        {
                            if (posts.Exists(x => x.Url == userPost.Url) || links.Exists(x => x.Url == userPost.Url.ToString())) continue;

                            posts.Add(userPost);
                            _countAllPosts++;
                        }

                        processedUsers.Add(user);
                    }

                    users = new HashSet<string>();
                    OutputLine($"Batch returned: {posts.Count} posts.", true);

                    for (var i = 0; i < posts.Count; i = i + _perSubThreadLimit)
                    {
                        var tasks = new List<Task>();

                        for (var j = 0; j < _perSubThreadLimit; j++)
                        {
                            if (posts.Count <= (i + j))
                                break;

                            var post = posts[i + j];
                            tasks.Add(Task<List<ImageLink>>.Factory.StartNew(
                                () => GetImgurImageOrAlbumFromUrl(post, outputPath)));
                        }
                        Task.WaitAll(tasks.ToArray());

                        links = tasks.Cast<Task<List<ImageLink>>>()
                            .Aggregate(links, (current, task) => CombineLinkLists(task.Result, current));
                        OutputLine($"Total Links found: {links.Count()}");
                    }

                    posts = new List<Post>();
                }
            }
            catch (IndexOutOfRangeException e)
            {
                return links;
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
            finally
            {
                OutputLine($"Running Total: {_countAllPosts} posts processed.");
            }

            return links;
        }

        private List<ImageLink> CombineLinkLists(IEnumerable<ImageLink> results, List<ImageLink> links)
        {
            foreach (var link in results)
            {
                if (links.Exists(x => x.Url == link.Url))
                    OutputLine($"Link already obtained: {link.Url} (XPost {link.Post.Url})", true);
                else
                    links.Add(link);
            }

            return links;
        }

        private List<ImageLink> GetImgurImageOrAlbumFromUrl(Post post, string outputPath)
        {
            var links = new List<ImageLink>();

            OutputLine($"\tGetting links from post: {post.Title}", true);

            var url = new Uri(post.Url.ToString()).GetLeftPart(UriPartial.Path).Replace(".gifv", ".gif");

            url = url.StartsWith("http://") ? url : "http://" + url.Substring(url.IndexOf(post.Url.DnsSafeHost, StringComparison.Ordinal));

            var name = Path.GetInvalidFileNameChars()
                .Aggregate(post.AuthorName, (current, c) => current.Replace(c, '-'));

            var filepath = outputPath + "\\";

            if (_allAuthorsPosts)
                filepath += post.AuthorName;
            else
                filepath += post.SubredditName + "\\" + name;

            var filename = filepath + $"\\{name}_{post.SubredditName}_{post.Id}";

            var extention = GetExtention(url);

            if (!string.IsNullOrEmpty(extention))
            {
                OutputLine($"\tAdding Link {url}", true);
                links.Add(new ImageLink(post, url, filename));
                return links;
            }

            string htmlString;
            if (!GetHtml(url, out htmlString)) return links;

            var caroselAlbum = htmlString.Contains(@"data-layout=""h""");

            if (caroselAlbum)
                if (!GetHtml(url + "/all", out htmlString)) return links;

            var gridAlbum = htmlString.Contains(@"data-layout=""g""");

            if (caroselAlbum && !gridAlbum) return links;

            var regPattern = new Regex(@"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>",
                RegexOptions.IgnoreCase);

            var matchImageLinks = regPattern.Matches(htmlString);

            OutputLine($"\tFound {matchImageLinks.Count} image(s) from link.", true);

            foreach (Match m in matchImageLinks)
            {
                var imgurl = m.Groups[1].Value.Replace(".gifv", ".gif");

                if (!imgurl.Contains("imgur.com")) continue;

                if (gridAlbum)
                    imgurl = imgurl.Remove(imgurl.LastIndexOf('.') - 1, 1);
                var domain = new Uri(imgurl).DnsSafeHost;
                imgurl = imgurl.StartsWith("http://") ? imgurl : "http://" + imgurl.Substring(imgurl.IndexOf(domain, StringComparison.Ordinal));

                links.Add(new ImageLink(post, imgurl, filename));
            }

            return links;
        }

        private bool GetHtml(string url, out string htmlString)
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

        public async void DownloadPost(KeyValuePair<string, List<ImageLink>> post, string destination)
        {
            if (post.Value.Any() && !string.IsNullOrWhiteSpace(destination))
            {
                var subName = post.Value.FirstOrDefault().Post.SubredditName;
                var userName = post.Value.FirstOrDefault().Post.AuthorName;
                var postId = post.Key;
                var filePath = destination + Path.DirectorySeparatorChar + subName + Path.DirectorySeparatorChar +
                               userName;
                var fileNameBase = $"{userName}_{subName}_{postId}";

                var dir = new DirectoryInfo(filePath);
                dir.Create();

                var existsingFiles = Directory.GetFiles(filePath, $"{fileNameBase}*",
                    SearchOption.TopDirectoryOnly).ToList();

                if (existsingFiles.Count() >= post.Value.Count) return;

                var downloads = new Dictionary<string, string>();
                var count = 0;
                foreach (var imageLink in post.Value)
                {
                    var uri = new Uri(imageLink.Url);
                    var domain = uri.DnsSafeHost;
                    if (string.IsNullOrWhiteSpace(domain) || imageLink.Url.Contains("removed.png")) continue;

                    var link = imageLink.Url.StartsWith("http://")
                    ? imageLink.Url
                    : "http://" +
                      imageLink.Url.Substring(imageLink.Url.IndexOf(domain, StringComparison.Ordinal));

                    link = link.Replace(".gifv", ".gif");

                    var extention = GetExtention(link);

                    count++;
                    var fullFilePath = filePath + Path.DirectorySeparatorChar + fileNameBase + "_" +
                                       count.ToString("0000") + extention;

                    if (!existsingFiles.Contains(fullFilePath))
                        await SaveFile(imageLink, fullFilePath, extention, link);
                }
            }
        }

        public async void DownloadLink(ImageLink imageLink, string filenameBase, int filenameNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filenameBase)) return;
                var filepath = Path.GetDirectoryName(filenameBase);
                if (string.IsNullOrWhiteSpace(filepath)) return;

                var dir = new DirectoryInfo(filepath);
                dir.Create();

                if (Directory.GetFiles(filepath, $"*{imageLink.Post.Id}*", SearchOption.TopDirectoryOnly).Any())
                {
                    OutputLine(
                        $"Skipping Post, already downloaded: {"https://reddit.com/r/" + imageLink.Post.SubredditName + imageLink.Post.Id} {imageLink.Post.Url}",
                        true);
                    return;
                }

                var uri = new Uri(imageLink.Url);
                var domain = uri.DnsSafeHost;

                // if the image was removed and was linked directly, removed.png is served up instead
                if (string.IsNullOrWhiteSpace(domain) || imageLink.Url.Contains("removed.png"))
                    return;

                var link = imageLink.Url.StartsWith("http://")
                    ? imageLink.Url
                    : "http://" +
                      imageLink.Url.Substring(imageLink.Url.IndexOf(domain, StringComparison.Ordinal));

                link = link.Replace(".gifv", ".gif");

                var extention = GetExtention(link);

                var filename = filenameBase + "_" + filenameNumber.ToString("0000") + extention;
                filename = filename.Trim('.');

                await SaveFile(imageLink, filename, extention, link);
            }
            catch (Exception e)
            {
                OutputLine($"Failed to download link {imageLink.Url}: {e.Message}", true);
            }
        }

        private async Task SaveFile(ImageLink imageLink, string filename, string extention, string link)
        {
            var tempFilename = Path.GetTempPath() + Path.GetRandomFileName() + extention;
            using (var wc = new WebClient())
            {
                await wc.DownloadFileTaskAsync(new Uri(link), tempFilename);
                OutputLine($"Downloaded: {imageLink.Url} to {filename}", true);

                using (var image = Image.FromFile(tempFilename))
                {
                    //TODO:: refactor to remove code smell - http://www.exiv2.org/tags.html

                    //XPTitle
                    SetImageProperty(image, 40091, Encoding.Unicode.GetBytes(imageLink.Post.Title + char.MinValue));
                    //XPComment
                    SetImageProperty(image, 40092, Encoding.Unicode.GetBytes(link + char.MinValue));
                    //XPAuthor
                    SetImageProperty(image, 40093, Encoding.Unicode.GetBytes(imageLink.Post.AuthorName + char.MinValue));
                    //XPKeywords
                    SetImageProperty(image, 40094,
                        Encoding.Unicode.GetBytes(imageLink.Post.SubredditName + ";" + imageLink.Post.AuthorName +
                                                  ";" + imageLink.Post.AuthorFlairText + ";" + imageLink.Post.Domain +
                                                  char.MinValue));
                    //Save to desination
                    image.Save(filename);
                }
            }

            //Delete temp file after web client has been disposed (makes sure no handles to file left over)
            File.Delete(tempFilename);
        }

        private static void SetImageProperty(Image image, int propertyId, byte[] value)
        {
            var prop = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
            prop.Id = propertyId;
            prop.Value = value;
            prop.Len = prop.Value.Length;
            prop.Type = 1;
            image.SetPropertyItem(prop);
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

        private void OutputLine(string message, bool verboseMessage = false)
        {
            if (verboseMessage && !_verboseLogging) return;
            
            Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}: {message}");
            if (verboseMessage)
            {
                Log.Debug(message);
            }
            else
            {
                Log.Info(message);
            }
        }
    }
}