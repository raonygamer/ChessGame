using Microsoft.Xna.Framework;

namespace Core.Nodes.Interfaces;

/// <summary>
/// The base interface for all nodes.
/// </summary>
public interface INode
{
    /// <summary>
    /// Whether this node is active. Inactive nodes will not be updated or drawn.
    /// </summary>
    bool IsActive { get; set; }

    /// <summary>
    /// The layer of this node. Nodes with higher layer values are drawn and updated on top of nodes with lower layer
    /// values.
    /// </summary>
    int Layer { get; set; }

    /// <summary>
    /// Called when the node should draw itself.
    /// </summary>
    /// <param name="time">Current game time.</param>
    void Draw(StateContext ctx, GameTime time);

    /// <summary>
    /// Called when the node should update itself.
    /// </summary>
    /// <param name="time">Current game time.</param>
    void Update(StateContext ctx, GameTime time);
}