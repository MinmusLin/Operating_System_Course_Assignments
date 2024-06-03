namespace File_Management.Classes;

[Serializable]
public class Block
{
    public const int Capacity = 16;
    private char[] info = new char[Capacity];
    private int length;
    private Index index = new();

    public void Write(string data)
    {
        length = data.Length > Capacity ? Capacity : data.Length;
        for (var i = 0; i < length; i++)
        {
            info[i] = data[i];
        }
    }

    public string Read()
    {
        return new string(info);
    }

    public bool IsIndexFull()
    {
        return index.IsFull();
    }

    public bool SetIndex(int idx)
    {
        if (index.IsFull())
        {
            return false;
        }

        index.AddIndex(idx);
        return true;
    }

    public List<int> GetIndex()
    {
        return index.IndexList;
    }
}