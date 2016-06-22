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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RedditSharp;
using RedditSharp.Things;

namespace RedditRip.UI
{
    public partial class Main : Form
    {
        private Task Downloads { get; set; }
        private CancellationTokenSource cts;
        private SearchRange SearchRange { get; set; }
        private Sorting SortOrder { get; set; }
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<ImageLink> _links { get; set; }

        public Main()
        {
            cts = new CancellationTokenSource();
            InitializeComponent();
        }

        private void StartDownload()
        {
            cts = new CancellationTokenSource();
            SetDestination();

            if (!string.IsNullOrWhiteSpace(txtDestination.Text))
            {
                EnableDestinationText(false);
                EnableGetLinksButton(false);

                OutputLine("Starting downloads....");
                Downloads = new TaskFactory().StartNew(() => DownloadLinks(cts.Token), cts.Token);

                EnableCancelButton(true);
                EnableDownloadButton(true);
            }
        }

        private void SetDestination()
        {
            if (string.IsNullOrWhiteSpace(txtDestination.Text) || !Directory.Exists(txtDestination.Text))
            {
                var dialog = new FolderBrowserDialog();
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    txtDestination.Text = dialog.SelectedPath;
                }
            }
        }

        private async void GetLinksAsync(List<string> subs, string filter, bool allowNsfw, bool onlyNsfw, SearchRange searchRange, Sorting sortOrder, CancellationToken token)
        {
            try
            {
                var imgurLinks = await GetImgurLinksFromRedditSub(subs, filter, allowNsfw, onlyNsfw, searchRange, sortOrder, token);
                SetLinks(imgurLinks);
                UpdateLinkTree(token);
            }
            catch (OperationCanceledException)
            {
                _links = new List<ImageLink>();
                ClearLinkNodes();
                OutputLine("Action Canceled by user.");
            }
            catch (Exception e)
            {
                OutputLine("Unexpected Error Occured: " + e.Message);
            }
            finally
            {
                EnableCancelButton(false);
            }
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

        private async Task<List<ImageLink>> GetImgurLinksFromRedditSub(IEnumerable<string> subReddits, string filter, bool allowNsfw, bool onlyNsfw, SearchRange searchRange, Sorting sortOrder, CancellationToken token)
        {
            var tasks = new List<Task>();

            var ripper = new Core.RedditRip(filter, false, allowNsfw, onlyNsfw, bVerbose.Checked);

            var reddit = new Reddit();
            var imageLinks = new List<ImageLink>();
            
            if (subReddits.Any())
            {
                tasks.AddRange(
                    subReddits.Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(sub => ripper.GetImgurLinksFromSubReddit(reddit, sub, searchRange, sortOrder, string.Empty, token))); //TODO:: Refactor destination out of saving links
            }

            await Task.WhenAll(tasks.ToArray());

            imageLinks = tasks.Cast<Task<List<ImageLink>>>()
                .Aggregate(imageLinks, (current, task) => CombineLinkLists(task.Result, current));

            return imageLinks;
        }

        private void UpdateLinkTree(CancellationToken token)
        {
            try
            {
                var nodes = new List<TreeNode>();
                var subs = _links.Select(x => x.Post.SubredditName).Distinct().ToList();
                OutputLine("Building nodes for Link Tree");
                Parallel.ForEach(subs, sub => nodes.Add(PopulateTreeWithLinks(sub, token)));
                foreach (var node in nodes.OrderBy(x => x.Text))
                {
                    AddLinkNode(node);
                }
            }
            catch (OperationCanceledException)
            {
                _links = new List<ImageLink>();
                ClearLinkNodes();
                OutputLine("Action Canceled by user.");
            }
            catch (Exception e)
            {
                OutputLine("Unexpected Error Occured: " + e.Message);
            }
            finally
            {
                EnableCancelButton(false);
                EnableDownloadButton(true);
            }
        }

        private TreeNode PopulateTreeWithLinks(string subName, CancellationToken token)
        {
            var subLinks = _links.Where(x => x.Post.SubredditName == subName);
            if (subLinks.Any())
            {
                var subUsers = subLinks.Where(x => x.Post.SubredditName == subName).Select(y => y.Post.AuthorName).Distinct();
                var subNode = new TreeNode() { Name = subName, Text = subName, Tag = "sub"};
                OutputLine($"Building nodes for {subName}.", true);

                foreach (var user in subUsers)
                {
                    token.ThrowIfCancellationRequested();
                    var posts = subLinks.Where(x => x.Post.AuthorName == user);
                    var userNode = new TreeNode() { Name = user, Text = user, Tag = "user"};
                    OutputLine($"Building nodes for {user}'s posts to {subName}.", true);

                    foreach (var post in posts.Select(x => x.Post.Title ?? x.Post.Id).Distinct())
                    {
                        token.ThrowIfCancellationRequested();
                        var postLinks = posts.Where(x => x.Post.Title == post || x.Post.Id == post);
                        var postNode = new TreeNode() { Name = post, Text = post, Tag = "post"};
                        OutputLine($"Building nodes for {user}'s post: '{post}' on {subName}", true);

                        foreach (var link in postLinks)
                        {
                            token.ThrowIfCancellationRequested();
                            var linkNode = new TreeNode() { Name = link.Url, Text = link.Url, Tag = "image"};
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

        private void DownloadLinks(CancellationToken token)
        {
            try
            {
                var posts = _links.GroupBy(x => x.Post.Id).ToDictionary(x => x.Key, x => x.ToList());
                var ripper = new Core.RedditRip(txtFilter.Text, false, bAllowNsfw.Checked, bOnlyNsfw.Checked,
                    bVerbose.Checked);
                var tasks = new List<Task>();

                foreach (var post in posts)
                {
                    token.ThrowIfCancellationRequested();
                    var downloadPostTask =
                        ripper.DownloadPost(post, txtDestination.Text, token)
                            .ContinueWith(antecedent =>
                            {
                                var link = post.Value.FirstOrDefault();
                                OutputLine("Finished downloading post: " +
                                           (link != null ? link.Post.SubredditName + "/" : string.Empty) + post.Key);
                            }, token);

                    tasks.Add(downloadPostTask);
                }

                var downloadBatches = Batch(tasks, 10).ToList();
                var batchCount = downloadBatches.Count;
                Task[] curretTasks = null;
                for (var i = 0; i < batchCount; i++)
                {
                    var batch = downloadBatches.First();
                    curretTasks = batch.ToArray();
                    Task.WaitAll(curretTasks);
                    downloadBatches.Remove(batch);
                }

                if (curretTasks.ToList().All(x => x.IsCompleted))
                {
                    OutputLine("Finished downloading.");
                    EnableGetLinksButton(true);
                }
            }
            catch (OperationCanceledException)
            {
                OutputLine("Downloads canceled by user.");
            }
            catch (Exception e)
            {
                OutputLine("Unexpected Error Occured: " + e.Message);
            }
            finally
            {
                EnableCancelButton(false);
            }
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

        private void UpdateSearchRange()
        {
            this.SearchRange = (SearchRange)trackBar1.Value;
            lbRange.Text = this.SearchRange.ToString();
            var enableFilter = this.SearchRange == Enum.GetValues(typeof(SearchRange)).Cast<SearchRange>().Max();
            txtFilter.Enabled = enableFilter;
            bNew.Enabled = !enableFilter;
            bTop.Enabled = !enableFilter;
        }

        private void UpdateSortOrder()
        {
            if (bTop.Checked)
                this.SortOrder = Sorting.Top;
            if (bNew.Checked)
                this.SortOrder = Sorting.New;

            bNew.Checked = !bTop.Checked;
            bTop.Checked = !bNew.Checked;
            UpdateSearchRange();
        }

        #region FormEvents

        private void Main_Load(object sender, EventArgs e)
        {
            this.AcceptButton = btnAddSub;
            this.SortOrder = Sorting.Top;
            txtLog.Text = Environment.NewLine;
            trackBar1.SetRange(Enum.GetValues(typeof(SearchRange)).Cast<int>().Min(), Enum.GetValues(typeof(SearchRange)).Cast<int>().Max());
            trackBar1.Value = trackBar1.Maximum;
            UpdateSearchRange();
        }
        private void btnDownload_Click(object sender, EventArgs e) => StartDownload();

        private void btnAddSub_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSubReddit.Text))
            {
                var subs =
                    txtSubReddit.Text.Split(',').Where(x => !string.IsNullOrWhiteSpace(x.Trim())).Select(x => x.Trim());

                foreach (var sub in subs)
                {
                    if (!listSubReddits.Items.ContainsKey(sub))
                    {
                        var listViewItem = new ListViewItem()
                        {
                            Name = sub,
                            Text = sub
                        };

                        listSubReddits.Items.Add(listViewItem);
                        listSubReddits.SelectedIndices.Clear();
                        listSubReddits.SelectedIndices.Add(listSubReddits.Items.IndexOfKey(sub));
                    }
                }
                txtSubReddit.Text = string.Empty;
                this.AcceptButton = btnGetLinks;
            }
        }

        private void btnClearSubs_Click(object sender, EventArgs e) => listSubReddits.Items.Clear();

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

        private void listSubReddits_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemoveSub.Enabled = listSubReddits.SelectedItems.Count > 0;
            btnGetLinks.Enabled = listSubReddits.Items.Count > 0;
        }

        private void btnGetLinks_Click(object sender, EventArgs e)
        {
            var download = MessageBox.Show("Download file after getting links?", "Download when done.",
                MessageBoxButtons.YesNoCancel);

            switch (download)
            {
                case DialogResult.Cancel:
                    return;
                case DialogResult.Yes:
                    SetDestination();
                    if (string.IsNullOrWhiteSpace(txtDestination.Text)) return;
                    break;
            }

            cts = new CancellationTokenSource();
            btnCancel.Enabled = true;
            btnGetLinks.Enabled = false;
            txtDestination.Enabled = false;
            ClearLinkNodes();

            var subs = (from ListViewItem item in listSubReddits.Items select item.Name).ToList();

            if (download == DialogResult.Yes)
            {
                new TaskFactory().StartNew(
                    () => GetLinksAsync(subs, txtFilter.Text, bAllowNsfw.Checked, bOnlyNsfw.Checked, SearchRange, SortOrder, cts.Token),
                    cts.Token).ContinueWith(x => StartDownload());
            }
            else
            {
                new TaskFactory().StartNew(
                    () => GetLinksAsync(subs, txtFilter.Text, bAllowNsfw.Checked, bOnlyNsfw.Checked, SearchRange, SortOrder, cts.Token),
                    cts.Token);
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
                cts = new CancellationTokenSource();
                using (var file = new StreamReader(dialog.FileName))
                {
                    string line;
                    OutputLine($"Reading file: {dialog.FileName}");
                    _links = new List<ImageLink>();
                    while ((line = file.ReadLine()) != null)
                    {
                        var link = line.Split('|');
                        _links.Add(new ImageLink(
                            new Post() { Id = link[0], SubredditName = link[1], AuthorName = link[2] }, link[4].Trim('.', ',', ':', ':', '|', ' '), link[3]));

                        if (file.EndOfStream) break;
                    }
                }
                OutputLine($"Finished reading file: {dialog.FileName}");
                OutputLine($"{_links.Count} links found.");
                var path = Path.GetDirectoryName(_links.FirstOrDefault()?.Filename);
                path = path?.Replace(_links.FirstOrDefault()?.Post.SubredditName + Path.DirectorySeparatorChar +
                                    _links.FirstOrDefault()?.Post.AuthorName, string.Empty).TrimEnd(Path.DirectorySeparatorChar);

                if (string.IsNullOrWhiteSpace(txtDestination.Text))
                    txtDestination.Text = path ?? string.Empty;

                btnCancel.Enabled = true;
                UpdateLinkTree(cts.Token);
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
                                       link.Post.AuthorName + "|" + $"{link.Filename}" + "|" + link.Url);
                            count++;
                        }
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            cts?.Cancel();
            base.OnFormClosing(e);
        }

        private void btnCancel_Click(object sender, EventArgs e) => cts?.Cancel();

        private void txtSubReddit_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSubReddit.Text))
                this.AcceptButton = btnAddSub;
        }

        private void trackBar1_Scroll(object sender, EventArgs e) => UpdateSearchRange();

        private void bTop_CheckedChanged(object sender, EventArgs e) => UpdateSortOrder();

        private void bNew_CheckedChanged(object sender, EventArgs e) => UpdateSortOrder();

        private void bOnlyNsfw_CheckedChanged(object sender, EventArgs e)
        {
            if (bOnlyNsfw.Checked) bAllowNsfw.Checked = true;
            bAllowNsfw.Enabled = !bOnlyNsfw.Checked;
        }

        #endregion

        #region Invoke Form Controls

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

        public void EnableDestinationText(bool value)
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new Action<bool>(EnableDestinationText), new object[] { value });
                    return;
                }

                txtDestination.Enabled = value;
            }
            catch
            {
                // ignored
            }
        }

        public void EnableCancelButton(bool value)
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new Action<bool>(EnableCancelButton), new object[] { value });
                    return;
                }

                btnCancel.Enabled = value;
            }
            catch
            {
                // ignored
            }
        }

        public void EnableDownloadButton(bool value)
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new Action<bool>(EnableDownloadButton), new object[] { value });
                    return;
                }

                btnDownload.Enabled = value;
            }
            catch
            {
                // ignored
            }
        }

        public void EnableGetLinksButton(bool value)
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new Action<bool>(EnableGetLinksButton), new object[] { value });
                    return;
                }

                btnGetLinks.Enabled = value;
            }
            catch
            {
                // ignored
            }
        }

        public void SetLinks(List<ImageLink> value)
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new Action<List<ImageLink>>(SetLinks), new object[] { value });
                    return;
                }

                _links = value;
            }
            catch
            {
                // ignored
            }
        }

        public void AddLinkNode(TreeNode value)
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new Action<TreeNode>(AddLinkNode), new object[] { value });
                    return;
                }

                linkTree.Nodes.Add(value);
            }
            catch
            {
                // ignored
            }
        }

        public void ClearLinkNodes()
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new Action(ClearLinkNodes));
                    return;
                }

                linkTree.Nodes.Clear();
            }
            catch
            {
                // ignored
            }
        }

        #endregion
    }
}
