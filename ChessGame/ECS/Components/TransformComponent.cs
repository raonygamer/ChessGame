using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ECS.Components
{
    public class TransformComponent : IComponent
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Rotation { get; set; } = 0f;
        public Vector2 Scale { get; set; } = Vector2.One;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public TransformComponent? Parent { get; private set; } = null;

        public void SetParent(TransformComponent? parent)
        {
            var current = parent;
            while (current != null)
            {
                if (current == this)
                {
                    throw new InvalidOperationException("Setting this parent would create a cycle in the Transform hierarchy.");
                }
                current = current.Parent;
            }
        }

        public TransformComponent()
        {
        }

        public TransformComponent(Vector2 position, float rotation, Vector2 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public TransformComponent(TransformComponent? parent)
        {
            SetParent(parent);
        }
    }
}
