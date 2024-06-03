using System.Text.RegularExpressions;
using File_Management.Classes;

namespace File_Management.Window;

public partial class RenameWindow : Form
{
    private readonly Node sourceNode = null!;
    private readonly Node currentNode = null!;
    private readonly Metadata fileMetadata = null!;
    public DelegateMethod.DelegateFunction UpdateCallback = null!;

    // 构造函数
    public RenameWindow()
    {
        InitializeComponent();
    }

    // 构造函数
    public RenameWindow(Node sourceNode, Metadata fileMetadata, Node currentNode)
    {
        InitializeComponent();
        this.sourceNode = sourceNode;
        this.fileMetadata = fileMetadata;
        this.currentNode = currentNode;
    }

    // 保存按钮鼠标单击响应函数
    private void SaveButtonClick(object sender, EventArgs e)
    {
        fileMetadata.ModifiedTime = DateTime.Now;
        var fileName = InputText.Text;
        var sameNameFile = new List<int>();
        var name = fileName;
        foreach (var match in from child in currentNode.ChildNode
                 where child.FileType == fileMetadata.FileType
                 let childFileName = Regex.Replace(child.FileName, $"{Regex4()}|\\.txt$", "")
                 where childFileName == name
                 select child.FileType == "文件夹" ? Regex2().Match(child.FileName) : Regex3().Match(child.FileName))
        {
            if (!match.Success)
            {
                sameNameFile.Add(0);
                continue;
            }

            var index = int.Parse(Regex1().Match(match.Value).Value);
            sameNameFile.Add(index);
        }

        for (var i = 0; i < currentNode.ChildNode.Count + 1; i++)
        {
            if (sameNameFile.Contains(i)) continue;
            if (i != 0) fileName += "(" + i + ")";
            break;
        }

        if (fileMetadata.FileType == "文本文件") fileName += ".txt";
        fileMetadata.FileName = fileName;
        sourceNode.FileName = fileName;
        UpdateCallback();
        Close();
    }

    // 取消按钮鼠标单击响应函数
    private void CancelButtonClick(object sender, EventArgs e)
    {
        Close();
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