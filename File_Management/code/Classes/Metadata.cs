using Newtonsoft.Json;

namespace File_Management.Classes;

[Serializable]
public class Metadata
{
    public int FileId;
    public string FileName;
    public string FileSize;
    public string FileType = "";
    public string FilePath = "";
    public DateTime ModifiedTime;
    public Table FileTable;

    public Metadata(string fileName, string fileSize)
    {
        FileName = fileName;
        FileSize = fileSize;
        ModifiedTime = DateTime.Now;
        FileTable = new Table();
    }

    public Metadata(Node node, string path = "")
    {
        FileId = node.FileId;
        FileName = node.FileName;
        FileSize = "0";
        FileType = node.FileType;
        FilePath = path + "\\" + FileName;
        ModifiedTime = DateTime.Now;
        FileTable = new Table();
    }

    [JsonConstructor]
    public Metadata(int fileId, string fileName, string fileType, string fileSize, string filePath,
        DateTime modifiedTime, Table fileTable)
    {
        FileId = fileId;
        FileName = fileName;
        FileType = fileType;
        FileSize = fileSize;
        FilePath = filePath;
        ModifiedTime = modifiedTime;
        FileTable = fileTable;
    }
}