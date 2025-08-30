using Core.Nodes.UI.Interfaces;
using Core.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Nodes.UI
{
    /// <summary>
    /// Represents a UI canvas that serves as the root for all UI elements.
    /// </summary>
    public class CanvasNode : Node, ILayoutNode
    {
        private Point _size;
        /// <summary>
        /// The current size of the canvas.
        /// </summary>
        public Point Size
        {
            get => _size;
            set
            {
                _size = value;
                InvalidateMeasure();
                InvalidateArrange();
            }
        }

        public Point DesiredSize => Size;
        public Rectangle FinalRect => new(Point.Zero, Size);
        public Point TopLeft => Point.Zero;
        public Point Offset { get; set; }
        public bool IsDynamicSize => false;

        public bool IsMeasureDirty { get; private set; } = true;
        public bool IsArrangeDirty { get; private set; } = true;
        public Point LastAvailableSize { get; }

        public void Arrange(Rectangle finalRect)
        {
            throw new NotImplementedException("CanvasNode doesn't implement arrange because its a root container.");
        }

        public void Measure(Point availableSize)
        {
            throw new NotImplementedException("CanvasNode doesn't implement measure because its a root container.");
        }

        /// <summary>
        /// Resizes the canvas to the specified size.
        /// </summary>
        public void OnResize(Point size)
        {
            Size = size;
        }

        public override void SetParent(Node? parent)
        {
            throw new InvalidOperationException("CanvasNode cannot have a parent.");
        }

        public void UpdateAllLayout()
        {
            if (!IsMeasureDirty && !IsArrangeDirty)
                return;

            foreach (var child in Children.OfType<ILayoutNode>())
            {
                child.Measure(Size);
            }

            foreach (var child in Children.OfType<ILayoutNode>())
            {
                var topLeft = child.Offset;
                child.Arrange(new Rectangle(topLeft, child.DesiredSize));
            }

            IsMeasureDirty = false;
            IsArrangeDirty = false;
        }

        public override void Draw(StateContext ctx, GameTime time)
        {
            DrawUI(this, null, ctx, time);
        }

        public void DrawUI(ILayoutNode canvas, ILayoutNode? parent, StateContext ctx, GameTime time)
        {
            UpdateAllLayout();
            ctx.SpriteBatch.DrawRectangle(FinalRect, Color.Green * 0.3f);
        }

        public void InvalidateMeasure()
        {
            IsMeasureDirty = true;
        }

        public void InvalidateArrange()
        {
            IsArrangeDirty = true;
        }
    }
}
