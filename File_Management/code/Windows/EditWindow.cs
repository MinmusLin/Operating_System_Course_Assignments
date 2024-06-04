using System.Text.RegularExpressions;
using File_Management.Classes;

namespace File_Management.Windows;

// 文本编辑窗口类
public partial class EditWindow : Form
{
    private bool changedFlag; // 修改标记
    private readonly Node node = null!; // 目录节点
    private readonly Metadata fileMetadata = null!; // 文件元数据
    private readonly Dictionary<int, Pair> fileDictionary = null!; // 文件字典
    private readonly Manager manager = null!; // 空间分配管理
    private readonly string fileSize = null!; // 文件大小
    public DelegateMethod.DelegateFunction UpdateCallback = null!; // 更新回调

    // 构造函数
    public EditWindow()
    {
        InitializeComponent();
    }

    // 构造函数
    public EditWindow(Node node, Metadata fileMetadata, Dictionary<int, Pair> fileDictionary, Manager manager,
        string fileSize)
    {
        InitializeComponent();
        this.node = node;
        this.fileMetadata = fileMetadata;
        this.fileDictionary = fileDictionary;
        this.manager = manager;
        ReadText();
        base.Text = fileMetadata.FileName;
        changedFlag = false;
        this.fileSize = fileSize;
    }

    // 读文本文件
    private void ReadText()
    {
        var indexList = fileMetadata.FileIndexTable.GetDataIndexList();
        var text = "";
        foreach (var index in indexList)
        {
            if (manager.GetBlock(index).GetIndex().Count == 0)
            {
                text += manager.GetBlock(index).Read();
            }
            else
            {
                text = manager.GetBlock(index).GetIndex()
                    .Aggregate(text, (current, idx) => current + manager.GetBlock(idx).Read());
            }
        }

        Text.Text = text;
    }

    // 写文本文件
    private void WriteText()
    {
        var text = Text.Text;
        fileMetadata.FileSize = text.Length * 4 + "B";
        manager.Remove(fileMetadata.FileIndexTable.GetDataIndexList());
        fileMetadata.FileIndexTable = manager.Write(text);
    }

    // 更新文本文件大小
    private void UpdateSize(string sizeBefore, string sizeAfter)
    {
        var delta = int.Parse(Regex1().Match(sizeAfter).Value) - int.Parse(Regex2().Match(sizeBefore).Value);
        var fatherNode = node.FatherNode;
        while (fileDictionary.ContainsKey(fatherNode.FileId))
        {
            var currentNode = fileDictionary[fatherNode.FileId].Metadata;
            currentNode.ModifiedTime = DateTime.Now;
            var newSize = int.Parse(Regex3().Match(currentNode.FileSize).Value) + delta;
            currentNode.FileSize = newSize + (Regex4().Match(currentNode.FileSize).Value == ""
                ? "B"
                : Regex5().Match(currentNode.FileSize).Value);
            fatherNode = fatherNode.FatherNode;
        }
    }

    // 文本内容更改响应函数
    private void TextChange(object sender, EventArgs e)
    {
        if (changedFlag) return;
        base.Text += @"*";
        changedFlag = true;
    }

    // 文本编辑窗口关闭响应函数
    private void EditWindowClose(object sender, FormClosingEventArgs e)
    {
        if (!changedFlag || MessageBox.Show(@"是否保存文本文件？", @"提示", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        fileMetadata.ModifiedTime = DateTime.Now;
        WriteText();
        UpdateSize(fileSize, fileMetadata.FileSize);
        UpdateCallback();
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex Regex1();

    [GeneratedRegex(@"\d+")]
    private static partial Regex Regex2();

    [GeneratedRegex(@"\d+")]
    private static partial Regex Regex3();

    [GeneratedRegex(@"\D+")]
    private static partial Regex Regex4();

    [GeneratedRegex(@"\D+")]
    private static partial Regex Regex5();
}