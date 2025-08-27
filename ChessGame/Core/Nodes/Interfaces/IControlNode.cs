using Core.Components;

namespace Core.Nodes.Interfaces;

public interface IControlNode
{
    RectTransform Transform { get; }
    bool ClipsToBounds { get; set; }
}