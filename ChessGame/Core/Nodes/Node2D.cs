using Core.Components;
using Core.Nodes.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Nodes
{
    /// <summary>
    /// Represents a 2D node with position, rotation, scale and origin.
    /// </summary>
    public class Node2D : INode
    {
        protected Transform2D transform;
        /// <summary>
        /// The 2D transform of this node.
        /// </summary>
        public Transform2D Transform => transform;

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Constructs a new <see cref="Node2D"/> object with a <see cref="Transform2D"/>.
        /// </summary>
        public Node2D()
        {
            transform = new Transform2D(this);
        }

        public virtual void Draw(StateContext ctx, GameTime time)
        {
            
        }

        public virtual void Update(StateContext ctx, GameTime time)
        {
            
        }
    }
}
