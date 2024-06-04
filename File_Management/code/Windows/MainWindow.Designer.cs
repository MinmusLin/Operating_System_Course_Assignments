namespace File_Management.Windows
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            ListViewItem listViewItem1 = new ListViewItem("");
            Menu = new MenuStrip();
            FileMenuItem = new ToolStripMenuItem();
            LoadDropdownItem = new ToolStripMenuItem();
            SaveDropdownItem = new ToolStripMenuItem();
            DropdownItemSeparator = new ToolStripSeparator();
            ResetDropdownItem = new ToolStripMenuItem();
            OperationMenuItem = new ToolStripMenuItem();
            CreateDropdownItem = new ToolStripMenuItem();
            TextDropdownItem = new ToolStripMenuItem();
            FolderDropdownItem = new ToolStripMenuItem();
            OpenDropdownItem = new ToolStripMenuItem();
            DeleteDropdownItem = new ToolStripMenuItem();
            RenameDropdownItem = new ToolStripMenuItem();
            PathText = new TextBox();
            BackwardButton = new Button();
            ForwardButton = new Button();
            FileTreeView = new TreeView();
            IconList = new ImageList(components);
            FileListView = new ListView();
            FileNameHeader = new ColumnHeader();
            ModifiedTimeHeader = new ColumnHeader();
            FileTypeHeader = new ColumnHeader();
            FileSizeHeader = new ColumnHeader();
            ContextMenu = new ContextMenuStrip(components);
            CreateContextMenuItem = new ToolStripMenuItem();
            TextContextMenuItem = new ToolStripMenuItem();
            FolderContextMenuItem = new ToolStripMenuItem();
            OpenContextMenuItem = new ToolStripMenuItem();
            DeleteContextMenuItem = new ToolStripMenuItem();
            RenameContextMenuItem = new ToolStripMenuItem();
            Menu.SuspendLayout();
            ContextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // Menu
            // 
            Menu.BackColor = SystemColors.Window;
            Menu.Font = new Font("Microsoft YaHei UI", 10F);
            Menu.ImageScalingSize = new Size(24, 24);
            Menu.Items.AddRange(new ToolStripItem[] { FileMenuItem, OperationMenuItem });
            Menu.Location = new Point(0, 0);
            Menu.Name = "Menu";
            Menu.Padding = new Padding(5, 1, 0, 1);
            Menu.Size = new Size(924, 26);
            Menu.TabIndex = 0;
            Menu.Text = "menuStrip1";
            // 
            // FileMenuItem
            // 
            FileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { LoadDropdownItem, SaveDropdownItem, DropdownItemSeparator, ResetDropdownItem });
            FileMenuItem.ImageAlign = ContentAlignment.BottomCenter;
            FileMenuItem.Name = "FileMenuItem";
            FileMenuItem.ShortcutKeys = Keys.Alt | Keys.F;
            FileMenuItem.Size = new Size(66, 24);
            FileMenuItem.Text = "文件(&F)";
            // 
            // LoadDropdownItem
            // 
            LoadDropdownItem.Image = (Image)resources.GetObject("LoadDropdownItem.Image");
            LoadDropdownItem.Name = "LoadDropdownItem";
            LoadDropdownItem.ShortcutKeys = Keys.Control | Keys.L;
            LoadDropdownItem.Size = new Size(206, 30);
            LoadDropdownItem.Text = "从本地加载";
            LoadDropdownItem.Click += LoadOperationClick;
            // 
            // SaveDropdownItem
            // 
            SaveDropdownItem.Image = (Image)resources.GetObject("SaveDropdownItem.Image");
            SaveDropdownItem.Name = "SaveDropdownItem";
            SaveDropdownItem.ShortcutKeys = Keys.Control | Keys.S;
            SaveDropdownItem.Size = new Size(206, 30);
            SaveDropdownItem.Text = "保存至本地";
            SaveDropdownItem.Click += SaveOperationClick;
            // 
            // DropdownItemSeparator
            // 
            DropdownItemSeparator.Name = "DropdownItemSeparator";
            DropdownItemSeparator.Size = new Size(203, 6);
            // 
            // ResetDropdownItem
            // 
            ResetDropdownItem.Image = (Image)resources.GetObject("ResetDropdownItem.Image");
            ResetDropdownItem.Name = "ResetDropdownItem";
            ResetDropdownItem.ShortcutKeys = Keys.Control | Keys.R;
            ResetDropdownItem.Size = new Size(206, 30);
            ResetDropdownItem.Text = "格式化";
            ResetDropdownItem.Click += ResetOperationClick;
            // 
            // OperationMenuItem
            // 
            OperationMenuItem.DropDownItems.AddRange(new ToolStripItem[] { CreateDropdownItem, OpenDropdownItem, DeleteDropdownItem, RenameDropdownItem });
            OperationMenuItem.Name = "OperationMenuItem";
            OperationMenuItem.ShortcutKeys = Keys.Alt | Keys.O;
            OperationMenuItem.Size = new Size(70, 24);
            OperationMenuItem.Text = "操作(&O)";
            // 
            // CreateDropdownItem
            // 
            CreateDropdownItem.DropDownItems.AddRange(new ToolStripItem[] { TextDropdownItem, FolderDropdownItem });
            CreateDropdownItem.Image = (Image)resources.GetObject("CreateDropdownItem.Image");
            CreateDropdownItem.Name = "CreateDropdownItem";
            CreateDropdownItem.Size = new Size(181, 30);
            CreateDropdownItem.Text = "新建";
            // 
            // TextDropdownItem
            // 
            TextDropdownItem.Image = (Image)resources.GetObject("TextDropdownItem.Image");
            TextDropdownItem.Name = "TextDropdownItem";
            TextDropdownItem.ShortcutKeys = Keys.Control | Keys.T;
            TextDropdownItem.Size = new Size(192, 30);
            TextDropdownItem.Text = "文本文件";
            TextDropdownItem.Click += CreateTextOperationClick;
            // 
            // FolderDropdownItem
            // 
            FolderDropdownItem.Image = (Image)resources.GetObject("FolderDropdownItem.Image");
            FolderDropdownItem.Name = "FolderDropdownItem";
            FolderDropdownItem.ShortcutKeys = Keys.Control | Keys.F;
            FolderDropdownItem.Size = new Size(192, 30);
            FolderDropdownItem.Text = "文件夹";
            FolderDropdownItem.Click += CreateFolderOperationClick;
            // 
            // OpenDropdownItem
            // 
            OpenDropdownItem.Image = (Image)resources.GetObject("OpenDropdownItem.Image");
            OpenDropdownItem.Name = "OpenDropdownItem";
            OpenDropdownItem.ShortcutKeys = Keys.Control | Keys.O;
            OpenDropdownItem.Size = new Size(181, 30);
            OpenDropdownItem.Text = "打开";
            OpenDropdownItem.Click += OpenOperationClick;
            // 
            // DeleteDropdownItem
            // 
            DeleteDropdownItem.Image = (Image)resources.GetObject("DeleteDropdownItem.Image");
            DeleteDropdownItem.Name = "DeleteDropdownItem";
            DeleteDropdownItem.ShortcutKeys = Keys.Control | Keys.D;
            DeleteDropdownItem.Size = new Size(181, 30);
            DeleteDropdownItem.Text = "删除";
            DeleteDropdownItem.Click += DeleteOperationClick;
            // 
            // RenameDropdownItem
            // 
            RenameDropdownItem.Image = (Image)resources.GetObject("RenameDropdownItem.Image");
            RenameDropdownItem.Name = "RenameDropdownItem";
            RenameDropdownItem.ShortcutKeys = Keys.Control | Keys.N;
            RenameDropdownItem.Size = new Size(181, 30);
            RenameDropdownItem.Text = "重命名";
            RenameDropdownItem.Click += RenameOperationClick;
            // 
            // PathText
            // 
            PathText.BackColor = SystemColors.Window;
            PathText.CausesValidation = false;
            PathText.Font = new Font("Microsoft YaHei UI", 12F);
            PathText.Location = new Point(75, 27);
            PathText.Margin = new Padding(2, 1, 2, 1);
            PathText.MaxLength = int.MaxValue;
            PathText.Name = "PathText";
            PathText.ReadOnly = true;
            PathText.Size = new Size(838, 28);
            PathText.TabIndex = 3;
            PathText.TabStop = false;
            PathText.Text = "> 根目录\\";
            PathText.WordWrap = false;
            // 
            // BackwardButton
            // 
            BackwardButton.BackColor = SystemColors.Window;
            BackwardButton.BackgroundImage = (Image)resources.GetObject("BackwardButton.BackgroundImage");
            BackwardButton.BackgroundImageLayout = ImageLayout.Zoom;
            BackwardButton.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            BackwardButton.ForeColor = SystemColors.ControlText;
            BackwardButton.Location = new Point(11, 27);
            BackwardButton.Margin = new Padding(2, 1, 2, 1);
            BackwardButton.Name = "BackwardButton";
            BackwardButton.Size = new Size(28, 28);
            BackwardButton.TabIndex = 1;
            BackwardButton.TabStop = false;
            BackwardButton.UseVisualStyleBackColor = false;
            BackwardButton.Click += BackwardButtonClick;
            // 
            // ForwardButton
            // 
            ForwardButton.BackColor = SystemColors.Window;
            ForwardButton.BackgroundImage = (Image)resources.GetObject("ForwardButton.BackgroundImage");
            ForwardButton.BackgroundImageLayout = ImageLayout.Zoom;
            ForwardButton.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            ForwardButton.ForeColor = SystemColors.ControlText;
            ForwardButton.Location = new Point(43, 27);
            ForwardButton.Margin = new Padding(2, 1, 2, 1);
            ForwardButton.Name = "ForwardButton";
            ForwardButton.Size = new Size(28, 28);
            ForwardButton.TabIndex = 2;
            ForwardButton.TabStop = false;
            ForwardButton.Text = "->";
            ForwardButton.UseVisualStyleBackColor = false;
            ForwardButton.Click += ForwardButtonClick;
            // 
            // FileTreeView
            // 
            FileTreeView.ImageIndex = 0;
            FileTreeView.ImageList = IconList;
            FileTreeView.Location = new Point(11, 63);
            FileTreeView.Margin = new Padding(2, 1, 2, 1);
            FileTreeView.Name = "FileTreeView";
            FileTreeView.SelectedImageIndex = 0;
            FileTreeView.Size = new Size(250, 555);
            FileTreeView.TabIndex = 4;
            FileTreeView.TabStop = false;
            // 
            // IconList
            // 
            IconList.ColorDepth = ColorDepth.Depth8Bit;
            IconList.ImageStream = (ImageListStreamer)resources.GetObject("IconList.ImageStream");
            IconList.TransparentColor = Color.Transparent;
            IconList.Images.SetKeyName(0, "Folder.png");
            IconList.Images.SetKeyName(1, "File.png");
            // 
            // FileListView
            // 
            FileListView.Columns.AddRange(new ColumnHeader[] { FileNameHeader, ModifiedTimeHeader, FileTypeHeader, FileSizeHeader });
            FileListView.ContextMenuStrip = ContextMenu;
            FileListView.Font = new Font("Microsoft YaHei UI", 10F);
            FileListView.FullRowSelect = true;
            FileListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            FileListView.Items.AddRange(new ListViewItem[] { listViewItem1 });
            FileListView.Location = new Point(269, 63);
            FileListView.Margin = new Padding(2, 1, 2, 1);
            FileListView.Name = "FileListView";
            FileListView.Size = new Size(644, 555);
            FileListView.SmallImageList = IconList;
            FileListView.Sorting = SortOrder.Ascending;
            FileListView.TabIndex = 5;
            FileListView.TabStop = false;
            FileListView.UseCompatibleStateImageBehavior = false;
            FileListView.View = View.Details;
            FileListView.DoubleClick += FileListViewDoubleClick;
            // 
            // FileNameHeader
            // 
            FileNameHeader.Text = "文件名 / 文件夹名";
            FileNameHeader.Width = 263;
            // 
            // ModifiedTimeHeader
            // 
            ModifiedTimeHeader.Text = "修改时间";
            ModifiedTimeHeader.Width = 160;
            // 
            // FileTypeHeader
            // 
            FileTypeHeader.Text = "文件类型";
            FileTypeHeader.Width = 100;
            // 
            // FileSizeHeader
            // 
            FileSizeHeader.Text = "文件大小";
            FileSizeHeader.Width = 100;
            // 
            // ContextMenu
            // 
            ContextMenu.Font = new Font("Microsoft YaHei UI", 10F);
            ContextMenu.ImageScalingSize = new Size(24, 24);
            ContextMenu.Items.AddRange(new ToolStripItem[] { CreateContextMenuItem, OpenContextMenuItem, DeleteContextMenuItem, RenameContextMenuItem });
            ContextMenu.Name = "contextMenuStrip1";
            ContextMenu.Size = new Size(223, 124);
            // 
            // CreateContextMenuItem
            // 
            CreateContextMenuItem.DropDownItems.AddRange(new ToolStripItem[] { TextContextMenuItem, FolderContextMenuItem });
            CreateContextMenuItem.Image = (Image)resources.GetObject("CreateContextMenuItem.Image");
            CreateContextMenuItem.Name = "CreateContextMenuItem";
            CreateContextMenuItem.Size = new Size(222, 30);
            CreateContextMenuItem.Text = "新建";
            // 
            // TextContextMenuItem
            // 
            TextContextMenuItem.Image = (Image)resources.GetObject("TextContextMenuItem.Image");
            TextContextMenuItem.Name = "TextContextMenuItem";
            TextContextMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.T;
            TextContextMenuItem.Size = new Size(233, 30);
            TextContextMenuItem.Text = "文本文件";
            TextContextMenuItem.Click += CreateTextOperationClick;
            // 
            // FolderContextMenuItem
            // 
            FolderContextMenuItem.Image = (Image)resources.GetObject("FolderContextMenuItem.Image");
            FolderContextMenuItem.Name = "FolderContextMenuItem";
            FolderContextMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.F;
            FolderContextMenuItem.Size = new Size(233, 30);
            FolderContextMenuItem.Text = "文件夹";
            FolderContextMenuItem.Click += CreateFolderOperationClick;
            // 
            // OpenContextMenuItem
            // 
            OpenContextMenuItem.Image = (Image)resources.GetObject("OpenContextMenuItem.Image");
            OpenContextMenuItem.Name = "OpenContextMenuItem";
            OpenContextMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.O;
            OpenContextMenuItem.Size = new Size(222, 30);
            OpenContextMenuItem.Text = "打开";
            OpenContextMenuItem.Click += OpenOperationClick;
            // 
            // DeleteContextMenuItem
            // 
            DeleteContextMenuItem.Image = (Image)resources.GetObject("DeleteContextMenuItem.Image");
            DeleteContextMenuItem.Name = "DeleteContextMenuItem";
            DeleteContextMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.D;
            DeleteContextMenuItem.Size = new Size(222, 30);
            DeleteContextMenuItem.Text = "删除";
            DeleteContextMenuItem.Click += DeleteOperationClick;
            // 
            // RenameContextMenuItem
            // 
            RenameContextMenuItem.Image = (Image)resources.GetObject("RenameContextMenuItem.Image");
            RenameContextMenuItem.Name = "RenameContextMenuItem";
            RenameContextMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.N;
            RenameContextMenuItem.Size = new Size(222, 30);
            RenameContextMenuItem.Text = "重命名";
            RenameContextMenuItem.Click += RenameOperationClick;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(924, 629);
            Controls.Add(FileListView);
            Controls.Add(FileTreeView);
            Controls.Add(ForwardButton);
            Controls.Add(BackwardButton);
            Controls.Add(PathText);
            Controls.Add(Menu);
            Font = new Font("Microsoft YaHei UI", 10F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2, 1, 2, 1);
            MaximizeBox = false;
            MaximumSize = new Size(940, 668);
            MinimumSize = new Size(940, 668);
            Name = "MainWindow";
            Text = "File Management | 文件管理 - Virtual File System Manager (VFSM) | 虚拟文件系统管理器 - 2250758 林继申";
            FormClosing += MainWindowClose;
            Menu.ResumeLayout(false);
            Menu.PerformLayout();
            ContextMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip Menu;
        private System.Windows.Forms.ToolStripMenuItem FileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadDropdownItem;
        private System.Windows.Forms.ToolStripMenuItem SaveDropdownItem;
        private System.Windows.Forms.ToolStripMenuItem OperationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CreateDropdownItem;
        private System.Windows.Forms.ToolStripMenuItem TextDropdownItem;
        private System.Windows.Forms.ToolStripMenuItem FolderDropdownItem;
        private System.Windows.Forms.ToolStripMenuItem OpenDropdownItem;
        private System.Windows.Forms.ToolStripMenuItem DeleteDropdownItem;
        private System.Windows.Forms.TextBox PathText;
        private System.Windows.Forms.Button BackwardButton;
        private System.Windows.Forms.Button ForwardButton;
        private System.Windows.Forms.TreeView FileTreeView;
        private System.Windows.Forms.ListView FileListView;
        private System.Windows.Forms.ColumnHeader FileNameHeader;
        private System.Windows.Forms.ColumnHeader ModifiedTimeHeader;
        private System.Windows.Forms.ColumnHeader FileTypeHeader;
        private System.Windows.Forms.ColumnHeader FileSizeHeader;
        private System.Windows.Forms.ContextMenuStrip ContextMenu;
        private System.Windows.Forms.ToolStripMenuItem OpenContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeleteContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RenameContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RenameDropdownItem;
        private System.Windows.Forms.ImageList IconList;
        private System.Windows.Forms.ToolStripMenuItem CreateContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FolderContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TextContextMenuItem;
        private ToolStripMenuItem ResetDropdownItem;
        private ToolStripSeparator DropdownItemSeparator;
    }
}

