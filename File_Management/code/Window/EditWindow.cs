using System.Text.RegularExpressions;
using File_Management.Classes;

namespace File_Management.Window;

public partial class EditWindow : Form
{
    // 修改标识
    bool changed;
    // 该文件的FCB
    private Metadata bf;
    // 
    private Node sf;
    // 
    private Dictionary<int, Pair> fileDict;
    // 为了管理blocks
    private Manager manager;
    // 编辑前的大小
    private string sizeBefore;
    // ?
    public DelegateMethod.delegateFunction CallBack;

    public EditWindow()
    {
        InitializeComponent();
    }
    public EditWindow(Node sf, Metadata bf, Dictionary<int, Pair> fileDict, Manager manager, string sizeBefore)
    {
        InitializeComponent();
        this.bf = bf;
        this.sf = sf;
        this.fileDict = fileDict;
        this.manager = manager;
        // 展示磁盘对应内容
        ShowText();
        // 窗口标题显示文件名
        base.Text = bf.FileName;
        // 没修改
        changed = false;
        // 编辑前的大小
        this.sizeBefore = sizeBefore;
    }
    private void ShowText()
    {
        // 得到该文件的所有索引
        List<int> indexList = bf.FileTable.GetDataIndexList();
        string text = "";
        foreach (int idx in indexList)
        {
            // 数据块
            if (manager.GetBlock(idx).GetIndex().Count == 0)
            {
                text += manager.GetBlockInfo(idx);
            }
            else
            {
                // 读索引块中索引对应数据块
                foreach (int index in manager.GetBlock(idx).GetIndex())
                {
                    text += manager.GetBlockInfo(index);
                }
            }
        }
        // 设置到文本
        Text.Text = text;
    }
    private void WriteData()
    { 
        string text = Text.Text;
        // 计算文件大小
        string ext = "B";
        long size = text.Length * 4;
        // 单位升级
        /*
        if (size > 1024)
        {
            size /= 1024;
            ext = "KB";
        }
        if (size > 1024)
        {
            size /= 1024;
            ext = "MB";
        }
        */
        bf.FileSize = size.ToString() + ext;
        // 释放之前的磁盘块
        FreeBlock();
        // 重新写数据 返回新的索引表
        bf.FileTable = manager.Write(text);
    }
    private void UpdateFile(string sizeBefore, string sizeAfter)
    {
        // 大小差值
        int delta = int.Parse(Regex.Match(sizeAfter, @"\d+").Value)
                    - int.Parse(Regex.Match(sizeBefore, @"\d+").Value);
        // 不用改变
        //if (delta == 0)
        //{
        //    return;
        //}
        Node curSf = sf.FatherNode;
        // 找到根节点停止
        while (fileDict.ContainsKey(curSf.FileId))
        {
            Metadata curBf = fileDict[curSf.FileId].Metadata;
            curBf.ModifiedTime = DateTime.Now;
            int newSize = int.Parse(Regex.Match(curBf.FileSize, @"\d+").Value) + delta;
            curBf.FileSize = newSize.ToString() + 
                             (Regex.Match(curBf.FileSize, @"\D+").Value == "" ? "B" : Regex.Match(curBf.FileSize, @"\D+").Value);

            curSf = curSf.FatherNode;
        }
    }

    private void MyCallBack()
    {
        if (CallBack != null)
        {
            CallBack();
        }
    }
    private void FreeBlock()
    {
        // 得到索引表中所有索引
        List<int> indexList = bf.FileTable.GetDataIndexList();
        // 删除索引表以及索引数据
        manager.Remove(indexList);
    }

    private void TxtInputWindow_FormClosing(object sender, FormClosingEventArgs e)
    {
        // 弹出消息窗口确认是否保存
        if (changed && MessageBox.Show("Do you want to save changes", "Tip", MessageBoxButtons.YesNo)
            == DialogResult.Yes)
        {
            bf.ModifiedTime = DateTime.Now;
            WriteData();
            UpdateFile(sizeBefore, bf.FileSize);
            MyCallBack();
        }
    }
    // 改变后加个星星 才要保存
    private void textBox_TextChanged(object sender, EventArgs e)
    {
        // 没有修改过才加星星
        if (!changed)
        {
            base.Text += "*";
            changed = true;
        }
            
    }
}
public class DelegateMethod
{
    public delegate void delegateFunction();
}