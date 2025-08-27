using Core.Components;
using Core.Nodes.Interfaces;
using Microsoft.Xna.Framework;

namespace Core.Nodes;

/// <summary>
///     Represents a 2D node with position, rotation, scale and origin.
/// </summary>
public class Node2D : INode
{
    protected Transform2D transform;

    /// <summary>
    ///     Constructs a new <see cref="Node2D" /> object with a <see cref="Transform2D" />.
    /// </summary>
    public Node2D()
    {
        transform = new Transform2D(this);
    }

    /// <summary>
    ///     The 2D transform of this node.
    /// </summary>
    public Transform2D Transform => transform;

    public bool IsActive { get; set; } = true;
    public int Layer { get; set; } = 0;

    public virtual void Draw(StateContext ctx, GameTime time)
    {
    }

    public virtual void Update(StateContext ctx, GameTime time)
    {
    }
}