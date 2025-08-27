using Core.Components;
using Core.Nodes.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Nodes
{
    /// <summary>
    /// Represents a control node for UI elements.
    /// </summary>
    public class ControlNode : Node2D, IControlNode
    {
        /// <summary>
        /// The RectTransform of this control node.
        /// </summary>
        public new RectTransform Transform => (RectTransform)base.Transform;

        public bool IsMouseOver { get; set; } = false;
        public bool IsPressed { get; set; } = false;

        public ControlNode()
        {
            transform = new RectTransform(this);
        }
    }
}
