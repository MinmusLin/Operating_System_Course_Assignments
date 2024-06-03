#pragma warning disable SYSLIB0011

using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using File_Management.Classes;

namespace File_Management.Window;

public partial class MainWindow : Form
{
    // 修改标识
    bool changed;

    // 当前symfcb
    public Node CurNode;

    // 根symfcb
    public Node RootNode;

    // 字典fileId -> fcb的映射
    public Dictionary<int, Pair> fileDict;

    // 管理
    public Manager manager;

    // 当前路径
    public string curPath = Directory.GetCurrentDirectory();

    // 文件显示窗口
    private Dictionary<int, ListViewItem> listTable;

    // 文件树根结点
    private TreeNode rootNode;

    // 保存前进方向
    private Stack<Node> fileStack;

    public MainWindow()
    {
        InitializeComponent();
        RootNode = new Node("ROOT", "folder");
        CurNode = RootNode;
        manager = new Manager();
        fileDict = new Dictionary<int, Pair>();
        fileStack = new Stack<Node>();
        changed = false;

        //Metadata rootBasicFCB = new Metadata(RootNode, "ROOT");
        //CreateMap(RootNode, rootBasicFCB);

        InitializeView();
    }

    // 初始化界面
    public void InitializeView()
    {
        //初始化列表
        InitializeListView();
        //初始化目录树
        InitializeTreeView();
        // 得到根
        CurNode = RootNode;
        PathText.Text = "> ROOT\\";
    }

    // 初始化树形界面
    public void InitializeListView()
    {
        FileListView.Items.Clear();
    }

    // 初始化列表
    public void InitializeTreeView()
    {
        FileTreeView.Nodes.Clear();
        rootNode = new TreeNode("ROOT");
        FileTreeView.Nodes.Add(rootNode);
        FileTreeView.ExpandAll();
    }

    // 更新界面
    public void UpdateView()
    {
        UpdateTreeView();
        UpdateListView();
    }

    // 更新树形界面
    public void UpdateTreeView()
    {
        FileTreeView.Nodes.Clear();
        rootNode = new TreeNode("ROOT");
        CreateTreeView(rootNode, RootNode);
        FileTreeView.Nodes.Add(rootNode);
        FileTreeView.ExpandAll();
    }

    // 递归函数生成树形界面
    public void CreateTreeView(TreeNode rootNode, Node curFCB)
    {
        foreach (Node child in curFCB.ChildNode)
        {
            TreeNode childNode = new TreeNode(child.FileName);
            // 如果是txt设置第二个图标 默认是第一个
            if (child.FileType == "txt")
            {
                childNode.ImageIndex = 1;
                childNode.SelectedImageIndex = 1;
            }

            CreateTreeView(childNode, child);
            rootNode.Nodes.Add(childNode);
        }
    }

    // 更新列表界面
    public void UpdateListView()
    {
        listTable = new Dictionary<int, ListViewItem>();
        FileListView.Items.Clear();
        foreach (Node child in CurNode.ChildNode)
        {
            Metadata file = fileDict[child.FileId].Metadata;
            ListViewItem item = new ListViewItem(new string[]
            {
                file.FileName,
                file.ModifiedTime.ToString(),
                file.FileType,
                file.FileSize
            }, file.FileType == "folder" ? 0 : 1);

            listTable[file.FileId] = item;
            // 加入列表视图
            FileListView.Items.Add(item);
        }
    }

    // 构建文件号到Pair的映射
    public void CreateMap(Node item, Metadata file)
    {
        fileDict[item.FileId] = new Pair(item, file);
    }

