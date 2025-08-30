using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils
{
    public static class SpriteBatchUtils
    {
        private static Texture2D? _pixel;

        public static void DrawRectangle(
            this SpriteBatch spriteBatch,
            Rectangle srcRect, 
            Color color)
        {
            var tex = GetPixel(spriteBatch);
            spriteBatch.Draw(tex, srcRect, color);
        }

        public static Texture2D GetPixel(this SpriteBatch spriteBatch)
        {
            if (_pixel is null)
            {
                _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixel.SetData([Microsoft.Xna.Framework.Color.White]);
            }
            return _pixel;
        }
    }
}
