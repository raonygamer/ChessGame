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
    public class FixedNode : Node, ILayoutNode
    {
        private Point _size = new(100, 100);
        /// <summary>
        /// The fixed size of this node.
        /// </summary>
        public Point Size
        {
            get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    var current = Parent;
                    ILayoutNode? last = null;
                    while (current is not null)
                    {
                        if (current is CanvasNode canvas)
                        {
                            last = canvas;
                            break;
                        }
                        last = current as ILayoutNode;
                        current = current.Parent;
                    }
                }
            }
        }

        public Point DesiredSize { get; private set; } = Point.Zero;
        public Rectangle FinalRect { get; private set; } = new(Point.Zero, Point.Zero);
        public Point TopLeft { get; private set; } = Point.Zero;
        public Point Offset { get; set; } = Point.Zero;
        public bool IsMeasureDirty { get; private set; } = true;
        public bool IsArrangeDirty { get; private set; } = true;

        public Point LastAvailableSize { get; private set; } = Point.Zero;
        public bool IsDynamicSize => false;

        public void Arrange(Rectangle finalRect)
        {
            TopLeft = finalRect.Location + ((Parent as ILayoutNode)?.TopLeft ?? Point.Zero);
            FinalRect = new(TopLeft, DesiredSize);
        }

        public void Measure(Point availableSize)
        {
            if (!IsMeasureDirty && LastAvailableSize == availableSize)
                return;
            DesiredSize = Size;
            LastAvailableSize = availableSize;
            IsMeasureDirty = false;
        }

        public override void Draw(StateContext ctx, GameTime time)
        {
            
        }

        public void DrawUI(ILayoutNode canvas, ILayoutNode? parent, StateContext ctx, GameTime time)
        {
            ctx.SpriteBatch.DrawRectangle(FinalRect, Color.Red * 0.3f);
        }

        public void InvalidateMeasure()
        {
            IsMeasureDirty = true;
            if (Parent is not ILayoutNode parentLayout || Parent is FixedNode)
                return;
            parentLayout.InvalidateMeasure();
        }

        public void InvalidateArrange()
        {
            throw new NotImplementedException();
        }
    }
}
