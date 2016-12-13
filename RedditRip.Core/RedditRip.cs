using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using log4net;
using log4net.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RedditRip.Core.Eroshare;
using Image = System.Drawing.Image;

namespace RedditRip.Core
{
    public class RedditRip
    {
        public ILog Log { get; set; }

        private int _perSubThreadLimit = 5;
        private const int PostQueryErrorLimit = 6;
        private readonly bool _allAuthorsPosts;
        private string _filter;
        private readonly bool _onlyNsfw;
        private readonly bool _verboseLogging;
        private readonly bool _nsfw;
        private static string[] videoExtensions = { ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA", ".AVI", ".MP4", ".DIVX", ".WMV" };

        public RedditRip(string filter, bool allAuthorsPosts, bool nsfw, bool onlyNsfw, bool verboseLogging)
        {
            _verboseLogging = verboseLogging;
            _allAuthorsPosts = allAuthorsPosts;
            _onlyNsfw = onlyNsfw;
            _filter = filter;
            _nsfw = nsfw;
            Log = LogManager.GetLogger(Assembly.GetEntryAssembly().ManifestModule.Name);
        }

        public async Task<List<ImageLink>> GetImgurLinksFromSubReddit(Reddit reddit, string sub, SearchRange searchRange, Sorting sortOrder, string outputPath, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
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

            if (_filter == null) _filter = string.Empty;

            var searchTo = DateTime.Now;
            var searchFrom = DateTime.Now;
            switch (searchRange)
            {
                case SearchRange.Today:
                    searchFrom = searchFrom.AddDays(-1);
                    break;
                case SearchRange.Week:
                    searchFrom = searchFrom.AddDays(-7);
                    break;
                case SearchRange.Fortnight:
                    searchFrom = searchFrom.AddDays(-14);
                    break;
                case SearchRange.Month:
                    searchFrom = searchFrom.AddMonths(-1);
                    break;
                case SearchRange.ThreeMonths:
                    searchFrom = searchFrom.AddMonths(-3);
                    break;
                case SearchRange.SixMonths:
                    searchFrom = searchFrom.AddMonths(-6);
                    break;
            }

            var search = !string.IsNullOrWhiteSpace(sub)
                ? searchRange == SearchRange.AllTime ? subreddit?.Search(_filter) : subreddit?.Search(searchFrom, searchTo, sortOrder)
                : reddit.Search<Post>(_filter);

            token.ThrowIfCancellationRequested();
            var listings = search?.GetEnumerator();

            links = CombineLinkLists(await GetImagesFromListing(reddit, listings, outputPath, token), links);

            return links;
        }

        private async Task<List<ImageLink>> GetImagesFromListing(Reddit reddit, IEnumerator<Post> listing, string outputPath, CancellationToken token)
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
                    token.ThrowIfCancellationRequested();
                    Console.WriteLine();
                    for (var i = 0; i < _perSubThreadLimit; i++)
                    {
                        if (_allAuthorsPosts)
                        {
                            if (!users.Contains(listing.Current.AuthorName) &&
                                !processedUsers.Contains(listing.Current.AuthorName))
                            {
                                OutputLine($"Adding user to batch: {listing.Current.AuthorName}", true);
                                users.Add(listing.Current.AuthorName);
                                i = users.Count - 1;
                            }
                        }
                        else
                        {
                            //Tidy this up, unify in list of domain, probably in a config
                            var compatableDomain = isSupportedDomain(listing.Current.Domain);

                            if (!compatableDomain || (!_nsfw && listing.Current.NSFW) ||
                                (_onlyNsfw && !listing.Current.NSFW))
                            {
                                var suffix = listing.Current.NSFW ? " NSFW" : listing.Current.Url.DnsSafeHost;
                                OutputLine($"Skipping non-imgur link: {listing.Current.Url} ({suffix})", true);
                                continue;
                            }

                            posts.Add(listing.Current);
                        }

                        if (!listing.MoveNext())
                            break;
                    }

                    foreach (var user in users)
                    {
                        token.ThrowIfCancellationRequested();
                        OutputLine($"Getting all posts for user: {user}", true);

                        var userPosts =
                            reddit.GetUser(user)
                                .Posts.OrderByDescending(post => post.Score)
                                .Where(post => isSupportedDomain(post.Domain));

                        if (_onlyNsfw)
                            userPosts = userPosts.Where(x => x.NSFW);

                        if (!_nsfw)
                            userPosts = userPosts.Where(x => !x.NSFW);

                        foreach (var userPost in userPosts)
                        {
                            token.ThrowIfCancellationRequested();
                            if (posts.Exists(x => x.Url == userPost.Url) ||
                                links.Exists(x => x.Url == userPost.Url.ToString())) continue;

                            posts.Add(userPost);
                        }

                        processedUsers.Add(user);
                    }

                    users = new HashSet<string>();
                    OutputLine($"Batch returned: {posts.Count} posts.", true);
                    var subName = string.Empty;
                    for (var i = 0; i < posts.Count; i = i + _perSubThreadLimit)
                    {
                        var tasks = new List<Task>();

                        for (var j = 0; j < _perSubThreadLimit; j++)
                        {
                            if (posts.Count <= (i + j))
                                break;

                            var post = posts[i + j];

                            if (post.Domain.Contains("imgur.com"))
                            {
                                tasks.Add(GetImgurImageOrAlbumFromUrl(post, outputPath, token));
                            }
                            else if (post.Domain.Contains("eroshare.com"))
                            {
                                tasks.Add(GetEroshareContentFromUrl(post, outputPath, token));
                            }
                            else if (post.Domain.Contains("reddituploads.com"))
                            {
                                links.Add(GetSingleImageFromUrlWithKnownExtention(post, outputPath, ".jpg", token));
                            }
                            else if (post.Domain.Contains("gfycat.com"))
                            {
                                links.Add(GetSingleImageFromUrlWithKnownExtention(post, outputPath, ".gif", token, true, "giant"));
                            }

                        }
                        Task.WaitAll(tasks.ToArray());

                        links = tasks.Cast<Task<List<ImageLink>>>().Aggregate(links, (current, task) => CombineLinkLists(task.Result, current));

                        if (string.IsNullOrWhiteSpace(subName))
                            subName = links.First()?.Post.SubredditName;

                        OutputLine($"Total Links found in {subName}: {links.Count()}");
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
                if (e is OperationCanceledException || e is AggregateException)
                {
                    throw new OperationCanceledException();
                }

                OutputLine($"Error: {e.Message}");
                erroCount++;
                if (erroCount > PostQueryErrorLimit)
                {
                    throw;
                }
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

        private ImageLink GetSingleImageFromUrlWithKnownExtention(Post post, string outputPath, string extention,
            CancellationToken token, bool appendExtensionToUrl = false, string prefixSubDomain = "")
        {
            token.ThrowIfCancellationRequested();
            var ext = extention.StartsWith(".") ? extention : "." + extention;
            var url = post.Url.ToString().Replace("&amp;", "&");

            if (appendExtensionToUrl)
                url = url + ext;

            if (!string.IsNullOrWhiteSpace(prefixSubDomain))
                url = url.Insert(url.IndexOf("//") + 2,prefixSubDomain + ".");
            
            var filename = GetFilename(post, outputPath);

            filename = filename.EndsWith(ext) ? filename : filename + ext;
            
            OutputLine($"\tAdding Link {url}", true);
            var link = new ImageLink(post, url, filename);
            return link;
        }

        private async Task<List<ImageLink>> GetEroshareContentFromUrl(Post post, string outputPath, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var links = new List<ImageLink>();
            var eroshareApiUrl = "https://api.eroshare.com/api/v1/albums/"; //TODO:: Move to config

            OutputLine($"\tGetting links from post: {post.Title}", true);

            var filename = GetFilename(post, outputPath);

            var url = post.Url.ToString();
            var eroshareId = url.Substring(url.LastIndexOf("/") + 1);

            var request = (HttpWebRequest) WebRequest.Create(eroshareApiUrl + eroshareId);
            var responseJson = string.Empty;
            try
            {
                var response = await request.GetResponseAsync();
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        responseJson = reader.ReadToEnd();
                    }
                }

                var album = JsonConvert.DeserializeObject<EroshareAlbum>(responseJson);

                if (album == null || !album.items.Any())
                    return links;

                foreach (var item in album.items)
                {
                    switch (item.type.ToLower())
                    {
                        case "image":
                            links.Add(new ImageLink(post, item.url_orig, filename + GetExtention(item.url_orig)));
                            break;
                        case "video":
                            links.Add(new ImageLink(post, item.url_mp4, filename + GetExtention(item.url_mp4)));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to download {url} - {ex.Message} {ex.InnerException?.Message}");
            }
            
            return links;
        }

        private async Task<List<ImageLink>> GetImgurImageOrAlbumFromUrl(Post post, string outputPath, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var links = new List<ImageLink>();

            OutputLine($"\tGetting links from post: {post.Title}", true);

            var url = post.Url.GetLeftPart(UriPartial.Path).Replace(".gifv", ".gif");

            url = url.StartsWith("http://") ? url : "http://" + url.Substring(url.IndexOf(post.Url.DnsSafeHost, StringComparison.Ordinal));

            var filename = GetFilename(post, outputPath);

            var extention = GetExtention(url);

            if (!string.IsNullOrEmpty(extention))
            {
                OutputLine($"\tAdding Link {url}", true);
                links.Add(new ImageLink(post, url, filename));
                return links;
            }

            var images = await GetImgurImageAsync(url.Substring(url.LastIndexOf("/") + 1), token);

            links.AddRange(
                images.Select(
                    image => new ImageLink(post, image.Link, filename + GetExtention(image.Link))));

            return links;
        }

        private string GetFilename(Post post, string outputPath)
        {
            var name = Path.GetInvalidFileNameChars()
                .Aggregate(post.AuthorName, (current, c) => current.Replace(c, '-'));

            var filepath = outputPath + "\\";

            if (_allAuthorsPosts)
                filepath += post.AuthorName;
            else
                filepath += post.SubredditName + "\\" + name;

            var filename = filepath + $"\\{name}_{post.SubredditName}_{post.Id}";
            return filename;
        }

        public async Task DownloadPost(KeyValuePair<string, List<ImageLink>> post, string destination, CancellationToken token)
        {
            if (post.Value.Any() && !string.IsNullOrWhiteSpace(destination))
            {
                var details = post.Value.FirstOrDefault()?.GetPostDetails(post);
                var subName = post.Value.FirstOrDefault()?.Post.SubredditName;
                var userName = post.Value.FirstOrDefault()?.Post.AuthorName;
                var postId = post.Key;
                var filePath = destination + Path.DirectorySeparatorChar + subName + Path.DirectorySeparatorChar + userName;
                var fileNameBase = $"{userName}_{subName}_{postId}";

                var dir = new DirectoryInfo(filePath);
                dir.Create();

                var existsingFiles = Directory.GetFiles(filePath, $"{fileNameBase}*", SearchOption.TopDirectoryOnly).ToList();

                if (existsingFiles.Count() >= post.Value.Count) return;

                var count = 0;
                foreach (var imageLink in post.Value)
                {
                    token.ThrowIfCancellationRequested();
                    var uri = new Uri(imageLink.Url);
                    var domain = uri.DnsSafeHost;
                    if (string.IsNullOrWhiteSpace(domain) || imageLink.Url.Contains("removed.png")) continue;

                    imageLink.Url = imageLink.Url.Replace(".gifv", ".gif");

                    var extention = string.IsNullOrWhiteSpace(imageLink.Filename)
                        ? GetExtention(imageLink.Url)
                        : GetExtention(imageLink.Filename);

                    count++;
                    var fullFilePath = string.IsNullOrWhiteSpace(imageLink.Filename) || !imageLink.Filename.Contains(".")
                        ? filePath + Path.DirectorySeparatorChar + fileNameBase + "_" + count.ToString("0000") + extention
                        : destination + Path.DirectorySeparatorChar +
                          imageLink.Filename.Insert(imageLink.Filename.LastIndexOf("."), count.ToString("0000"));

                    if (!existsingFiles.Contains(fullFilePath))
                    {
                        var retryCount = 0;
                        while (!await SaveFile(imageLink, fullFilePath, extention))
                        {
                            retryCount++;
                            if (retryCount >= 5) break;
                        }
                    }
                }

                var detailsFilename = filePath + Path.DirectorySeparatorChar + "postDetails.txt";

                if (File.Exists(detailsFilename))
                    details = Environment.NewLine + Environment.NewLine + details;

                File.AppendAllText(detailsFilename, details);
            }
        }

        private async Task<bool> SaveFile(ImageLink imageLink, string filename, string extention)
        {
            var isImage = videoExtensions.Contains(extention.Replace(".", ""));
            using (var wc = new WebClient())
            {
                try
                {
                    var uri = new Uri(imageLink.Url);
                    var domain = uri.DnsSafeHost;

                    var link = imageLink.Url.StartsWith("http://") ? imageLink.Url : "http://" + imageLink.Url.Substring(imageLink.Url.IndexOf(domain, StringComparison.Ordinal));

                    var tempFilename = isImage ? Path.GetTempPath() + Path.GetRandomFileName() + extention : filename;

                    await wc.DownloadFileTaskAsync(new Uri(link), tempFilename);

                    if (isImage)
                    {
                        using (var stream = new FileStream(tempFilename, FileMode.Open, FileAccess.Read))
                        {
                            using (var image = Image.FromStream(stream))
                            {
                                //XPTitle
                                SetImageProperty(image, 40091,
                                    Encoding.Unicode.GetBytes(imageLink.Post.Title + char.MinValue));
                                //XPComment
                                SetImageProperty(image, 40092, Encoding.Unicode.GetBytes(link + char.MinValue));
                                //XPAuthor
                                SetImageProperty(image, 40093,
                                    Encoding.Unicode.GetBytes(imageLink.Post.AuthorName + char.MinValue));
                                //XPKeywords
                                var keywords = new List<string>
                                {
                                    imageLink.Post.SubredditName,
                                    imageLink.Post.AuthorName,
                                    imageLink.Post.AuthorFlairText,
                                    imageLink.Post.Domain,
                                    imageLink.Post.LinkFlairText
                                };

                                if (imageLink.Post.NSFW)
                                    keywords.Add("NSFW");

                                var title =
                                    imageLink.Post.Title
                                        .Replace('(', '[')
                                        .Replace(')', ']')
                                        .Replace('{', '[')
                                        .Replace('}', ']');

                                keywords.AddRange(from Match match in Regex.Matches(title, @"\[(.*?)\]")
                                    select match.Groups[1].Value.Replace(" ", ""));

                                SetImageProperty(image, 40094,
                                    Encoding.Unicode.GetBytes(string.Join(";", keywords) + char.MinValue));
                                //Save to desination
                                image.Save(filename);
                            }
                        }
                    }

                    //Delete temp file after web client has been disposed (makes sure no handles to file left over)
                    if (isImage)
                        File.Delete(tempFilename);

                    OutputLine($"Downloaded: {imageLink.Url} to {filename}", true);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    OutputLine($"Error: {imageLink.Url} to {filename}", true);
                    OutputLine(e.Message, true);
                    return false;
                }
                return true;
            }
        }

        private static void SetImageProperty(Image image, int propertyId, byte[] value)
        {
            var prop = (PropertyItem) FormatterServices.GetUninitializedObject(typeof(PropertyItem));
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

        public async Task<IEnumerable<IImage>> GetImgurImageAsync(string id, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var error = string.Empty;

                //TODO:: Move to config
                //       Given this is using entirely anonymous calls, there is no reason to keep the id and secret...well, a secret.
                //       If this ever becomes popular, and we want to make authenticated calls; then I will register a new client for that purpose.
                var client = new ImgurClient("5b7f2dbb306592e", "c6321d265e604df392cabd1c1d1fd5002bb918f0");

                var albumEndpoint = new AlbumEndpoint(client);
                try
                {
                    var album = await albumEndpoint.GetAlbumAsync(id);
                    if (album != null)
                        return album.Images;
                }
                catch (Exception e)
                {
                    error = e.Message;
                }

                token.ThrowIfCancellationRequested();
                var imageEndpoint = new ImageEndpoint(client);

                try
                {
                    var image = await imageEndpoint.GetImageAsync(id);

                    if (image != null)
                        return new List<IImage> { image };
                }
                catch (Exception e)
                {
                    error = error + e.Message;
                }

                if (!string.IsNullOrWhiteSpace(error))
                    Log.Error(error);

                return new List<IImage>();
            }
            catch (ImgurException imgurEx)
            {
                Log.Error("An error occurred getting an image from Imgur.");
                Log.Error(imgurEx.Message);
               return null;
            }

        }

        private bool isSupportedDomain(string domain)
        {
            return domain.Contains("imgur.com") ||
                   domain.Contains("eroshare.com") ||
                   domain.Contains("reddituploads.com") ||
                   domain.Contains("gfycat.com");
        }
    }
}