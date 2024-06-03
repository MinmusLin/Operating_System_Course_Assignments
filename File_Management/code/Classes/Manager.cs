using Newtonsoft.Json;

namespace File_Management.Classes;

[Serializable]
public class Manager
{
    public const int Capacity = 1000000;
    private Block[] blocks;
    private bool[] bitMap;
    private int bitIndex;

    [JsonConstructor]
    public Manager()
    {
        blocks = new Block[Capacity];
        bitMap = new bool[Capacity];
        bitIndex = 0;
        for (var i = 0; i < Capacity; i++) bitMap[i] = true;
    }

    public Block GetBlock(int idx)
    {
        return blocks[idx];
    }

    public string GetBlockInfo(int idx)
    {
        return blocks[idx].Read();
    }

    public int AllocateBlock()
    {
        bitIndex %= Capacity;
        var tempIdx = bitIndex;
        while (true)
        {
            if (bitMap[tempIdx])
            {
                blocks[tempIdx] = new Block();
                bitIndex = tempIdx + 1;
                return tempIdx;
            }

            tempIdx = (tempIdx + 1) % Capacity;
            if (tempIdx == bitIndex) break;
        }

        return -1;
    }

    public void Remove(int idx)
    {
        bitMap[idx] = true;
        foreach (var index in blocks[idx].GetIndex()) bitMap[index] = true;
    }

    public void Remove(List<int> indexList)
    {
        foreach (int index in indexList) Remove(index);
    }

    public Table Write(string data)
    {
        var table = new Table();
        int emptyIndex;
        while (data.Length > Block.Capacity)
        {
            emptyIndex = AllocateBlock();
            blocks[emptyIndex].Write(data[..Block.Capacity]);
            if (!table.IsDataListFull())
            {
                table.AddDataIndex(emptyIndex);
            }
            else
            {
                var flag = false;
                foreach (var index in table.IndexIndexList.Where(index => !blocks[index].IsIndexFull()))
                {
                    blocks[index].SetIndex(emptyIndex);
                    flag = true;
                    break;
                }

                if (!flag)
                {
                    var indexEmptyIndex = AllocateBlock();
                    blocks[indexEmptyIndex].SetIndex(emptyIndex);
                    table.AddIndexIndex(indexEmptyIndex);
                }
            }

            data = data.Remove(0, Block.Capacity);
        }

        emptyIndex = AllocateBlock();
        blocks[emptyIndex].Write(data);
        if (!table.IsDataListFull())
        {
            table.AddDataIndex(emptyIndex);
        }
        else
        {
            var flag = false;
            foreach (var index in table.IndexIndexList.Where(index => !blocks[index].IsIndexFull()))
            {
                blocks[index].SetIndex(emptyIndex);
                flag = true;
                break;
            }

            if (flag) return table;
            var indexEmptyIndex = AllocateBlock();
            blocks[indexEmptyIndex].SetIndex(emptyIndex);
            table.AddIndexIndex(indexEmptyIndex);
        }

        return table;
    }
}