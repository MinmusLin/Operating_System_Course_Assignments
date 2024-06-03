#pragma warning disable SYSLIB0011

using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using File_Management.Classes;

namespace File_Management.Window;

public partial class MainWindow : Form
{
    private bool changedFlag;
    private Node rootNode = null!;
    private Node currentNode = null!;
    private Manager manager = null!;
    private Dictionary<int, Pair> pairDictionary = null!;
    private readonly string currentPath = Directory.GetCurrentDirectory();
    private Dictionary<int, ListViewItem> listViewItemDirectory = null!;
    private TreeNode rootTreeNode = null!;
    private Stack<Node> nodeStack = null!;

    // 构造函数
    public MainWindow()
    {
        InitializeComponent();
        ResetOperation();
    }

    // 初始化文件视图
    private void InitializeFileView()
    {
        FileListView.Items.Clear();
        FileTreeView.Nodes.Clear();
        rootTreeNode = new TreeNode("根目录");
        FileTreeView.Nodes.Add(rootTreeNode);
        FileTreeView.ExpandAll();
        currentNode = rootNode;
        PathText.Text = @"> 根目录\";
    }

    // 初始化文件树视图
    private static void InitializeCreateFileTreeView(TreeNode treeNode, Node node)
    {
        foreach (var child in node.ChildNode)
        {
            var childNode = new TreeNode(child.FileName);
            if (child.FileType == "文本文件")
            {
                childNode.ImageIndex = 1;
                childNode.SelectedImageIndex = 1;
            }

            InitializeCreateFileTreeView(childNode, child);
            treeNode.Nodes.Add(childNode);
        }
    }

    // 更新文件视图
    private void UpdateFileView()
    {
        UpdateFileTreeView();
        UpdateFileListView();
    }

    // 更新文件树视图
    private void UpdateFileTreeView()
    {
        FileTreeView.Nodes.Clear();
        rootTreeNode = new TreeNode("根目录");
        InitializeCreateFileTreeView(rootTreeNode, rootNode);
        FileTreeView.Nodes.Add(rootTreeNode);
        FileTreeView.ExpandAll();
    }

    // 更新文件列表视图
    private void UpdateFileListView()
    {
        listViewItemDirectory = new Dictionary<int, ListViewItem>();
        FileListView.Items.Clear();
        foreach (var child in currentNode.ChildNode)
        {
            var file = pairDictionary[child.FileId].Metadata;
            var item = new ListViewItem([
                file.FileName,
                file.ModifiedTime.ToString(CultureInfo.CurrentCulture),
                file.FileType,
                file.FileSize
            ], file.FileType == "文件夹" ? 0 : 1);
            listViewItemDirectory[file.FileId] = item;
            FileListView.Items.Add(item);
        }
    }

    // 新建操作函数
    private void CreateOperation(string fileType, string ext = "")
    {
        changedFlag = true;
        var fileName = CheckFileName("新建" + fileType, ext);
        var newNode = new Node(fileName, fileType);
        currentNode.AddChildNode(newNode);
        Metadata fatherMetadata = null!;
        if (pairDictionary.TryGetValue(currentNode.FileId, out var value)) fatherMetadata = value.Metadata;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var fatherPath = fatherMetadata == null ? "根目录" : fatherMetadata.FilePath;
        var @new = new Metadata(newNode, fatherPath);
        pairDictionary[newNode.FileId] = new Pair(newNode, @new);
        UpdateFileView();
    }

    // 打开操作函数
    private void OpenOperation(int fileId)
    {
        var node = pairDictionary[fileId].Node;
        var metadata = pairDictionary[fileId].Metadata;
        var fileSize = metadata.FileSize;
        switch (node.FileType)
        {
            case "文件夹":
            {
                currentNode = node;
                PathText.Text = @"> " + metadata.FilePath;
                BackwardButton.Enabled = true;
                nodeStack.Clear();
                UpdateFileListView();
                break;
            }
            case "文本文件":
            {
                var txtInputWindow = new EditWindow(node, metadata, pairDictionary, manager, fileSize);
                txtInputWindow.CallBack = UpdateFileView;
                txtInputWindow.Show();
                break;
            }
        }
    }

    // 删除操作函数
    private void DeleteOperation()
    {
        if (FileListView.SelectedItems.Count == 0)
        {
            MessageBox.Show(@"请选中一个文件或文件夹", @"提示");
            return;
        }

        changedFlag = true;
        foreach (ListViewItem item in FileListView.SelectedItems)
        {
            var fileId = GetFileId(item);
            var metadata = pairDictionary[fileId].Metadata;
            var indexList = metadata.FileTable.GetDataIndexList();
            manager.Remove(indexList);
            currentNode.RemoveChildNode(pairDictionary[fileId].Node);
            pairDictionary.Remove(fileId);
        }

        UpdateFileView();
    }

