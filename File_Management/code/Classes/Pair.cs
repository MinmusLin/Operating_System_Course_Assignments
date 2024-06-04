using Newtonsoft.Json;

namespace File_Management.Classes;

// 目录节点与文件元数据关联类
[Serializable]
[method: JsonConstructor]
public class Pair(Node node, Metadata metadata)
{
    public Node Node = node; // 目录节点
    public Metadata Metadata = metadata; // 文件元数据
}