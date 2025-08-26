using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Represents a basic state machine.
    /// </summary>
    public class StateMachine(Game game, SpriteBatch spriteBatch)
    {
        /// <summary>
        /// The game instance associated with this state machine.
        /// </summary>
        public readonly Game Game = game;

        /// <summary>
        /// The sprite batch used for rendering.
        /// </summary>
        public readonly SpriteBatch SpriteBatch = spriteBatch;

        private readonly Dictionary<string, State> states = [];
        /// <summary>
        /// The collection of states managed by this instance.
        /// </summary>
        public IReadOnlyDictionary<string, State> States => states;

        /// <summary>
        /// The current active state or null if there is no active state.
        /// </summary>
        public (string Name, State State)? Current { get; private set; } = null;

        /// <summary>
        /// Adds a state to the state machine.
        /// </summary>
        /// <param name="name">Name of the state to add.</param>
        /// <param name="state">The state to add.</param>
        /// <exception cref="InvalidOperationException">If the state already exists with the same name.</exception>
        public void AddState(string name, State state)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(state);
            if (states.ContainsKey(name))
                throw new InvalidOperationException($"State with name '{name}' is already added.");
            states[name] = state;
        }

        /// <summary>
        /// Removes a state from the state machine.
        /// </summary>
        /// <param name="name">Name of the state to remove.</param>
        public void RemoveState(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            if (!states.ContainsKey(name))
                throw new InvalidOperationException($"State with name '{name}' does not exist.");
            if (Current?.Name == name)
                Current = null;
            states.Remove(name);
        }

        /// <summary>
        /// Changes the current state to the state with the given name.
        /// </summary>
        /// <param name="name">Name of the state to change to.</param>
        /// <exception cref="InvalidOperationException">State not found.</exception>
        public void ChangeState(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            if (!states.TryGetValue(name, out State? value))
                throw new InvalidOperationException($"State with name '{name}' does not exist.");
            Current?.State.OnExit(this, game);
            Current = (name, value);
            Current?.State.OnEnter(this, game);
        }

        public void Draw(Game game, GameTime time)
        {
            if (Current == null)
                return;
            Current.Value.State.Draw(this, game, time);
        }

        public void Update(Game game, GameTime time)
        {
            if (Current == null)
                return;
            Current.Value.State.Update(this, game, time);
        }

        public void OnResize(Vector2 size)
        {
            if (Current == null)
                return;
            Current.Value.State.OnResize(size);
        }
    }
}
