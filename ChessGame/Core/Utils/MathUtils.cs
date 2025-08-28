using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Core.Utils;

/// <summary>
/// Contains some useful math utility functions.
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Rescales and rotates a rectangle around its center, returning the axis-aligned bounding box that contains the transformed rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to transform.</param>
    /// <param name="scale">The new scale.</param>
    /// <param name="angle">The new rotation.</param>
    /// <returns></returns>
    public static Rectangle ScaleAndRotateRect(this Rectangle rect, Vector2 scale, float angle)
    {
        var center = rect.Center.ToVector2();

        var halfX = rect.Width * scale.X / 2f;
        var halfY = rect.Height * scale.Y / 2f;

        var corners = new Vector2[]
        {
            new(-halfX, -halfY),
            new(halfX, -halfY),
            new(halfX, halfY),
            new(-halfX, halfY)
        };

        var cos = (float)Math.Cos(angle);
        var sin = (float)Math.Sin(angle);

        var worldCorners = new Vector2[4];
        for (var i = 0; i < 4; i++)
        {
            var x = corners[i].X;
            var y = corners[i].Y;

            var rx = x * cos - y * sin;
            var ry = x * sin + y * cos;

            worldCorners[i] = center + new Vector2(rx, ry);
        }

        var minX = (int)MathF.Floor(worldCorners.Min(c => c.X));
        var maxX = (int)MathF.Floor(worldCorners.Max(c => c.X));
        var minY = (int)MathF.Floor(worldCorners.Min(c => c.Y));
        var maxY = (int)MathF.Floor(worldCorners.Max(c => c.Y));
        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }
}