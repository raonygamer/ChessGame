using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ECS.Components
{
    public class Texture2DComponent : IComponent
    {
        public Texture2D? Texture { get; set; }
        public Rectangle Rectangle { get; set; } = Rectangle.Empty;
        public Color Color { get; set; } = Color.White;
        public float LayerDepth { get; set; } = 0f;
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        public Texture2DComponent()
        {
        }

        public Texture2DComponent(Texture2D? texture)
        {
            Texture = texture;
            if (texture != null)
            {
                Rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            }
        }

        public Texture2DComponent(Texture2D? texture, Rectangle rectangle)
        {
            Texture = texture;
            Rectangle = rectangle;
        }
    }
}
