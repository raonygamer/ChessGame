using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Represents the most basic node.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// The parent of this node. Can be null.
        /// </summary>
        public Node? Parent { get; private set; }

        private readonly List<Node> children = [];
        /// <summary>
        /// Contains all the children of this node.
        /// </summary>
        public IReadOnlyList<Node> Children => children;

        /// <summary>
        /// Whether this node is active. Inactive nodes will not be updated or drawn.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Whether this node is a root node (i.e., has no parent).
        /// </summary>
        public bool IsRoot => Parent == null;

        /// <summary>
        /// Called when the node should draw itself.
        /// </summary>
        /// <param name="time">Current game time.</param>
        public virtual void Draw(State state, StateMachine machine, Game game, GameTime time) { }

        /// <summary>
        /// Called when the node should update itself.
        /// </summary>
        /// <param name="time">Current game time.</param>
        public virtual void Update(State state, StateMachine machine, Game game, GameTime time) { }

        /// <summary>
        /// Sets the parent of this node. Throws an exception if the parent is this node or if it would create a cyclic relationship.
        /// </summary>
        /// <param name="parent">The target parent.</param>
        /// <exception cref="InvalidOperationException"/>
        public virtual void SetParent(Node? parent)
        {
            if (parent == this)
                throw new InvalidOperationException("Node cannot be its own parent.");

            var current = parent;
            while (current != null)
            {
                if (current == this)
                    throw new InvalidOperationException("Cyclic parent-child relationship detected.");
                current = current.Parent;
            }
            Parent?.children.Remove(this);
            Parent = parent;
            Parent?.children.Add(this);
        }
    }
}