    // 检查是否重名
    private string CheckSameName(string fileName, string ext = "")
    {
        // 列表记录同名的共多少个
        List<int> sameNameFile = new List<int>();
        // 在当前目录下找是否有重名
        foreach (Node child in CurNode.ChildNode)
        {
            // 文件类型不同
            if (!((child.FileType == "folder" && ext == "") || (child.FileType == ext)))
            {
                continue;
            }

            // 去掉最后一个圆括号及其之后的内容，[^\(]* 表示任意不是左括号的字符可以出现 0 次或多次
            string childFileName = Regex.Replace(child.FileName, @"\(\d+\)[^(\(\d+\))]*$", "");
            // 去掉后缀
            childFileName = Regex.Replace(childFileName, String.Format(@"\.{0}", ext), "");

            if (childFileName == fileName)
            {
                // 获得当前最后一个圆括号，匹配.之前的数字和圆括号
                Match match1;
                if (child.FileType == "folder")
                    match1 = Regex.Match(child.FileName, @"\(\d+\)$", RegexOptions.RightToLeft);
                else
                    match1 = Regex.Match(child.FileName, @"\(\d+\)\.", RegexOptions.RightToLeft);
                // 没有匹配的第0个
                if (!match1.Success)
                {
                    sameNameFile.Add(0);
                    continue;
                }

                // 获取括号中的数字
                Match match2 = Regex.Match(match1.Value, @"\d+");
                // 数字转换为下标
                int idx = int.Parse(match2.Value);
                // 标记为true
                sameNameFile.Add(idx);
            }
        }

        // 出现重名
        for (int i = 0; i < CurNode.ChildNode.Count + 1; ++i)
        {
            if (sameNameFile.Contains(i))
                continue;
            // 找第一个缺的数字，i == 0就不用增加
            if (i != 0)
                fileName += "(" + i.ToString() + ")";
            break;
        }

        return fileName + ((ext == "") ? "" : ("." + ext));
    }

    // 由ListViewItem得到fileId
    public int GetFileId(ListViewItem item)
    {
        foreach (var kv in listTable)
        {
            if (kv.Value.Text == item.Text)
            {
                return kv.Key;
            }
        }

        // 找不到
        MessageBox.Show("FILE NOT FOUND");
        return -1;
    }

    // 新建文件
    private void txtToolStripMenuItem_Click(object sender, EventArgs e)
    {
        changed = true;
        string fileName = CheckSameName("NEW TEXT", "txt");
        string fatherPath;
        // 添加到当前目录下的孩子中
        Node newNode = new Node(fileName, "txt");
        CurNode.AddChildNode(newNode);

        Metadata father = null;
        if (fileDict.ContainsKey(CurNode.FileId))
        {
            father = fileDict[CurNode.FileId].Metadata;
        }

        fatherPath = father == null ? "ROOT" : father.FilePath;
        Metadata @new = new Metadata(newNode, fatherPath);
        CreateMap(newNode, @new);
        UpdateView();
    }

    // 新建文件夹
    private void folderToolStripMenuItem_Click(object sender, EventArgs e)
    {
        changed = true;
        string fileName = CheckSameName("NEW FOLDER");
        string fatherPath;
        // 添加到当前目录下的孩子中
        Node newNode = new Node(fileName, "folder");
        CurNode.AddChildNode(newNode);

        Metadata father = null;
        if (fileDict.ContainsKey(CurNode.FileId))
        {
            father = fileDict[CurNode.FileId].Metadata;
        }

        fatherPath = father == null ? "ROOT" : father.FilePath;
        Metadata @new = new Metadata(newNode, fatherPath);
        CreateMap(newNode, @new);
        UpdateView();
    }

    // 格式化
    private void formatToolStripMenuItem_Click(object sender, EventArgs e)
    {
        fileDict = new Dictionary<int, Pair>();
        RootNode = new Node("ROOT", "folder");
        CurNode = RootNode;
        manager = new Manager();
        fileStack = new Stack<Node>();
        changed = false;
        //Metadata rootBasicFCB = new Metadata(RootNode, "ROOT");
        //CreateMap(RootNode, rootBasicFCB);

        InitializeView();
    }

    // 删除文件点击响应
    private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (FileListView.SelectedItems.Count == 0)
        {
            MessageBox.Show("PLEASE SELECT A FILE OR  FOLDER");
            return;
        }

