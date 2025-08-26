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
    /// Represents a control node for UI elements.
    /// </summary>
    public class ControlNode : Node2D, ISize
    {
        private Vector2 size = Vector2.Zero;
        /// <summary>
        /// The size of this control node.
        /// </summary>
        public virtual Vector2 Size
        {
            get => size;
            set
            {
                size = value;
                PropagateThisResize();
            }
        }

        /// <summary>
        /// The top-left position of this control node in world coordinates.
        /// </summary>
        public Vector2 WorldTopLeft
        {
            get
            {
                var parentSize = GetParentSize();
                var parentTopLeft = GetParentTopLeft();
                var parentOrigin = GetParentOrigin();
                var parentRotation = GetParentRotation();
                var parentScale = GetParentScale();
                var finalPosition = parentTopLeft;
                var parentWorldOrigin = parentOrigin * parentSize;
                var offset = LocalTopLeft - parentWorldOrigin;
                var rotatedOffset = Vector2.Transform(offset * parentScale, Matrix.CreateRotationZ(parentRotation));
                return rotatedOffset + finalPosition;
            }
        }

        /// <summary>
        /// The top-left position of this control node in local coordinates.
        /// </summary>
        public Vector2 LocalTopLeft
        {
            get
            {
                var parentSize = GetParentSize();
                return Transform.Position + parentSize * Anchor;
            }
        }

        /// <summary>
        /// The bottom-right position of this control node in world coordinates.
        /// </summary>
        public Vector2 WorldBottomRight => WorldTopLeft + Size;

        /// <summary>
        /// The bottom-right position of this control node in local coordinates.
        /// </summary>
        public Vector2 LocalBottomRight => LocalTopLeft + Size;

        /// <summary>
        /// The rectangle representing this control node in world coordinates.
        /// </summary>
        public Rectangle WorldRect => new((int)WorldTopLeft.X, (int)WorldTopLeft.Y, (int)Size.X, (int)Size.Y);

        /// <summary>
        /// The rectangle representing this control node in local coordinates.
        /// </summary>
        public Rectangle LocalRect => new((int)LocalTopLeft.X, (int)LocalTopLeft.Y, (int)Size.X, (int)Size.Y);

        /// <summary>
        /// The anchor point for positioning this control node relative to its parent. (0,0) is top-left, (1,1) is bottom-right.
        /// </summary>
        public Vector2 Anchor { get; set; } = new Vector2(0, 0);

        public Texture2D? Texture;
        public Color Color = Color.White;

        public void PropagateResize(Vector2 size)
        {
            foreach (var child in Children.OfType<ControlNode>())
            {
                child.PropagateResize(size);
            }
        }

        private void PropagateThisResize()
        {
            foreach (var child in Children.OfType<ControlNode>())
            {
                child.PropagateResize(Size);
            }
        }

        public override void Draw(StateContext ctx, GameTime time)
        {
            base.Draw(ctx, time);
            if (Texture != null)
            {
                ctx.SpriteBatch.Draw(Texture, WorldTopLeft, null, Color, WorldRotation, Transform.Origin * new Vector2(Texture.Width, Texture.Height), Size * WorldScale, SpriteEffects.None, 0f);
            }
        }

        /// <summary>
        /// Gets the size of the nearest parent, or Vector2.Zero if none exists.
        /// </summary>
        /// <returns>The nearest parent with size.</returns>
        public Vector2 GetParentSize()
        {
            var current = Parent;
            while (current != null)
            {
                if (current is ISize sizeNode)
                    return sizeNode.Size;
                current = current.Parent;
            }
            return Vector2.Zero;
        }

        public Vector2 GetParentTopLeft()
        {
            var current = Parent;
            while (current != null)
            {
                if (current is CanvasNode canvas)
                    return canvas.TopLeft;
                if (current is ControlNode node)
                    return node.WorldTopLeft;
                current = current.Parent;
            }
            return Vector2.Zero;
        }

        public Vector2 GetParentOrigin()
        {
            var current = Parent;
            while (current != null)
            {
                if (current is Node2D node)
                    return node.Transform.Origin;
                current = current.Parent;
            }
            return Vector2.Zero;
        }

        public float GetParentRotation()
        {
            var current = Parent;
            while (current != null)
            {
                if (current is Node2D node)
                    return node.WorldRotation;
                current = current.Parent;
            }
            return 0f;
        }

        public Vector2 GetParentScale()
        {
            var current = Parent;
            while (current != null)
            {
                if (current is Node2D node)
                    return node.WorldScale;
                current = current.Parent;
            }
            return Vector2.One;
        }
    }
}
