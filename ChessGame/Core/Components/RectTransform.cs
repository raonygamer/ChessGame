using Core.Nodes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Components
{
    /// <summary>
    /// Represents a rectangle transform for UI elements.
    /// </summary>
    public class RectTransform(ControlNode node) : Transform2D(node)
    {
        /// <summary>
        /// The control node associated with this rect transform.
        /// </summary>
        public new readonly ControlNode Node = node;

        private Vector2 size = Vector2.Zero;
        /// <summary>
        /// The size of the rectangle.
        /// </summary>
        public Vector2 Size
        {
            get => size;
            set
            {
                size = value;
                ShouldRecalculate = true;
            }
        }

        private Vector2 anchor = new Vector2(0, 0);
        /// <summary>
        /// The anchor point for positioning the control node relative to its parent. (0,0) is top-left, (1,1) is bottom-right.
        /// </summary>
        public Vector2 Anchor
        {
            get => anchor;
            set
            {
                anchor = Vector2.Clamp(value, Vector2.Zero, Vector2.One);
                ShouldRecalculate = true;
            }
        }

        /// <summary>
        /// The size of the nearest parent RectTransform, or Vector2.Zero if none exists.
        /// </summary>
        public Vector2 ParentSize
        {
            get
            {
                var current = Parent;
                while (current != null)
                {
                    if (current is RectTransform rectTransform)
                        return rectTransform.Size;
                    current = current.Parent;
                }
                return Vector2.Zero;
            }
        }

        /// <summary>
        /// The anchor of the nearest parent RectTransform, or Vector2.Zero if none exists.
        /// </summary>
        public Vector2 ParentAnchor
        {
            get
            {
                var current = Parent;
                while (current != null)
                {
                    if (current is RectTransform rectTransform)
                        return rectTransform.Anchor;
                    current = current.Parent;
                }
                return Vector2.Zero;
            }
        }

        /// <summary>
        /// The top-left position of the nearest parent RectTransform in world coordinates, or Vector2.Zero if none exists.
        /// </summary>
        public Vector2 ParentWorldTopLeft
        {
            get
            {
                var current = Parent;
                while (current != null)
                {
                    if (current is RectTransform rectTransform)
                        return rectTransform.WorldTopLeft;
                    current = current.Parent;
                }
                return Vector2.Zero;
            }
        }

        /// <summary>
        /// The origin of the parent transform, or Vector2.Zero if no parent exists.
        /// </summary>
        public Vector2 ParentOrigin => Parent?.Origin ?? Vector2.Zero;

        /// <summary>
        /// The world position of the parent transform, or Vector2.Zero if no parent exists.
        /// </summary>
        public Vector2 ParentWorldPosition => Parent?.WorldPosition ?? Vector2.Zero;

        /// <summary>
        /// The world rotation of the parent transform, or 0 if no parent exists.
        /// </summary>
        public float ParentWorldRotation => Parent?.WorldRotation ?? 0f;

        /// <summary>
        /// The world scale of the parent transform, or Vector2.One if no parent exists.
        /// </summary>
        public Vector2 ParentWorldScale => Parent?.WorldScale ?? Vector2.One;

        private Vector2 localTopLeft = Vector2.Zero;
        /// <summary>
        /// The top-left position of this RectTransform in local coordinates.
        /// </summary>
        public Vector2 LocalTopLeft
        {
            get
            {
                if (ShouldRecalculate || Parent?.ShouldRecalculate == true)
                    RecalculateWorldTransform();
                return localTopLeft;
            }
        }

        private Vector2 worldTopLeft = Vector2.Zero;
        /// <summary>
        /// The top-left position of this RectTransform in world coordinates.
        /// </summary>
        public Vector2 WorldTopLeft
        {
            get
            {
                if (ShouldRecalculate || Parent?.ShouldRecalculate == true)
                    RecalculateWorldTransform();
                return worldTopLeft;
            }
        }

        /// <summary>
        /// The bottom-right position of this RectTransform in local coordinates.
        /// </summary>
        public Vector2 LocalBottomRight => LocalTopLeft + Size;

        /// <summary>
        /// The bottom-right position of this RectTransform in world coordinates.
        /// </summary>
        public Vector2 WorldBottomRight => WorldTopLeft + Size;

        /// <summary>
        /// The rectangle representing this RectTransform in local coordinates.
        /// </summary>
        public Rectangle LocalRect => new Rectangle(LocalTopLeft.ToPoint() - (Size * Origin).ToPoint(), Size.ToPoint());

        /// <summary>
        /// The rectangle representing this RectTransform in world coordinates.
        /// </summary>
        public Rectangle WorldRect => new Rectangle(WorldTopLeft.ToPoint() - (Size * Origin).ToPoint(), Size.ToPoint());

        /// <summary>
        /// The center position of this RectTransform in local coordinates.
        /// </summary>
        public Vector2 LocalCenter => LocalTopLeft + Size * 0.5f;

        /// <summary>
        /// The center position of this RectTransform in world coordinates.
        /// </summary>
        public Vector2 WorldCenter => WorldTopLeft + Size * 0.5f;

        protected override void RecalculateWorldTransform()
        {
            if (Parent is null || IsRoot)
            {
                WorldPosition = LocalPosition;
                WorldRotation = LocalRotation;
                WorldScale = LocalScale;
            }
            else
            {
                WorldPosition = Parent.WorldPosition + Vector2.Transform(LocalPosition * Parent.WorldScale, Matrix.CreateRotationZ(Parent.WorldRotation));
                WorldRotation = LocalRotation + Parent.WorldRotation;
                WorldScale = LocalScale * Parent.WorldScale;
            }
            ShouldRecalculate = false;
            localTopLeft = LocalPosition + ParentSize * Anchor;
            worldTopLeft = ParentWorldTopLeft + Vector2.Transform((LocalTopLeft - ParentOrigin * ParentSize) * ParentWorldScale, Matrix.CreateRotationZ(ParentWorldRotation));
            foreach (var child in Children)
            {
                child.ShouldRecalculate = true;
            }
        }
    }
}
