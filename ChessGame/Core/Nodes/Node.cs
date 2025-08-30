using Core.Nodes.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Nodes
{
    public abstract class Node
    {
        private readonly List<Node> _children = [];

        /// <summary>
        /// The name of this node.
        /// </summary>
        public string Name { get; set; } = "Unnamed Node";

        /// <summary>
        /// The parent of this node. Null if it has no parent.
        /// </summary>
        public Node? Parent { get; protected set; } = null;

        /// <summary>
        /// The children of this node.
        /// </summary>
        public IReadOnlyList<Node> Children => _children;

        /// <summary>
        /// The descendants of this node (all children, grandchildren, etc.).
        /// </summary>
        public IEnumerable<Node> Descendants => _children.SelectMany(c => c.Descendants).Concat(_children);

        /// <summary>
        /// Adds a child to this node.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public virtual void AddChild(Node child)
        {
            if (child is CanvasNode)
                throw new InvalidOperationException("CanvasNode cannot have a parent.");

            if (_children.Contains(child))
                return;

            child.Parent?.RemoveChild(child);
            _children.Add(child);
            child.Parent = this;
        }

        /// <summary>
        /// Removes a child from this node.
        /// </summary>
        /// <param name="child">The child to remove.</param>
        public virtual void RemoveChild(Node child)
        {
            if (child is CanvasNode)
                throw new InvalidOperationException("CanvasNode cannot have a parent.");

            if (_children.Remove(child))
            {
                child.Parent = null;
            }
        }

        /// <summary>
        /// Sets the parent of this node.
        /// </summary>
        /// <param name="parent">The parent to set.</param>
        public virtual void SetParent(Node? parent)
        {
            if (Parent == parent)
                return;
            Parent?.RemoveChild(this);
            parent?.AddChild(this);
        }

        /// <summary>
        /// Called every frame to update the node.
        /// </summary>
        public virtual void Update(StateContext ctx, GameTime time)
        {
            
        }

        /// <summary>
        /// Called every frame to draw the node.
        /// </summary>
        public virtual void Draw(StateContext ctx, GameTime time)
        {

        }
    }
}
