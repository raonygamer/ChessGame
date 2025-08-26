using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Represents a 2D node with position, rotation, and scale.
    /// </summary>
    public class Node2D : Node
    {
        /// <summary>
        /// The 2D transform of this node.
        /// </summary>
        public readonly Transform2D Transform = new();
        private bool worldTransformDirty = true;

        private Vector2 worldPosition = Vector2.Zero;
        /// <summary>
        /// The world position of this node, taking into account the transformations of all its parents.
        /// </summary>
        public Vector2 WorldPosition
        {
            get
            {
                if (worldTransformDirty)
                {
                    RecalculateWorldTransformations();
                    worldTransformDirty = false;
                }
                return worldPosition;
            }
        }

        private float worldRotation = 0f;
        /// <summary>
        /// The world rotation of this node in radians, taking into account the transformations of all its parents.
        /// </summary>
        public float WorldRotation
        {
            get
            {
                if (worldTransformDirty)
                {
                    RecalculateWorldTransformations();
                    worldTransformDirty = false;
                }
                return worldRotation;
            }
        }

        private Vector2 worldScale = Vector2.One;
        /// <summary>
        /// The world scale of this node, taking into account the transformations of all its parents.
        /// </summary>
        public Vector2 WorldScale
        {
            get
            {
                if (worldTransformDirty)
                {
                    RecalculateWorldTransformations();
                    worldTransformDirty = false;
                }
                return worldScale;
            }
        }

        /// <summary>
        /// Called when the transform of this node changes.
        /// </summary>
        public event Action<Node2D>? OnTransformChanged;

        /// <summary>
        /// Constructs a new <see cref="Node2D"/> object with a <see cref="Transform2D"/>.
        /// </summary>
        public Node2D()
        {
            Transform.OnPositionChanged += _ => TransformChanged();
            Transform.OnRotationChanged += _ => TransformChanged();
            Transform.OnScaleChanged += _ => TransformChanged();
        }

        /// <summary>
        /// Called when the transform changes.
        /// </summary>
        private void TransformChanged()
        {
            OnTransformChanged?.Invoke(this);
            foreach (var child in Children.OfType<Node2D>())
            {
                child.ParentTransformChanged();
            }
            worldTransformDirty = true;
        }

        /// <summary>
        /// Called when the transform of the parent changes.
        /// </summary>
        /// <param name="parent">The parent that changed the transform.</param>
        private void ParentTransformChanged()
        {
            foreach (var child in Children.OfType<Node2D>())
            {
                child.ParentTransformChanged();
            }
            worldTransformDirty = true;
        }

        /// <summary>
        /// Recalculates the world transformations of this node by traversing up the parent hierarchy.
        /// </summary>
        public void RecalculateWorldTransformations()
        {
            var current = Parent;
            Vector2 worldPos = Transform.Position;
            float worldRot = Transform.Rotation;
            Vector2 worldScale = Transform.Scale;
            while (current != null)
            {
                if (current is Node2D parent)
                {
                    worldPos = parent.WorldPosition + Vector2.Transform(worldPos * parent.WorldScale, Matrix.CreateRotationZ(parent.WorldRotation));
                    worldRot += parent.WorldRotation;
                    worldScale *= parent.WorldScale;
                    break;
                }
                current = current.Parent;
            }
            worldPosition = worldPos;
            worldRotation = worldRot;
            this.worldScale = worldScale;
        }

        /// <summary>
        /// Sets the parent of this node and marks the world transform as dirty.
        /// </summary>
        /// <param name="parent">The target parent.</param>
        public override void SetParent(Node? parent)
        {
            base.SetParent(parent);
            worldTransformDirty = true;
        }
    }
}
