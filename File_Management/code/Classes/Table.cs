using Newtonsoft.Json;

namespace File_Management.Classes;

[Serializable]
[method: JsonConstructor]
public class Table(List<int> dataIndexList, List<int> indexIndexList)
{
    public List<int> DataIndexList = dataIndexList;
    public List<int> IndexIndexList = indexIndexList;
    private const int DataIndexCapacity = 10;
    private const int IndexIndexCapacity = 3;

    public Table() : this(new List<int>(DataIndexCapacity), new List<int>(IndexIndexCapacity))
    {
    }

    public bool IsDataListFull()
    {
        return DataIndexList.Count >= DataIndexCapacity;
    }

    public bool IsIndexListFull()
    {
        return IndexIndexList.Count >= DataIndexCapacity;
    }

    public void AddDataIndex(int idx)
    {
        DataIndexList.Add(idx);
    }

    public void AddIndexIndex(int idx)
    {
        IndexIndexList.Add(idx);
    }

    public List<int> GetDataIndexList()
    {
        return IndexIndexList.Count == 0 ? DataIndexList : DataIndexList.Concat(IndexIndexList).ToList();
    }
}