using Core.Nodes;
using Core.Utils;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace ChessGame.Tests.Tests
{
    [TestFixture]
    public class Test_Transform2D
    {
        [Test]
        public void Test_Position_Rotation_Scale_Recalculate_Globals()
        {
            var position = new Vector2(10, 20);
            var rotation = MathHelper.ToRadians(45);
            var scale = new Vector2(2, 2);

            var node = new Node2D();
            node.Transform.Position = position;
            node.Transform.Rotation = rotation;
            node.Transform.Scale = scale;

            Assert.That(node.Transform.GlobalPosition.Approximately(position), "GlobalPosition should match local position when no parent.");
            Assert.That(node.Transform.GlobalRotation.Approximately(rotation), "GlobalRotation should match local rotation when no parent.");
            Assert.That(node.Transform.GlobalScale.Approximately(scale), "GlobalScale should match local scale when no parent.");
        }

        [Test]
        public void Test_Hierarchy_Recalculate_Globals()
        {
            var childPosition = new Vector2(10, 0);
            var childRotation = MathHelper.ToRadians(90);
            var childScale = new Vector2(2, 2);

            var parentPosition = new Vector2(10, 0);
            var parentRotation = MathHelper.ToRadians(45);
            var parentScale = new Vector2(0.5f, 0.5f);

            var expectedPosition = new Vector2(13.535534f, 3.535534f);
            var expectedRotation = MathHelper.ToRadians(135);
            var expectedScale = new Vector2(1, 1);

            var parent = new Node2D
            {
                Transform =
                {
                    Position = parentPosition,
                    Rotation = parentRotation,
                    Scale = parentScale
                }
            };

            var child = new Node2D
            {
                Transform =
                {
                    Parent = parent.Transform,
                    Position = childPosition,
                    Rotation = childRotation,
                    Scale = childScale
                }
            };

            Assert.That(child.Transform.GlobalPosition.Approximately(expectedPosition), "Child global position should be correct in hierarchy.");
            Assert.That(child.Transform.GlobalRotation.Approximately(expectedRotation), "Child global rotation should be correct in hierarchy.");
            Assert.That(child.Transform.GlobalScale.Approximately(expectedScale), "Child global scale should be correct in hierarchy.");

            // Remove parent and check if globals revert to local
            child.Transform.Parent = null;

            Assert.That(child.Transform.GlobalPosition.Approximately(childPosition), "GlobalPosition should revert to local after removing parent.");
            Assert.That(child.Transform.GlobalRotation.Approximately(childRotation), "GlobalRotation should revert to local after removing parent.");
            Assert.That(child.Transform.GlobalScale.Approximately(childScale), "GlobalScale should revert to local after removing parent.");
        }
    }
}
