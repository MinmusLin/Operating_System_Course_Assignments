using Newtonsoft.Json;

namespace File_Management.Classes;

// 文件索引表类
[Serializable]
[method: JsonConstructor]
public class Table(List<int> dataIndexList, List<int> indexIndexList)
{
    public List<int> DataIndexList = dataIndexList; // 数据索引列表
    public List<int> IndexIndexList = indexIndexList; // 索引索引列表
    private const int DataIndexCapacity = 10; // 数据索引列表容量
    private const int IndexIndexCapacity = 3; // 索引索引列表容量

    // 构造函数
    public Table() : this(new List<int>(DataIndexCapacity), new List<int>(IndexIndexCapacity))
    {
    }

    // 判断数据索引列表是否已满
    public bool IsDataListFull()
    {
        return DataIndexList.Count >= DataIndexCapacity;
    }

    // 判断索引索引列表是否已满
    public bool IsIndexListFull()
    {
        return IndexIndexList.Count >= DataIndexCapacity;
    }

    // 添加数据索引
    public void AddDataIndex(int idx)
    {
        DataIndexList.Add(idx);
    }

    // 添加索引索引
    public void AddIndexIndex(int idx)
    {
        IndexIndexList.Add(idx);
    }

    // 获取数据索引列表
    public List<int> GetDataIndexList()
    {
        return IndexIndexList.Count == 0 ? DataIndexList : DataIndexList.Concat(IndexIndexList).ToList();
    }
}