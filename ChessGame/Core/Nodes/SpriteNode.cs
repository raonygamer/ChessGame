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
    /// Represents a basic sprite node.
    /// </summary>
    public class SpriteNode : Node2D
    {
        /// <summary>
        /// The texture of this sprite. Can be null.
        /// </summary>
        public Texture2D? Texture { get; set; }

        /// <summary>
        /// The source rectangle of the texture to draw. If null, the entire texture is drawn.
        /// </summary>
        public Rectangle? SourceRectangle { get; set; } = null;

        /// <summary>
        /// The color to tint the sprite. Default is white (no tint).
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// The sprite effects to apply when drawing. Default is none.
        /// </summary>
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        /// <summary>
        /// The depth layer for drawing order. Default is 0f (front). Range is typically 0f (front) to 1f (back).
        /// </summary>
        public float LayerDepth { get; set; } = 0f;

        public override void Draw(StateContext ctx, GameTime time)
        {
            base.Draw(ctx, time);
            if (Texture != null)
            {
                ctx.SpriteBatch.Draw(Texture, Transform.WorldPosition, SourceRectangle, Color, Transform.WorldRotation, new Vector2(Texture.Bounds.Width, Texture.Bounds.Height) * Transform.Origin, Transform.WorldScale, Effects, LayerDepth);
            }
        }
    }
}
