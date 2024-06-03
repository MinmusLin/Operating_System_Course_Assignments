using Newtonsoft.Json;

namespace File_Management.Classes;

[Serializable]
public class Node
{
#pragma warning disable CA2211
    public static int counter;
#pragma warning restore CA2211
    public int FileId;
    public string FileName = null!;
    public string FileType;
    public Node FatherNode;
    public List<Node> ChildNode;

    public Node()
    {
        FileId = counter++;
        FileType = "文件夹";
        FatherNode = null!;
        ChildNode = new List<Node>();
    }

    public Node(string fileName, string fileType)
    {
        FileId = counter++;
        FileName = fileName;
        FileType = fileType;
        FatherNode = null!;
        ChildNode = new List<Node>();
    }

    [JsonConstructor]
    public Node(string fileName, string fileType, int fileId, Node fatherNode, List<Node> childNode)
    {
        FileName = fileName;
        FileType = fileType;
        FileId = fileId;
        FatherNode = fatherNode;
        ChildNode = childNode;
    }

    public void AddChildNode(Node childNode)
    {
        ChildNode.Add(childNode);
        childNode.FatherNode = this;
    }

    public void RemoveChildNode(Node childNode)
    {
        ChildNode.Remove(childNode);
    }
}