using Core.Nodes.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Components
{
    /// <summary>
    /// Represents a basic 2D transformation.
    /// </summary>
    public class Transform2D(INode node)
    {
        /// <summary>
        /// The node associated with this transform.
        /// </summary>
        public readonly INode Node = node;

        protected bool shouldRecalculate = true;
        /// <summary>
        /// Indicates whether the world transform needs to be recalculated.
        /// </summary>
        public bool ShouldRecalculate
        {
            get => shouldRecalculate;
            set => shouldRecalculate = value;
        }

        /// <summary>
        /// Indicates whether this transform is a root (has no parent).
        /// </summary>
        public bool IsRoot => Parent == null;

        private Transform2D? parent = null;
        /// <summary>
        /// The parent transform of this transform. Can be null.
        /// </summary>
        public Transform2D? Parent
        {
            get => parent;
            set
            {
                if (value == this)
                    throw new InvalidOperationException("Node cannot be its own parent.");

                var current = value;
                while (current != null)
                {
                    if (current == this)
                        throw new InvalidOperationException("Cyclic parent-child relationship detected.");
                    current = current.Parent;
                }
                parent?.children.Remove(this);
                parent = value;
                parent?.children.Add(this);
                ShouldRecalculate = true;
            }
        }

        private readonly List<Transform2D> children = [];
        /// <summary>
        /// The children of this transform.
        /// </summary>
        public IReadOnlyList<Transform2D> Children => children;

        private Vector2 localPosition = Vector2.Zero;
        /// <summary>
        /// The position of this transform in 2D space.
        /// </summary>
        public Vector2 LocalPosition
        {
            get => localPosition;
            set
            {
                localPosition = value;
                PositionChanged();
            }
        }

        private float localRotation = 0f;
        /// <summary>
        /// The rotation of this transform in radians.
        /// </summary>
        public float LocalRotation
        {
            get => localRotation;
            set
            {
                localRotation = value;
                RotationChanged();
            }
        }

        private Vector2 localScale = Vector2.One;
        /// <summary>
        /// The scale of this transform in 2D space.
        /// </summary>
        public Vector2 LocalScale
        {
            get => localScale;
            set
            {
                localScale = value;
                ScaleChanged();
            }
        }

        private Vector2 worldPosition = Vector2.Zero;
        /// <summary>
        /// The world position of this transform, taking into account the transformations of all its parents.
        /// </summary>
        public Vector2 WorldPosition
        {
            get
            {
                if (shouldRecalculate)
                    RecalculateWorldTransform();
                return worldPosition;
            }
            protected set => worldPosition = value;
        }

        private float worldRotation = 0f;
        /// <summary>
        /// The world rotation of this transform in radians, taking into account the transformations of all its parents.
        /// </summary>
        public float WorldRotation
        {
            get
            {
                if (shouldRecalculate)
                    RecalculateWorldTransform();
                return worldRotation;
            }
            protected set => worldRotation = value;
        }

        private Vector2 worldScale = Vector2.One;
        /// <summary>
        /// The world scale of this transform, taking into account the transformations of all its parents.
        /// </summary>
        public Vector2 WorldScale
        {
            get
            {
                if (shouldRecalculate)
                    RecalculateWorldTransform();
                return worldScale;
            }
            protected set => worldScale = value;
        }

        /// <summary>
        /// The origin point for position, rotation and scaling. Default is (0,0) (top-left corner).
        /// </summary>
        public Vector2 Origin { get; set; } = Vector2.Zero;

        /// <summary>
        /// Called when the position changes.
        /// </summary>
        public event Action<Transform2D>? OnPositionChanged;

        /// <summary>
        /// Called when the rotation changes.
        /// </summary>
        public event Action<Transform2D>? OnRotationChanged;

        /// <summary>
        /// Called when the scale changes.
        /// </summary>
        public event Action<Transform2D>? OnScaleChanged;

        private void PositionChanged()
        {
            ShouldRecalculate = true;
            OnPositionChanged?.Invoke(this);
        }

        private void RotationChanged()
        {
            ShouldRecalculate = true;
            OnRotationChanged?.Invoke(this);
        }

        private void ScaleChanged()
        {
            ShouldRecalculate = true;
            OnScaleChanged?.Invoke(this);
        }

        protected virtual void RecalculateWorldTransform()
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
            foreach (var child in Children)
            {
                child.ShouldRecalculate = true;
            }
        }
    }
}
