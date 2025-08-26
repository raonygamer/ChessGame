using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Represents a basic state.
    /// </summary>
    public abstract class State
    {
        private readonly List<Node> nodes = [];
        /// <summary>
        /// The collection of nodes managed by this instance.
        /// </summary>
        public IReadOnlyList<Node> Nodes => nodes;

        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public virtual void OnEnter() { }

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public virtual void OnExit() { }

        /// <summary>
        /// Called when the state should draw itself and its nodes.
        /// </summary>
        /// <param name="time">The game time.</param>
        public virtual void Draw(StateMachine machine, Game game, GameTime time)
        {
            machine.SpriteBatch.Begin();
            foreach (var node in nodes)
            {
                if (node.IsActive)
                    node.Draw(this, machine, game, time);
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
            {
                if (node.IsActive)
                    node.Update(this, machine, game, time);
            }
        }

        /// <summary>
        /// Adds a node to the state.
        /// </summary>
        /// <param name="node">The node to add. Cannot be <see langword="null"/>.</param>
        /// <exception cref="InvalidOperationException">Thrown if the <paramref name="node"/> is already added to this state.</exception>
        public void AddNode(Node node)
        {
            ArgumentNullException.ThrowIfNull(node);
            if (nodes.Contains(node))
                throw new InvalidOperationException("Node is already added to this state.");
            nodes.Add(node);
        }

        /// <summary>
        /// Removes a node from the state.
        /// </summary>
        /// <param name="node">The node to add. Cannot be <see langword="null"/>.</param>
        /// <exception cref="InvalidOperationException">Thrown if the <paramref name="node"/> is not in this state.</exception>
        public void RemoveNode(Node node)
        {
            ArgumentNullException.ThrowIfNull(node);
            if (!nodes.Contains(node))
                throw new InvalidOperationException("Node is not part of this state.");
            nodes.Remove(node);
        }
    }
}
