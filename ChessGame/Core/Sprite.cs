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
    /// Represents a basic sprite node.
    /// </summary>
    public class Sprite : Node2D
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
        /// The origin point for rotation and scaling. Default is (0,0) (top-left corner).
        /// </summary>
        public Vector2 Origin { get; set; } = Vector2.Zero;

        /// <summary>
        /// The sprite effects to apply when drawing. Default is none.
        /// </summary>
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        /// <summary>
        /// The depth layer for drawing order. Default is 0f (front). Range is typically 0f (front) to 1f (back).
        /// </summary>
        public float LayerDepth { get; set; } = 0f;

        public override void Draw(State state, StateMachine machine, Game game, GameTime time)
        {
            base.Draw(state, machine, game, time);
            if (Texture != null)
            {
                machine.SpriteBatch.Draw(Texture, WorldPosition, SourceRectangle, Color, WorldRotation, new Vector2(Texture.Bounds.Width, Texture.Bounds.Height) * Origin, WorldScale, Effects, LayerDepth);
            }
        }
    }
}
