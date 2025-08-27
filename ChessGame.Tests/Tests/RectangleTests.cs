using Core.Utils;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class RectangleTests
    {
        [Test]
        public void TestRectangleRotateAndScale()
        {
            const float scale = 1f;
            var rect = new Rectangle(0, 0, 100, 100);
            var scaledAndRotated = rect.ScaleAndRotateRect(Vector2.One * scale, MathHelper.ToRadians(45));
            Assert.That(new Rectangle(-21, -21, 141, 141) == scaledAndRotated, $"Rect was {scaledAndRotated}");
        }
    }
}
