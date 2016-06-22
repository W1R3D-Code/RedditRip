namespace RedditRip.UI
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtSubReddit = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddSub = new System.Windows.Forms.Button();
            this.listSubReddits = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.btnDestDir = new System.Windows.Forms.Button();
            this.btnRemoveSub = new System.Windows.Forms.Button();
            this.btnClearSubs = new System.Windows.Forms.Button();
            this.btnGetLinks = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bOnlyNsfw = new System.Windows.Forms.CheckBox();
            this.bAllowNsfw = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbRange = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportLinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Tabs = new System.Windows.Forms.TabControl();
            this.links = new System.Windows.Forms.TabPage();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.linkTree = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.bVerbose = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.bTop = new System.Windows.Forms.RadioButton();
            this.bNew = new System.Windows.Forms.RadioButton();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.Tabs.SuspendLayout();
            this.links.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSubReddit
            // 
            this.txtSubReddit.Location = new System.Drawing.Point(82, 22);
            this.txtSubReddit.Name = "txtSubReddit";
            this.txtSubReddit.Size = new System.Drawing.Size(188, 20);
            this.txtSubReddit.TabIndex = 0;
            this.txtSubReddit.TextChanged += new System.EventHandler(this.txtSubReddit_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Sub Reddit:";
            // 
            // btnAddSub
            // 
            this.btnAddSub.Location = new System.Drawing.Point(276, 21);
            this.btnAddSub.Name = "btnAddSub";
            this.btnAddSub.Size = new System.Drawing.Size(75, 21);
            this.btnAddSub.TabIndex = 2;
            this.btnAddSub.Text = "Add";
            this.btnAddSub.UseVisualStyleBackColor = true;
            this.btnAddSub.Click += new System.EventHandler(this.btnAddSub_Click);
            // 
            // listSubReddits
            // 
            this.listSubReddits.Location = new System.Drawing.Point(11, 48);
            this.listSubReddits.Name = "listSubReddits";
            this.listSubReddits.Size = new System.Drawing.Size(260, 81);
            this.listSubReddits.TabIndex = 3;
            this.listSubReddits.UseCompatibleStateImageBehavior = false;
            this.listSubReddits.View = System.Windows.Forms.View.List;
            this.listSubReddits.SelectedIndexChanged += new System.EventHandler(this.listSubReddits_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Destination Folder:";
            // 
            // txtDestination
            // 
            this.txtDestination.Location = new System.Drawing.Point(109, 205);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(212, 20);
            this.txtDestination.TabIndex = 5;
            // 
            // btnDestDir
            // 
            this.btnDestDir.Location = new System.Drawing.Point(327, 205);
            this.btnDestDir.Name = "btnDestDir";
            this.btnDestDir.Size = new System.Drawing.Size(24, 20);
            this.btnDestDir.TabIndex = 6;
            this.btnDestDir.Text = "...";
            this.btnDestDir.UseVisualStyleBackColor = true;
            this.btnDestDir.Click += new System.EventHandler(this.btnDestDir_Click);
            // 
            // btnRemoveSub
            // 
            this.btnRemoveSub.Enabled = false;
            this.btnRemoveSub.Location = new System.Drawing.Point(277, 77);
            this.btnRemoveSub.Name = "btnRemoveSub";
            this.btnRemoveSub.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveSub.TabIndex = 7;
            this.btnRemoveSub.Text = "Remove";
            this.btnRemoveSub.UseVisualStyleBackColor = true;
            this.btnRemoveSub.Click += new System.EventHandler(this.btnRemoveSub_Click);
            // 
            // btnClearSubs
            // 
            this.btnClearSubs.Location = new System.Drawing.Point(277, 106);
            this.btnClearSubs.Name = "btnClearSubs";
            this.btnClearSubs.Size = new System.Drawing.Size(75, 23);
            this.btnClearSubs.TabIndex = 8;
            this.btnClearSubs.Text = "Clear";
            this.btnClearSubs.UseVisualStyleBackColor = true;
            this.btnClearSubs.Click += new System.EventHandler(this.btnClearSubs_Click);
            // 
            // btnGetLinks
            // 
            this.btnGetLinks.Enabled = false;
            this.btnGetLinks.Location = new System.Drawing.Point(397, 204);
            this.btnGetLinks.Name = "btnGetLinks";
            this.btnGetLinks.Size = new System.Drawing.Size(111, 23);
            this.btnGetLinks.TabIndex = 9;
            this.btnGetLinks.Text = "Discover Links";
            this.btnGetLinks.UseVisualStyleBackColor = true;
            this.btnGetLinks.Click += new System.EventHandler(this.btnGetLinks_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Enabled = false;
            this.btnDownload.Location = new System.Drawing.Point(533, 204);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 10;
            this.btnDownload.Text = "Download Links";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtFilter);
            this.groupBox2.Controls.Add(this.bVerbose);
            this.groupBox2.Controls.Add(this.bOnlyNsfw);
            this.groupBox2.Controls.Add(this.bAllowNsfw);
            this.groupBox2.Location = new System.Drawing.Point(385, 25);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(304, 104);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // bOnlyNsfw
            // 
            this.bOnlyNsfw.AutoSize = true;
            this.bOnlyNsfw.Location = new System.Drawing.Point(104, 19);
            this.bOnlyNsfw.Name = "bOnlyNsfw";
            this.bOnlyNsfw.Size = new System.Drawing.Size(82, 17);
            this.bOnlyNsfw.TabIndex = 1;
            this.bOnlyNsfw.Text = "Only NSFW";
            this.bOnlyNsfw.UseVisualStyleBackColor = true;
            this.bOnlyNsfw.CheckedChanged += new System.EventHandler(this.bOnlyNsfw_CheckedChanged);
            // 
            // bAllowNsfw
            // 
            this.bAllowNsfw.AutoSize = true;
            this.bAllowNsfw.Checked = true;
            this.bAllowNsfw.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bAllowNsfw.Location = new System.Drawing.Point(12, 19);
            this.bAllowNsfw.Name = "bAllowNsfw";
            this.bAllowNsfw.Size = new System.Drawing.Size(86, 17);
            this.bAllowNsfw.TabIndex = 0;
            this.bAllowNsfw.Text = "Allow NSFW";
            this.bAllowNsfw.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bNew);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.bTop);
            this.groupBox1.Controls.Add(this.lbRange);
            this.groupBox1.Controls.Add(this.trackBar1);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.btnDownload);
            this.groupBox1.Controls.Add(this.btnGetLinks);
            this.groupBox1.Controls.Add(this.btnClearSubs);
            this.groupBox1.Controls.Add(this.btnRemoveSub);
            this.groupBox1.Controls.Add(this.btnDestDir);
            this.groupBox1.Controls.Add(this.txtDestination);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.listSubReddits);
            this.groupBox1.Controls.Add(this.btnAddSub);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtSubReddit);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(700, 241);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search";
            // 
            // lbRange
            // 
            this.lbRange.AutoSize = true;
            this.lbRange.Location = new System.Drawing.Point(93, 141);
            this.lbRange.Name = "lbRange";
            this.lbRange.Size = new System.Drawing.Size(37, 13);
            this.lbRange.TabIndex = 17;
            this.lbRange.Text = "Today";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(11, 160);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(259, 45);
            this.trackBar1.TabIndex = 16;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Value = 1;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(614, 204);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(719, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportLinksToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.importToolStripMenuItem.Text = "Import Links";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportLinksToolStripMenuItem
            // 
            this.exportLinksToolStripMenuItem.Name = "exportLinksToolStripMenuItem";
            this.exportLinksToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.exportLinksToolStripMenuItem.Text = "Export Links";
            this.exportLinksToolStripMenuItem.Click += new System.EventHandler(this.exportLinksToolStripMenuItem_Click);
            // 
            // Tabs
            // 
            this.Tabs.Controls.Add(this.links);
            this.Tabs.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Tabs.Location = new System.Drawing.Point(0, 258);
            this.Tabs.Name = "Tabs";
            this.Tabs.SelectedIndex = 0;
            this.Tabs.Size = new System.Drawing.Size(719, 485);
            this.Tabs.TabIndex = 3;
            // 
            // links
            // 
            this.links.Controls.Add(this.txtLog);
            this.links.Controls.Add(this.linkTree);
            this.links.Location = new System.Drawing.Point(4, 22);
            this.links.Name = "links";
            this.links.Padding = new System.Windows.Forms.Padding(3);
            this.links.Size = new System.Drawing.Size(711, 459);
            this.links.TabIndex = 0;
            this.links.Text = "Links";
            this.links.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.ControlText;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.ForeColor = System.Drawing.SystemColors.Info;
            this.txtLog.Location = new System.Drawing.Point(263, 3);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(445, 453);
            this.txtLog.TabIndex = 2;
            // 
            // linkTree
            // 
            this.linkTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.linkTree.Location = new System.Drawing.Point(3, 3);
            this.linkTree.Name = "linkTree";
            this.linkTree.Size = new System.Drawing.Size(260, 453);
            this.linkTree.TabIndex = 1;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(32, 32);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Search Range:";
            // 
            // bVerbose
            // 
            this.bVerbose.AutoSize = true;
            this.bVerbose.Location = new System.Drawing.Point(192, 19);
            this.bVerbose.Name = "bVerbose";
            this.bVerbose.Size = new System.Drawing.Size(91, 17);
            this.bVerbose.TabIndex = 15;
            this.bVerbose.Text = "Verbose Logs";
            this.bVerbose.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Search Filter:";
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(84, 55);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(199, 20);
            this.txtFilter.TabIndex = 16;
            // 
            // bTop
            // 
            this.bTop.AutoSize = true;
            this.bTop.Checked = true;
            this.bTop.Location = new System.Drawing.Point(277, 160);
            this.bTop.Name = "bTop";
            this.bTop.Size = new System.Drawing.Size(44, 17);
            this.bTop.TabIndex = 18;
            this.bTop.TabStop = true;
            this.bTop.Text = "Top";
            this.bTop.UseVisualStyleBackColor = true;
            this.bTop.CheckedChanged += new System.EventHandler(this.bTop_CheckedChanged);
            // 
            // bNew
            // 
            this.bNew.AutoSize = true;
            this.bNew.Location = new System.Drawing.Point(327, 160);
            this.bNew.Name = "bNew";
            this.bNew.Size = new System.Drawing.Size(47, 17);
            this.bNew.TabIndex = 20;
            this.bNew.Text = "New";
            this.bNew.UseVisualStyleBackColor = true;
            this.bNew.CheckedChanged += new System.EventHandler(this.bNew_CheckedChanged);
            // 
            // Main
            // 
            this.AcceptButton = this.btnAddSub;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(719, 743);
            this.Controls.Add(this.Tabs);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "RedditRip";
            this.Load += new System.EventHandler(this.Main_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.Tabs.ResumeLayout(false);
            this.links.ResumeLayout(false);
            this.links.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtSubReddit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddSub;
        private System.Windows.Forms.ListView listSubReddits;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.Button btnDestDir;
        private System.Windows.Forms.Button btnRemoveSub;
        private System.Windows.Forms.Button btnClearSubs;
        private System.Windows.Forms.Button btnGetLinks;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox bOnlyNsfw;
        private System.Windows.Forms.CheckBox bAllowNsfw;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportLinksToolStripMenuItem;
        private System.Windows.Forms.TabControl Tabs;
        private System.Windows.Forms.TabPage links;
        private System.Windows.Forms.TreeView linkTree;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label lbRange;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.CheckBox bVerbose;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton bNew;
        private System.Windows.Forms.RadioButton bTop;
    }
}

