using Core.Components;
using Core.Nodes.Interfaces;

namespace Core.Nodes.UI;

/// <summary>
/// Represents a control node for UI elements.
/// </summary>
public class ControlNode : Node2D, IControlNode
{
    public ControlNode()
    {
        transform = new RectTransform(this);
    }

    /// <summary>
    /// The RectTransform of this control node.
    /// </summary>
    public new RectTransform Transform => (RectTransform)base.Transform;

    /// <summary>
    /// Specifies whether to clip this node to the bounds of the parent node.
    /// </summary>
    public bool ClipsToBounds { get; set; } = false;
}