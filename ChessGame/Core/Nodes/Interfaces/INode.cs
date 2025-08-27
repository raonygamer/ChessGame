using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Nodes.Interfaces
{
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
}