    // 重命名操作函数
    private void RenameOperation()
    {
        if (FileListView.SelectedItems.Count != 1)
        {
            MessageBox.Show(@"请选中一个文件或文件夹", @"提示");
            return;
        }

        changedFlag = true;
        var item = FileListView.SelectedItems[0];
        var fileId = GetFileId(item);
        var node = pairDictionary[fileId].Node;
        var metadata = pairDictionary[fileId].Metadata;
        var renameBox = new RenameWindow(node, metadata, currentNode);
        renameBox.UpdateCallback = UpdateFileView;
        renameBox.Show();
    }

    // 从本地加载虚拟磁盘文件
    private void LoadFromDisk()
    {
        var binaryFormatter = new BinaryFormatter();
        using (var fileDictStream = new FileStream(Path.Combine(currentPath, "FileDictionary.dat"), FileMode.Open,
                   FileAccess.Read, FileShare.Read))
            pairDictionary = (binaryFormatter.Deserialize(fileDictStream) as Dictionary<int, Pair>)!;
        using (var fileRootNodeStream = new FileStream(Path.Combine(currentPath, "FileRootNode.dat"), FileMode.Open,
                   FileAccess.Read, FileShare.Read))
            rootNode = (binaryFormatter.Deserialize(fileRootNodeStream) as Node)!;
        using (var fileManagerStream = new FileStream(Path.Combine(currentPath, "FileManager.dat"), FileMode.Open,
                   FileAccess.Read, FileShare.Read))
            manager = (binaryFormatter.Deserialize(fileManagerStream) as Manager)!;
        var content = "";
        using (var streamReader = new StreamReader(Path.Combine(currentPath, "FileCount.dat")))
            while (streamReader.ReadLine() is { } line)
                content += line;
        Node.counter = int.Parse(content);
        InitializeFileView();
        MessageBox.Show(@"从本地加载虚拟磁盘文件", @"提示");
    }

    // 保存虚拟磁盘文件至本地
    private void SaveToDisk()
    {
        var binaryFormatter = new BinaryFormatter();
        using (var fileDictionaryStream =
               new FileStream(Path.Combine(currentPath, "FileDictionary.dat"), FileMode.Create))
            binaryFormatter.Serialize(fileDictionaryStream, pairDictionary);
        using (var fileRootNodeStream = new FileStream(Path.Combine(currentPath, "FileRootNode.dat"), FileMode.Create))
            binaryFormatter.Serialize(fileRootNodeStream, rootNode);
        using (var fileManagerStream = new FileStream(Path.Combine(currentPath, "FileManager.dat"), FileMode.Create))
            binaryFormatter.Serialize(fileManagerStream, manager);
        using (var fileCountStream =
               new FileStream(Path.Combine(currentPath, "FileCount.dat"), FileMode.Create, FileAccess.Write))
        using (var streamWriter = new StreamWriter(fileCountStream))
            streamWriter.WriteLine(Node.counter.ToString());
        MessageBox.Show(@"保存虚拟磁盘文件至本地：" + currentPath, @"提示");
    }

    // 格式化操作函数
    private void ResetOperation()
    {
        changedFlag = false;
        rootNode = new Node("根目录", "文件夹");
        currentNode = rootNode;
        manager = new Manager();
        pairDictionary = new Dictionary<int, Pair>();
        nodeStack = new Stack<Node>();
        InitializeFileView();
    }

    // 新建文本文件操作鼠标单击响应函数
    private void CreateTextOperationClick(object sender, EventArgs e)
    {
        CreateOperation("文本文件", "txt");
    }

    // 新建文件夹操作鼠标单击响应函数
    private void CreateFolderOperationClick(object sender, EventArgs e)
    {
        CreateOperation("文件夹");
    }

    // 打开操作鼠标单击响应函数
    private void OpenOperationClick(object sender, EventArgs e)
    {
        if (FileListView.SelectedItems.Count != 1)
        {
            MessageBox.Show(@"请选中一个文件或文件夹", @"提示");
            return;
        }

        var item = FileListView.SelectedItems[0];
        var fileId = GetFileId(item);
        OpenOperation(fileId);
    }

    // 删除操作鼠标单击响应函数
    private void DeleteOperationClick(object sender, EventArgs e)
    {
        DeleteOperation();
    }

