using RedditRip.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RedditSharp;
using RedditSharp.Things;

namespace RedditRip.UI
{
    public partial class Main : Form
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<ImageLink> _links { get; set; }

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            txtLog.Text = Environment.NewLine;
        }
        private async void btnDownload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDestination.Text))
                SetDestination();

            if (!string.IsNullOrWhiteSpace(txtDestination.Text))
            {
                txtDestination.Enabled = false;
                btnGetLinks.Enabled = false;

                Tabs.SelectedIndex = Tabs.TabPages["log"].TabIndex;
                OutputLine("Starting downloads....");
                var downloads = new Task(DownloadLinks);
                downloads.Start();
                
                txtDestination.Enabled = true;
                btnGetLinks.Enabled = true;
            }
        }

        private void btnAddSub_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSubReddit.Text))
            {
                if (!listSubReddits.Items.ContainsKey(txtSubReddit.Text))
                {
                    var sub = new ListViewItem()
                    {
                        Name = txtSubReddit.Text,
                        Text = txtSubReddit.Text
                    };

                    listSubReddits.Items.Add(sub);
                    listSubReddits.SelectedIndices.Clear();
                    listSubReddits.SelectedIndices.Add(listSubReddits.Items.IndexOfKey(txtSubReddit.Text));
                }
                txtSubReddit.Text = string.Empty;
            }
        }

        private void btnClearSubs_Click(object sender, EventArgs e)
        {
            listSubReddits.Items.Clear();
        }

        private void btnRemoveSub_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listSubReddits.SelectedItems)
            {
                listSubReddits.Items.Remove(item);
            }
        }

        private void btnDestDir_Click(object sender, EventArgs e)
        {
            SetDestination();
        }

        private void SetDestination()
        {
            var dialog = new FolderBrowserDialog();

            var result = dialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                txtDestination.Text = dialog.SelectedPath.ToString();
            }
        }

        private void listSubReddits_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemoveSub.Enabled = listSubReddits.SelectedItems.Count > 0;
            btnGetLinks.Enabled = listSubReddits.Items.Count > 0;
        }

        private async void btnGetLinks_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDestination.Text))
            {
                MessageBox.Show(
                    "You must select a destination." + Environment.NewLine +
                    "Links are saved with a destination filepath based on the root destination folder you select.",
                    "Select a destination");
                return;
            }
            txtDestination.Enabled = false;
            linkTree.Nodes.Clear();
            Tabs.SelectedIndex = Tabs.TabPages["log"].TabIndex;
            _links = await GetLinks();
            UpdateLinkTree();
        }

        private void bOnlyNsfw_CheckedChanged(object sender, EventArgs e)
        {
            if (bOnlyNsfw.Checked) bAllowNsfw.Checked = true;
            bAllowNsfw.Enabled = !bOnlyNsfw.Checked;
        }

        private void OutputLine(string message, bool verboseMessage = false)
        {
            if (verboseMessage && !bVerbose.Checked) return;
            Debug.WriteLine($"{DateTime.Now.ToShortTimeString()}: {message}");
            log.Info(message);
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

        private async Task<List<ImageLink>> GetLinks()
        {
            var subReddits = new List<string>();
            var tasks = new List<Task>();

            var ripper = new Core.RedditRip(txtFilter.Text, false, bAllowNsfw.Checked, bOnlyNsfw.Checked,
                bVerbose.Checked);

            var reddit = new Reddit();
            var links = new List<ImageLink>();

            foreach (ListViewItem item in listSubReddits.Items)
            {
                subReddits.Add(item.Name);
            }

            if (subReddits.Any())
            {
                tasks.AddRange(subReddits.Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(
                        sub =>
                            Task<List<ImageLink>>.Factory.StartNew(
                                () => ripper.GetImgurLinksFromSubReddit(reddit, sub, txtDestination.Text))));
            }

            await Task.WhenAll(tasks.ToArray());

            links = tasks.Cast<Task<List<ImageLink>>>()
                .Aggregate(links, (current, task) => CombineLinkLists(task.Result, current));

            return links;
        }

        private async void UpdateLinkTree()
        {
            lbLoadingLinks.Visible = true;
            var nodes = new List<TreeNode>();
            var subs = _links.Select(x => x.Post.SubredditName).Distinct().ToList();
            OutputLine("Building nodes for Link Tree");
            //var tasks = subs.Select(sub => new Task<TreeNode>(() => PopulateTreeWithLinks(sub))).ToList();
            //OutputLine("Executing Tasks in parallel.", true);
            //await Task.WhenAll(tasks.ToArray());
            //nodes.AddRange(tasks.Select(task => task.Result));

            //foreach (var sub in subs)
            //{
            //    linkTree.Nodes.Add(PopulateTreeWithLinks(sub));
            //}
            Tabs.SelectedIndex = Tabs.TabPages["log"].TabIndex;
            Parallel.ForEach(subs, sub => nodes.Add(PopulateTreeWithLinks(sub)));
            Tabs.SelectedIndex = Tabs.TabPages["links"].TabIndex;
            foreach (var node in nodes.OrderBy(x => x.Text))
            {
                linkTree.Nodes.Add(node);
            }
            btnDownload.Enabled = true;
            lbLoadingLinks.Visible = false;
        }

        private TreeNode PopulateTreeWithLinks(string subName)
        {
            var subLinks = _links.Where(x => x.Post.SubredditName == subName);
            if (subLinks.Any())
            {
                var subUsers = subLinks.Where(x => x.Post.SubredditName == subName).Select(y => y.Post.AuthorName).Distinct();
                var subNode = new TreeNode() { Name = subName, Text = subName };
                OutputLine($"Building nodes for {subName}.", true);

                foreach (var user in subUsers)
                {
                    var posts = subLinks.Where(x => x.Post.AuthorName == user);
                    var userNode = new TreeNode() { Name = user, Text = user };
                    OutputLine($"Building nodes for {user}'s posts to {subName}.", true);

                    foreach (var post in posts.Select(x => x.Post.Title ?? x.Post.Id).Distinct())
                    {
                        var postLinks = posts.Where(x => x.Post.Title == post || x.Post.Id == post);
                        var postNode = new TreeNode() { Name = post, Text = post };
                        OutputLine($"Building nodes for {user}'s post: '{post}' on {subName}", true);

                        foreach (var link in postLinks)
                        {
                            var linkNode = new TreeNode() { Name = link.Url, Text = link.Url };
                            postNode.Nodes.Add(linkNode);
                        }
                        userNode.Nodes.Add(postNode);
                    }
                    subNode.Nodes.Add(userNode);
                }
                return subNode;
            }
            else
            {
                OutputLine($"No links found for sub: {subName}");
            }

            return null;
        }

        public void AppendLog(string value)
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new Action<string>(AppendLog), new object[] { value });
                    return;
                }

                txtLog.AppendText(value);
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
            catch
            {
                // ignored
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open Text File",
                Filter = "TXT files|*.txt",
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Tabs.SelectedIndex = Tabs.TabPages["log"].TabIndex;
                using (var file = new StreamReader(dialog.FileName))
                {
                    string line;
                    OutputLine($"Reading file: {dialog.FileName}");
                    _links = new List<ImageLink>();
                    while ((line = file.ReadLine()) != null)
                    {
                        var link = line.Split('|');
                        _links.Add(new ImageLink(
                            new Post() {Id = link[0], SubredditName = link[1], AuthorName = link[2]}, link[4].Trim('.', ',', ':', ':', '|', ' '), link[3]));

                        if (file.EndOfStream) break;
                    }
                }
                OutputLine($"Finished reading file: {dialog.FileName}");
                OutputLine($"{_links.Count} links found.");
                var path = Path.GetDirectoryName(_links.FirstOrDefault()?.Filename);
                path = path?.Replace(_links.FirstOrDefault()?.Post.SubredditName + Path.DirectorySeparatorChar +
                                    _links.FirstOrDefault()?.Post.AuthorName, string.Empty).TrimEnd(Path.DirectorySeparatorChar);

                txtDestination.Text = path ?? string.Empty;
                UpdateLinkTree();
            }
        }

        private void exportLinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                Title = "Save Links",
                Filter = "TXT files|*.txt",
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Directory.CreateDirectory(new FileInfo(dialog.FileName).Directory.FullName);
                using (var file = new StreamWriter(dialog.FileName))
                {
                    var posts = _links.GroupBy(x => x.Post.Id).ToDictionary(x => x.Key, x => x.ToList());
                    foreach (var post in posts)
                    {
                        var count = 1;
                        foreach (var link in post.Value)
                        {
                            file.WriteLine(link.Post.Id + "|" + link.Post.SubredditName + "|" +
                                       link.Post.AuthorName + "|" + $"{link.Filename}_{count.ToString("000")}" + "|" + link.Url);
                            count++;
                        }
                    }
                }
            }
        }

        private async void DownloadLinks()
        {
            var posts = _links.GroupBy(x => x.Post.Id).ToDictionary(x => x.Key, x => x.ToList());
            var ripper = new Core.RedditRip(txtFilter.Text, false, bAllowNsfw.Checked, bOnlyNsfw.Checked,
                bVerbose.Checked);
            var tasks = new List<Task>();

            foreach (var post in posts)
            {
                var firstLink = post.Value.FirstOrDefault();

                if (firstLink?.Post != null)
                {
                    var detailsFilepath = Path.GetDirectoryName(firstLink.Filename);
                    var detailsFilename = detailsFilepath + "\\postDetails.txt";
                    var details = firstLink.GetPostDetails(post);

                    Directory.CreateDirectory(detailsFilepath);
                    File.WriteAllText(detailsFilename, details);
                }

                tasks.Add(Task.Factory.StartNew(() => ripper.DownloadPost(post, txtDestination.Text)));
            }

            var downloadBatches = Batch(tasks, 10);
            foreach (var batch in downloadBatches)
            {
                await Task.WhenAll(batch.ToArray());
            }

            OutputLine("Finished downloading.");
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
