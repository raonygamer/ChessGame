using Core.Components;

namespace Core.Nodes.Interfaces;

public interface IControlNode : INode
{
    RectTransform Transform { get; }
    bool ClipsToBounds { get; set; }
}