    // 重命名操作鼠标单击响应函数
    private void RenameOperationClick(object sender, EventArgs e)
    {
        RenameOperation();
    }

    // 从本地加载虚拟磁盘文件操作鼠标单击响应函数
    private void LoadOperationClick(object sender, EventArgs e)
    {
        var fileList = Directory.GetFiles(currentPath, "*.dat").Select(Path.GetFileName).ToList();
        string[] targetFile = ["FileDictionary.dat", "FileRootNode.dat", "FileManager.dat", "FileCount.dat"];
        if (targetFile.Any(file => !fileList.Contains(file)))
        {
            MessageBox.Show(@"从本地加载虚拟磁盘文件失败", @"提示");
            return;
        }

        LoadFromDisk();
        foreach (var child in rootNode.ChildNode) child.FatherNode = rootNode;
        UpdateFileView();
    }

    // 保存虚拟磁盘文件至本地操作鼠标单击响应函数
    private void SaveOperationClick(object sender, EventArgs e)
    {
        SaveToDisk();
        changedFlag = false;
    }

    // 格式化操作鼠标单击响应函数
    private void ResetOperationClick(object sender, EventArgs e)
    {
        ResetOperation();
    }

    // 路径返回按钮鼠标单击响应函数
    private void BackwardButtonClick(object sender, EventArgs e)
    {
        if (currentNode.FileId == rootNode.FileId) return;
        ForwardButton.Enabled = true;
        nodeStack.Push(currentNode);
        currentNode = currentNode.FatherNode;
        if (currentNode.FileId == rootNode.FileId)
        {
            PathText.Text = @"> 根目录\";
        }
        else
        {
            PathText.Text = @"> " + pairDictionary[currentNode.FileId].Metadata.FilePath;
        }

        UpdateFileListView();
    }

    // 路径前进按钮鼠标单击响应函数
    private void ForwardButtonClick(object sender, EventArgs e)
    {
        if (nodeStack.Count == 0) return;
        currentNode = nodeStack.Pop();
        PathText.Text = @"> " + pairDictionary[currentNode.FileId].Metadata.FilePath;
        BackwardButton.Enabled = true;
        UpdateFileListView();
    }

    // 文件列表视图鼠标双击响应函数
    private void FileListViewDoubleClick(object sender, EventArgs e)
    {
        if (FileListView.SelectedItems.Count != 1) return;
        var item = FileListView.SelectedItems[0];
        var fileId = GetFileId(item);
        OpenOperation(fileId);
    }

    // 主窗口关闭响应函数
    private void MainWindowClose(object sender, FormClosingEventArgs e)
    {
        if (changedFlag && MessageBox.Show(@"是否保存虚拟磁盘文件至本地？", @"提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            SaveToDisk();
    }

    // 获取文件 ID
    private int GetFileId(ListViewItem item)
    {
        foreach (var kvp in listViewItemDirectory.Where(kvp => kvp.Value.Text == item.Text)) return kvp.Key;
        MessageBox.Show(@"未找到该文件或文件夹", @"提示");
        return -1;
    }

    // 检查文件名
    private string CheckFileName(string fileName, string ext = "")
    {
        var sameNameFile = new List<int>();
        var name = fileName;
        foreach (var match in from child in currentNode.ChildNode
                 where (child.FileType == "文件夹" && ext == "") || (child.FileType == "文本文件" && ext == "txt")
                 let childFileName = Regex.Replace(child.FileName, $"{Regex4()}|\\.{ext}$", "")
                 where childFileName == name
                 select child.FileType == "文件夹" ? Regex2().Match(child.FileName) : Regex3().Match(child.FileName))
        {
            if (!match.Success)
            {
                sameNameFile.Add(0);
                continue;
            }

            sameNameFile.Add(int.Parse(Regex1().Match(match.Value).Value));
        }

        for (var i = 0; i < currentNode.ChildNode.Count + 1; i++)
        {
            if (sameNameFile.Contains(i)) continue;
            if (i != 0) fileName += "(" + i + ")";
            break;
        }

        return $"{fileName}{(ext == "" ? "" : ("." + ext))}";
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex Regex1();

    [GeneratedRegex(@"\(\d+\)$", RegexOptions.RightToLeft)]
    private static partial Regex Regex2();

    [GeneratedRegex(@"\(\d+\)\.", RegexOptions.RightToLeft)]
    private static partial Regex Regex3();

    [GeneratedRegex(@"\(\d+\)[^(\(\d+\))]*$")]
    private static partial Regex Regex4();
}