        changed = true;
        // 找到每个item对应的fileId，从而找到对应数据块、索引块的索引
        foreach (ListViewItem item in FileListView.SelectedItems)
        {
            int fileId = GetFileId(item);
            Metadata bf = fileDict[fileId].Metadata;
            List<int> idxList = bf.FileTable.GetDataIndexList();
            // 取消占用标记
            manager.Remove(idxList);
            // 删除树形结构中的记录
            CurNode.RemoveChildNode(fileDict[fileId].Node);
            // 映射表中移除
            fileDict.Remove(fileId);
        }

        // 更新视图
        UpdateView();
    }

    // 打开文件点击响应
    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (FileListView.SelectedItems.Count != 1)
        {
            MessageBox.Show("PLEASE SELECT A FILE OR  FOLDER");
            return;
        }

        ListViewItem curItem = FileListView.SelectedItems[0];
        int fileId = GetFileId(curItem);
        OpenFile(fileId);
    }

    // 打开文件的操作
    private void OpenFile(int fileId)
    {
        Node sf = fileDict[fileId].Node;
        Metadata bf = fileDict[fileId].Metadata;
        string sizeBefore = bf.FileSize;
        // 根据文件类型判断操作
        if (sf.FileType == "folder")
        {
            // 变换当前视图sym结构根节点
            CurNode = sf;
            PathText.Text = "> " + bf.FilePath;
            // 打开文件夹后可以返回
            BackwardButton.Enabled = true;
            // 打开文件夹后不能前进
            ForwardButton.Enabled = false;
            // 打开新的文件把旧的栈中清除
            fileStack.Clear();
            // 变换列表视图
            UpdateListView();
        }
        else if (sf.FileType == "txt")
        {
            EditWindow txtInputWindow = new EditWindow(sf, bf, fileDict, manager, sizeBefore);
            txtInputWindow.CallBack = UpdateView;
            txtInputWindow.Show();
        }
    }

    // 双击列表视图
    private void listView_DoubleClick(object sender, EventArgs e)
    {
        // 选中的不是一个就不响应
        if (FileListView.SelectedItems.Count != 1)
        {
            return;
        }

        ListViewItem item = FileListView.SelectedItems[0];
        // 获得文件Id
        int fileId = GetFileId(item);
        // 打开文件操作
        OpenFile(fileId);
    }

    // 重命名
    private void renameToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        // 选中不止一个无效
        if (FileListView.SelectedItems.Count != 1)
        {
            return;
        }

        changed = true;
        ListViewItem curItem = FileListView.SelectedItems[0];
        int fileId = GetFileId(curItem);
        Node sf = fileDict[fileId].Node;
        Metadata bf = fileDict[fileId].Metadata;
        RenameWindow renameBox = new RenameWindow(sf, bf, CurNode);
        renameBox.CallBack = UpdateView;
        renameBox.Show();
    }

    // 返回上一级按钮
    private void btn_return_Click(object sender, EventArgs e)
    {
        // 是根目录就不返回了
        if (CurNode.FileId == RootNode.FileId)
        {
            return;
        }

        // 能返回就一定能前进
        ForwardButton.Enabled = true;
        // 加入前进栈
        fileStack.Push(CurNode);
        // 返回上一级
        CurNode = CurNode.FatherNode;
        if (CurNode.FileId == RootNode.FileId)
        {
            PathText.Text = "> ROOT\\";
            // 根目录下不能返回
            BackwardButton.Enabled = false;
        }
        else
        {
            PathText.Text = "> " + fileDict[CurNode.FileId].Metadata.FilePath;
        }

        // 更新列表视图
        UpdateListView();
    }

    // 前进按钮
    private void btn_forward_Click(object sender, EventArgs e)
    {
        if (fileStack.Count == 0)
        {
            return;
        }

        CurNode = fileStack.Pop();
        if (fileStack.Count == 0)
        {
            ForwardButton.Enabled = false;
        }

        PathText.Text = "> " + fileDict[CurNode.FileId].Metadata.FilePath;
        // 能前进必然也能后退
        BackwardButton.Enabled = true;
        UpdateListView();
    }

    // 加载已有文件
    private void loadToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // 先查看当前目录下有无需要的文件 转换为文件名
        var jsonFileList = new List<string>(Directory.GetFiles(curPath, "*.*")
            .Where(s => s.EndsWith(".dat") || s.EndsWith(".txt")).Select(Path.GetFileName));
        //var jsonFileList = Directory.GetFiles(curPath, "*.json").ToList();
        string[] targetFile = new string[] { "fileDict.dat", "RootNode.dat", "manager.dat", "fileCount.txt" };
        foreach (string file in targetFile)
        {
            // 不包含任何一个目标函数
            if (!jsonFileList.Contains(file))
            {
                // 弹出窗口提示并退出
                MessageBox.Show("TARGET FILE NOT FOUND!", "WARNING");
                return;
            }
        }

        // 加载文件
        LoadData();
        // 检查循环引用
        CheckFather();
        // 更新界面
        UpdateView();
    }

    // 保存当前文件
    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SaveData();
        changed = false;
    }

    // 保存数据
    private void SaveData()
    {
        // 缩进保存
        //var options = new JsonSerializerOptions { WriteIndented = true };
        // 处理循环引用
        /*
        var settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Formatting = Formatting.Indented
        };

        // 转换为json格式
        //string jsonFileDict = JsonSerializer.Serialize(fileDict, options);
        string jsonFileDict = JsonConvert.SerializeObject(fileDict, settings);
        // 创建一个新文件，若已存在会覆盖
        File.WriteAllText(Path.Combine(curPath, "fileDict.json"), jsonFileDict);
        // 转换为json格式
        //string jsonRootSymFCB = JsonConvert.SerializeObject(RootNode, settings);
        // 创建一个新文件，若已存在会覆盖
        //File.WriteAllText(Path.Combine(curPath, "RootNode.json"), jsonRootSymFCB);
        // 转换为json格式
        string jsonManager = JsonConvert.SerializeObject(manager, settings);
        // 创建一个新文件，若已存在会覆盖
        File.WriteAllText(Path.Combine(curPath, "manager.json"), jsonManager);
        */
        BinaryFormatter bf = new BinaryFormatter();

        // var fileDictStream = new FileStream(Path.Combine(curPath, "fileDict.dat"), FileMode.Create);
        // bf.Serialize(fileDictStream, fileDict);
        // fileDictStream.Close();
        using (var fileDictStream = new FileStream(Path.Combine(curPath, "fileDict.dat"), FileMode.Create))
        {
            bf.Serialize(fileDictStream, fileDict);
        }

        // var rootSymFCBStream = new FileStream(Path.Combine(curPath, "RootNode.dat"), FileMode.Create);
        // bf.Serialize(rootSymFCBStream, RootNode);
        // rootSymFCBStream.Close();
        using (var rootSymFCBStream = new FileStream(Path.Combine(curPath, "RootNode.dat"), FileMode.Create))
        {
            bf.Serialize(rootSymFCBStream, RootNode);
        }

        // var managerStream = new FileStream(Path.Combine(curPath, "manager.dat"), FileMode.Create);
        // bf.Serialize(managerStream, manager);
        // managerStream.Close();
        using (var managerStream = new FileStream(Path.Combine(curPath, "manager.dat"), FileMode.Create))
        {
            bf.Serialize(managerStream, manager);
        }

        // int类型直接存入txt文件
        // var fileCountStream = new FileStream(Path.Combine(curPath, "fileCount.txt"), FileMode.Create, FileAccess.Write);
        // var sw = new StreamWriter(fileCountStream);
        // sw.WriteLine(Node.counter.ToString());
        // sw.Close();
        // fileCountStream.Close();
        using (var fileCountStream =
               new FileStream(Path.Combine(curPath, "fileCount.txt"), FileMode.Create, FileAccess.Write))
        {
            using (var sw = new StreamWriter(fileCountStream))
            {
                sw.WriteLine(Node.counter.ToString());
            }
        }

        // 提示消息
        MessageBox.Show("Save Successfully\n" + curPath, "Tip");
    }

    private void LoadData()
    {
        /*
        // 处理循环引用
        var settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };
        // 从文件中读取 反序列化
        string jsonFileDict = File.ReadAllText(Path.Combine(curPath, "fileDict.json"));
        // fileDict = JsonSerializer.Deserialize<Dictionary<int, Pair>>(jsonFileDict);
        JsonConvert.PopulateObject(jsonFileDict, fileDict, settings);
        // fileDict = JsonConvert.DeserializeObject<Dictionary<int, Pair>>(jsonFileDict);
        // 从文件中读取 反序列化
        // string jsonRootSymFCB = File.ReadAllText(Path.Combine(curPath, "RootNode.json"));
        // JsonConvert.PopulateObject(jsonRootSymFCB, RootNode, settings);
        // RootNode = JsonConvert.DeserializeObject<Node>(jsonRootSymFCB);
        // 从文件中读取 反序列化
        string jsonManager = File.ReadAllText(Path.Combine(curPath, "manager.json"));
        JsonConvert.PopulateObject(jsonManager, manager, settings);
        // manager = JsonConvert.DeserializeObject<Manager>(jsonManager);
        */

        BinaryFormatter bf = new BinaryFormatter();

        // var fileDictStream = new FileStream(Path.Combine(curPath, "fileDict.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
        // fileDict = bf.Deserialize(fileDictStream) as Dictionary<int, Pair>;
        // fileDictStream.Close();
        using (var fileDictStream = new FileStream(Path.Combine(curPath, "fileDict.dat"), FileMode.Open,
                   FileAccess.Read, FileShare.Read))
        {
            fileDict = bf.Deserialize(fileDictStream) as Dictionary<int, Pair>;
        }

        // var rootSymFCBStream = new FileStream(Path.Combine(curPath, "RootNode.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
        // RootNode = bf.Deserialize(rootSymFCBStream) as Node;
        // rootSymFCBStream.Close();
        using (var rootSymFCBStream = new FileStream(Path.Combine(curPath, "RootNode.dat"), FileMode.Open,
                   FileAccess.Read, FileShare.Read))
        {
            RootNode = bf.Deserialize(rootSymFCBStream) as Node;
        }

        // var managerStream = new FileStream(Path.Combine(curPath, "manager.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
        // manager = bf.Deserialize(managerStream) as Manager;
        // managerStream.Close();
        using (var managerStream = new FileStream(Path.Combine(curPath, "manager.dat"), FileMode.Open,
                   FileAccess.Read, FileShare.Read))
        {
            manager = bf.Deserialize(managerStream) as Manager;
        }

        // var fileCountStream = new FileStream(Path.Combine(curPath, "fileCount.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
        // Node.counter = int.Parse(bf.Deserialize(fileCountStream) as string);
        // fileCountStream.Close();
        string res = "";
        using (StreamReader sr = new StreamReader(Path.Combine(curPath, "fileCount.txt")))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                res += line;
            }
        }

        Node.counter = int.Parse(res);

        // 初始化
        InitializeView();
        MessageBox.Show("Load successfully", "Tip");
    }

    private void CheckFather()
    {
        foreach (Node child in RootNode.ChildNode)
        {
            child.FatherNode = RootNode;
        }
    }

    private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
    {
        // 关闭窗口时弹出消息窗口是否保存
        if (changed && MessageBox.Show("Do you want save the data?", "Tip", MessageBoxButtons.YesNo) ==
            DialogResult.Yes)
        {
            SaveData();
        }
    }


    private void MainWindow_Load(object sender, EventArgs e)
    {
    }

    private void fileToolStripMenuItem_Click(object sender, EventArgs e)
    {
    }

    private void cur_path_text_TextChanged(object sender, EventArgs e)
    {
    }

    private void FileListView_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    private void FileTreeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
    }
}