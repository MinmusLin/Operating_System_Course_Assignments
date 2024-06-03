using Newtonsoft.Json;

namespace File_Management.Classes;

[Serializable]
[method: JsonConstructor]
public class Pair(Node node, Metadata metadata)
{
    public Node Node = node;
    public Metadata Metadata = metadata;
}