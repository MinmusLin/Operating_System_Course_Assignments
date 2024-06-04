namespace File_Management.Classes;

// 文件索引类
[Serializable]
public class Index
{
    public List<int> IndexList = []; // 索引列表
    public const int Capacity = 256; // 容量

    // 判断索引列表是否已满
    public bool IsFull()
    {
        return IndexList.Count >= Capacity;
    }

    // 添加索引
    public bool AddIndex(int idx)
    {
        if (IsFull()) return false;
        IndexList.Add(idx);
        return true;
    }
}