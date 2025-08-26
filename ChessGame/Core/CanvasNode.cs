using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Represents a canvas node for control elements.
    /// </summary>
    public class CanvasNode : Node2D, ISize
    {
        private Vector2 size = Vector2.Zero;
        /// <summary>
        /// The size of this canvas node.
        /// </summary>
        public Vector2 Size
        {
            get => size;
            set 
            {
                size = value;
                PropagateThisResize();
            }
        }

        /// <summary>
        /// The top-left position of this canvas node in world coordinates.
        /// </summary>
        public Vector2 TopLeft => WorldPosition - (Size * Transform.Origin);

        /// <summary>
        /// The bottom-right position of this canvas node in world coordinates.
        /// </summary>
        public Vector2 BottomRight => TopLeft + Size;

        /// <summary>
        /// The rectangle representing this canvas node in world coordinates.
        /// </summary>
        public Rectangle Rect => new((int)TopLeft.X, (int)TopLeft.Y, (int)Size.X, (int)Size.Y);

        /// <summary>
        /// Resizes this canvas node to the specified size.
        /// </summary>
        /// <param name="size">The new size.</param>
        public void Resize(Vector2 size)
        {
            Size = size;
            PropagateThisResize();
        }

        private void PropagateThisResize()
        {
            foreach (var child in Children.OfType<ControlNode>())
            {
                child.PropagateResize(Size);
            }
        }
    }
}
