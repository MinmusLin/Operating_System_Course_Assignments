using Newtonsoft.Json;

namespace File_Management.Classes;

// 目录节点类
[Serializable]
public class Node
{
#pragma warning disable CA2211
    public static int counter; // 计数器
#pragma warning restore CA2211
    public int FileId; // 文件 ID
    public string FileName = null!; // 文件名
    public string FileType; // 文件类型
    public Node FatherNode; // 文件父节点
    public List<Node> ChildNode; // 文件子节点

    // 构造函数
    public Node()
    {
        FileId = counter++;
        FileType = "文件夹";
        FatherNode = null!;
        ChildNode = new List<Node>();
    }

    // 构造函数
    public Node(string fileName, string fileType)
    {
        FileId = counter++;
        FileName = fileName;
        FileType = fileType;
        FatherNode = null!;
        ChildNode = new List<Node>();
    }

    // 构造函数
    [JsonConstructor]
    public Node(string fileName, string fileType, int fileId, Node fatherNode, List<Node> childNode)
    {
        FileName = fileName;
        FileType = fileType;
        FileId = fileId;
        FatherNode = fatherNode;
        ChildNode = childNode;
    }

    // 添加文件子节点
    public void AddChildNode(Node childNode)
    {
        ChildNode.Add(childNode);
        childNode.FatherNode = this;
    }

    // 移除文件子节点
    public void RemoveChildNode(Node childNode)
    {
        ChildNode.Remove(childNode);
    }
}