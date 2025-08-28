using System;
using System.Collections.Generic;
using System.Linq;
using Core.Nodes.Interfaces;
using Core.Nodes.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

/// <summary>
/// Represents a basic state.
/// </summary>
public abstract class State
{
    private readonly List<INode> nodes = [];

    /// <summary>
    /// The collection of nodes managed by this instance.
    /// </summary>
    public IReadOnlyList<INode> Nodes => nodes;

    public State(Game game)
    {
        
    }
    
    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public virtual void OnEnter(StateMachine machine, Game game)
    {
        OnResize(new Vector2(game.Window.ClientBounds.Width, game.Window.ClientBounds.Height));
    }

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public virtual void OnExit(StateMachine machine, Game game)
    {
    }

    /// <summary>
    /// Called when the game window is resized.
    /// </summary>
    /// <param name="size">The new game window size.</param>
    public virtual void OnResize(Vector2 size)
    {
        foreach (var node in nodes.OfType<CanvasNode>()) node.Transform.Size = size;
    }

    /// <summary>
    /// Called when the state should draw itself and its nodes.
    /// </summary>
    /// <param name="time">The game time.</param>
    public virtual void Draw(StateMachine machine, Game game, GameTime time)
    {
        machine.SpriteBatch.Begin(
            SpriteSortMode.Immediate,
            BlendState.AlphaBlend,
            rasterizerState: new RasterizerState
            {
                ScissorTestEnable = true
            }
        );

        foreach (var node in nodes.OrderBy(c => c.Layer))
        {
            if (node.IsActive)
            {
                var oldClipRect = Rectangle.Empty;
                var clipWasChanged = false;
                if (node is ControlNode { ClipsToBounds: true } control)
                {
                    var rect = control.Transform.AncestorRectTransform?.GlobalBounds;
                    if (rect is not null)
                    {
                        oldClipRect = machine.SpriteBatch.GraphicsDevice.ScissorRectangle;
                        machine.SpriteBatch.GraphicsDevice.ScissorRectangle = rect.Value;
                        clipWasChanged = true;
                    }
                }
                node.Draw(new StateContext(machine, this, game), time);
                if (clipWasChanged)
                    machine.SpriteBatch.GraphicsDevice.ScissorRectangle = oldClipRect;
            }
        }

        machine.SpriteBatch.End();
    }

    /// <summary>
    /// Called when the state should update itself and its nodes.
    /// </summary>
    /// <param name="time">The game time.</param>
    public virtual void Update(StateMachine machine, Game game, GameTime time)
    {
        foreach (var node in nodes)
            if (node.IsActive)
            {
                if (node is ControlNode control)
                {
                    // TODO: Update input for control nodes
                }

                node.Update(new StateContext(machine, this, game), time);
            }
    }

    /// <summary>
    /// Adds a node to the state.
    /// </summary>
    /// <param name="node">The node to add. Cannot be <see langword="null" />.</param>
    /// <exception cref="InvalidOperationException">Thrown if the <paramref name="node" /> is already added to this state.</exception>
    public void AddNode(INode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (nodes.Contains(node))
            throw new InvalidOperationException("Node is already added to this state.");
        nodes.Add(node);
    }

    /// <summary>
    /// Removes a node from the state.
    /// </summary>
    /// <param name="node">The node to add. Cannot be <see langword="null" />.</param>
    /// <exception cref="InvalidOperationException">Thrown if the <paramref name="node" /> is not in this state.</exception>
    public void RemoveNode(INode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (!nodes.Contains(node))
            throw new InvalidOperationException("Node is not part of this state.");
        nodes.Remove(node);
    }
}