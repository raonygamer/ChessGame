using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Nodes.UI.Interfaces
{
    /// <summary>
    /// Represents a UI element that participates in layout calculations.
    /// </summary>
    public interface ILayoutNode
    {
        bool IsMeasureDirty { get; }
        bool IsArrangeDirty { get; }
        bool IsDynamicSize { get; }

        /// <summary>
        /// Gets the desired size of this node after measurement.
        /// </summary>
        Point DesiredSize { get; }

        /// <summary>
        /// Gets the final rectangle allocated to this node after arrangement.
        /// </summary>
        Rectangle FinalRect { get; }

        /// <summary>
        /// Gets the top-left world coordinates calculated by using the parent's Min and this FinalRect.
        /// </summary>
        Point TopLeft { get; }

        /// <summary>
        /// Gets or sets the offset applied to this node's position within its parent.
        /// </summary>
        Point Offset { get; set; }

        /// <summary>
        /// Gets the last available size used during the most recent measure pass.
        /// </summary>
        Point LastAvailableSize { get; }

        /// <summary>
        /// Measures the desired size of this node given the available size.
        /// </summary>
        void Measure(Point availableSize);

        /// <summary>
        /// Arranges the content within the specified rectangular area.
        /// </summary>
        void Arrange(Rectangle finalRect);

        /// <summary>
        /// Draws the UI element.
        /// </summary>
        /// <param name="canvas">The canvas that triggered the first draw.</param>
        /// <param name="parent">The parent of this object.</param>
        /// <param name="ctx">The state context.</param>
        /// <param name="time">The game time.</param>
        void DrawUI(ILayoutNode canvas, ILayoutNode? parent, StateContext ctx, GameTime time);

        void InvalidateMeasure();
        void InvalidateArrange();
    }
}
