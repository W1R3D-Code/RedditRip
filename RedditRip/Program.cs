using CommandLine.Text;
using RedditRip.Core;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedditRip
{
    internal class Program
    {
        //TODO:: Sort out multiple threads per sub reddit
        private static Options _options;

        private static void Main(string[] args)
        {
            string destination;
            var reddit = new Reddit();
            var subReddits = new List<string>();
            var links = new List<ImageLink>();
            _options = new Options();

            if (CommandLine.Parser.Default.ParseArguments(args, _options))
            {
                if (_options.Subreddits != null)
                    subReddits.AddRange(_options.Subreddits.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)));

                var ripper = new Core.RedditRip(_options.Filter, _options.AllAuthorsPosts, _options.NSFW,
                    _options.OnlyNSFW, _options.Verbose);

                destination = _options.Destination.TrimEnd('\\').Trim();

                if (!string.IsNullOrWhiteSpace(_options.Username))
                {
                    if (!string.IsNullOrWhiteSpace(_options.Password))
                    {
                        OutputLine($"Logging into Reddit as {_options.Username}");
                        reddit.LogIn(_options.Username, _options.Password);
                    }
                    else
                    {
                        OutputLine("Password can not be empty when supplying a username.");
                        return;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(_options.Password))
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
                try
                {
                    var tasks = new List<Task>();
                    if (!string.IsNullOrWhiteSpace(_options.ImportFrom))
                    {
                        try
                        {
                            using (var file = new StreamReader(_options.ImportFrom))
                            {
                                string line;
                                var lineCount = 1;
                                //var importedPosts = new List<Post>();

                                while ((line = file.ReadLine()) != null)
                                {
                                    var link = line.Split('|');
                                    //Post post;
                                    OutputLine($"Reading Line {lineCount}");
                                    //if (!importedPosts.Exists(x => x.Id == line[0]))
                                    //{
                                    //    var postUrl = new Uri("https://reddit.com/r/" + line[1] + "/" + line[0]);
                                    //    post = reddit.GetPost(postUrl);
                                    //    if (post != null)
                                    //        importedPosts.Add(post);
                                    //}
                                    //else
                                    //{
                                    //    post = importedPosts.FirstOrDefault(x => x.Id == line[0]);
                                    //}
                                    links.Add(new ImageLink(new Post() { Id = link[0], SubredditName = link[1], AuthorName = link[2] }, link[4], link[3]));
                                    lineCount++;

                                    if (file.EndOfStream) break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            OutputLine($"Error reading from input file: {e.Message}");
                            throw;
                        }
                    }
                    else
                    {
                        if (subReddits.Any())
                        {
                            tasks.AddRange(subReddits.Where(x => !string.IsNullOrWhiteSpace(x))
                                .Select(
                                    sub => ripper.GetImgurLinksFromSubReddit(reddit, sub, SearchRange.AllTime, Sorting.Top,  destination, new CancellationToken())));
                        }
                        else
                        {
                            tasks.Add(ripper.GetImgurLinksFromSubReddit(reddit, null, SearchRange.AllTime, Sorting.Top, destination, new CancellationToken()));
                        }

                        Task.WaitAll(tasks.ToArray());

                        links = tasks.Cast<Task<List<ImageLink>>>()
                            .Aggregate(links, (current, task) => CombineLinkLists(task.Result, current));
                    }

                    var posts = links.GroupBy(x => x.Post.Id).ToDictionary(x => x.Key, x => x.ToList());

                    tasks = new List<Task>();

                    if (!_options.ExportLinks)
                    {
                        foreach (var post in posts)
                        {
                            var downloadPostTask =
                                ripper.DownloadPost(post, _options.Destination, new CancellationToken())
                                    .ContinueWith(antecedent => OutputLine("Finished downloading post: " + post.Key));

                            tasks.Add(downloadPostTask);
                        }

                        var downloadBatches = Batch(tasks, 10).ToList();
                        var batchCount = downloadBatches.Count;
                        for (var i = 0; i < batchCount; i++)
                        {
                            var batch = downloadBatches.First();
                            Task.WaitAll(batch.ToArray());
                            downloadBatches.Remove(batch);
                        }

                        OutputLine("Finished downloading.");
                    }
                    else
                    {
                        var timestamp = (DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString()).Replace('/', '.').Replace('\\', '.').Replace(':', '.');
                        var linksFilename = _options.Destination + $"\\links-{timestamp}.txt";
                        Directory.CreateDirectory(_options.Destination);
                        using (var file = new StreamWriter(linksFilename))
                        {
                            foreach (var link in posts.SelectMany(post => post.Value))
                            {
                                file.WriteLine(link.Post.Id + "|" + link.Post.SubredditName + "|" +
                                               link.Post.AuthorName + "|" + link.Filename + "|" + link.Url);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    OutputLine($"An Unexpected Error Occured: {e.Message}");
                }
            }
            else
            {
                Console.WriteLine(HelpText.AutoBuild(_options));
            }
        }
        
        private static void OutputLine(string message, bool verboseMessage = false)
        {
            if (verboseMessage && !_options.Verbose) return;
            Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}: {message}");
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: : {message}");
        }

        private static List<ImageLink> CombineLinkLists(IEnumerable<ImageLink> results, List<ImageLink> links)
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

        public static IEnumerable<IEnumerable<Task>> Batch<Task>(IEnumerable<Task> source, int size)
        {
            Task[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new Task[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }
    }
}