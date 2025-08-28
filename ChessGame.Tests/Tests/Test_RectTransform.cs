using Core.Nodes.UI;
using Core.Utils;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Tests.Tests
{
    [TestFixture]
    public class Test_RectTransform
    {
        [Test]
        public void RecalculateLocalMin_NoParent_ReturnsPositionPlusMarginMin()
        {
            var node = new ImageNode();
            var rect = node.Transform;
            rect.Position = new Vector2(10, 20);
            rect.MarginMin = new Vector2(5, 5);

            rect.RecalculateLocalMin();

            Assert.That(rect.Min.Approximately(new Vector2(15, 25)));
        }

        [Test]
        public void RecalculateLocalMin_WithParent_ConsidersFinalSizeAnchorMinAndParentPadding()
        {
            var parentNode = new ImageNode();
            var parent = parentNode.Transform;
            parent.Size = new Vector2(100, 100);
            parent.PaddingMin = new Vector2(2, 2);

            var childNode = new ImageNode();
            var child = childNode.Transform;
            child.Position = new Vector2(10, 10);
            child.AnchorMin = new Vector2(0.5f, 0.5f);
            child.MarginMin = new Vector2(1, 1);

            child.Parent = parent;

            child.RecalculateLocalMin();

            var expected = new Vector2(10, 10) + (new Vector2(100, 100) * 0.5f + new Vector2(2, 2)) + new Vector2(1, 1);
            Assert.That(child.Min.Approximately(expected));
        }

        [Test]
        public void RecalculateGlobalMin_NoParent_EqualsMin()
        {
            var node = new ImageNode();
            var rect = node.Transform;
            rect.Position = new Vector2(7, 8);

            rect.RecalculateLocalMin();
            rect.RecalculateGlobalMin();

            Assert.That(rect.GlobalMin.Approximately(rect.Min));
        }

        [Test]
        public void RecalculateGlobalMin_WithParent_ConsidersTransformations()
        {
            var parentNode = new ImageNode();
            var parent = parentNode.Transform;
            parent.Size = new Vector2(50, 50);
            parent.Pivot = new Vector2(0, 0);
            parent.Position = new Vector2(100, 100);

            var childNode = new ImageNode();
            var child = childNode.Transform;
            child.Position = new Vector2(10, 10);
            child.AnchorMin = new Vector2(0, 0);

            child.Parent = parent;

            parent.RecalculateLocalMin();
            parent.RecalculateGlobalMin();
            child.RecalculateLocalMin();
            child.RecalculateGlobalMin();

            var offset = (child.Min - parent.Pivot * parent.FinalSize) * parent.GlobalScale;
            var expected = parent.GlobalMin + Vector2.Transform(offset, Matrix.CreateRotationZ(parent.GlobalRotation));
            Assert.That(child.GlobalMin.Approximately(expected));
        }

        [Test]
        public void RecalculateStretchSize_NoParent_EqualsSize()
        {
            var node = new ImageNode();
            var rect = node.Transform;
            rect.Size = new Vector2(20, 30);

            rect.RecalculateStretchSize();

            Assert.That(rect.StretchSize.Approximately(rect.Size));
        }

        [Test]
        public void RecalculateStretchSize_WithParent_ConsidersAnchorsAndMargins()
        {
            var parentNode = new ImageNode();
            var parent = parentNode.Transform;
            parent.Size = new Vector2(200, 100);
            parent.PaddingMin = new Vector2(2, 2);
            parent.PaddingMax = new Vector2(3, 3);

            var childNode = new ImageNode();
            var child = childNode.Transform;
            child.AnchorMin = new Vector2(0.2f, 0.2f);
            child.AnchorMax = new Vector2(0.8f, 0.8f);
            child.MarginMin = new Vector2(1, 1);
            child.MarginMax = new Vector2(2, 2);

            child.Parent = parent;

            child.RecalculateStretchSize();

            var expected = parent.FinalSize * (child.AnchorMax - child.AnchorMin)
                - (parent.PaddingMin + parent.PaddingMax)
                - child.MarginMin - child.MarginMax;
            Assert.That(child.StretchSize.Approximately(expected));
        }
    }
}
