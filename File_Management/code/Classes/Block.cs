namespace File_Management.Classes;

// 文件存储块类
[Serializable]
public class Block
{
    public const int Capacity = 16; // 容量
    private char[] info = new char[Capacity]; // 信息
    private int length; // 长度
    private Index index = new(); // 索引

    // 写操作
    public void Write(string data)
    {
        length = data.Length > Capacity ? Capacity : data.Length;
        for (var i = 0; i < length; i++) info[i] = data[i];
    }

    // 读操作
    public string Read()
    {
        return new string(info);
    }

    // 判断索引是否已满
    public bool IsIndexFull()
    {
        return index.IsFull();
    }

    // 设置索引
    public bool SetIndex(int idx)
    {
        if (index.IsFull()) return false;
        index.AddIndex(idx);
        return true;
    }

    // 获取索引
    public List<int> GetIndex()
    {
        return index.IndexList;
    }
}