namespace File_Management.Classes;

[Serializable]
public class Index
{
    public List<int> IndexList = [];
    public const int Capacity = 256;

    public bool IsFull()
    {
        return IndexList.Count >= Capacity;
    }

    public bool AddIndex(int idx)
    {
        if (IsFull())
        {
            return false;
        }

        IndexList.Add(idx);
        return true;
    }
}