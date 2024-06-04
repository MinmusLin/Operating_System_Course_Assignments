using Newtonsoft.Json;

namespace File_Management.Classes;

// 文件元数据类
[Serializable]
public class Metadata
{
    public int FileId; // 文件 ID
    public string FileName; // 文件名
    public string FileSize; // 文件大小
    public string FileType = ""; // 文件类型
    public string FilePath = ""; // 文件路径
    public DateTime ModifiedTime; // 修改时间
    public Table FileIndexTable; // 文件索引表

    // 构造函数
    public Metadata(string fileName, string fileSize)
    {
        FileName = fileName;
        FileSize = fileSize;
        ModifiedTime = DateTime.Now;
        FileIndexTable = new Table();
    }

    // 构造函数
    public Metadata(Node node, string path = "")
    {
        FileId = node.FileId;
        FileName = node.FileName;
        FileSize = "0";
        FileType = node.FileType;
        FilePath = path + "\\" + FileName;
        ModifiedTime = DateTime.Now;
        FileIndexTable = new Table();
    }

    // 构造函数
    [JsonConstructor]
    public Metadata(int fileId, string fileName, string fileType, string fileSize, string filePath,
        DateTime modifiedTime, Table fileIndexTable)
    {
        FileId = fileId;
        FileName = fileName;
        FileType = fileType;
        FileSize = fileSize;
        FilePath = filePath;
        ModifiedTime = modifiedTime;
        FileIndexTable = fileIndexTable;
    }
}