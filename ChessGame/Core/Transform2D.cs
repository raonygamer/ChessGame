using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Represents a basic 2D transformation.
    /// </summary>
    public class Transform2D
    {
        private Vector2 position = Vector2.Zero;
        /// <summary>
        /// The position of this transform in 2D space.
        /// </summary>
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                PositionChanged();
            }
        }

        private float rotation = 0f;
        /// <summary>
        /// The rotation of this transform in radians.
        /// </summary>
        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                RotationChanged();
            }
        }

        private Vector2 scale = Vector2.One;
        /// <summary>
        /// The scale of this transform in 2D space.
        /// </summary>
        public Vector2 Scale
        {
            get => scale;
            set
            {
                scale = value;
                ScaleChanged();
            }
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

        private void PositionChanged() => OnPositionChanged?.Invoke(this);
        private void RotationChanged() => OnRotationChanged?.Invoke(this);
        private void ScaleChanged() => OnScaleChanged?.Invoke(this);
    }
}
