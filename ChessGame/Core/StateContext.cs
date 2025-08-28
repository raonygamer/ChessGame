using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

/// <summary>
/// Represents the context in which nodes operate.
/// </summary>
public readonly struct StateContext(StateMachine machine, State state, Game game)
{
    /// <summary>
    /// The state machine managing the current state.
    /// </summary>
    public readonly StateMachine Machine = machine;

    /// <summary>
    /// The current state in which the node is operating.
    /// </summary>
    public readonly State State = state;

    /// <summary>
    /// The game instance.
    /// </summary>
    public readonly Game Game = game;

    /// <summary>
    /// The graphics device.
    /// </summary>
    public GraphicsDevice Graphics => Game.GraphicsDevice;

    /// <summary>
    /// The sprite batch used for rendering.
    /// </summary>
    public SpriteBatch SpriteBatch => Machine.SpriteBatch;
}