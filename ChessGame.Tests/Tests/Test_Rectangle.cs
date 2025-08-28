using Core.Utils;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace ChessGame.Tests.Tests
{
    [TestFixture]
    public class Test_Rectangle
    {
        [Test]
        public void Test_Rectangle_Rotate_And_Scale_Correctly()
        {
            const float scale = 1f;
            var rect = new Rectangle(0, 0, 100, 100);
            var expected = new Rectangle(-21, -21, 141, 141);

            var scaledAndRotated = rect.ScaleAndRotateRect(Vector2.One * scale, MathHelper.ToRadians(45));

            Assert.That(scaledAndRotated == expected, $"Expected {expected}, but got {scaledAndRotated}");
        }
    }
}
