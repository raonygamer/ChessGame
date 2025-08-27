using Core.Nodes;
using Core.Utils;
using Microsoft.Xna.Framework;

namespace Core.Components;

/// <summary>
///     Represents a rectangle transform for UI elements.
/// </summary>
public class RectTransform(ControlNode node) : Transform2D(node)
{
    /// <summary>
    ///     The control node associated with this rect transform.
    /// </summary>
    public new readonly ControlNode Node = node;
    private Vector2 anchorPoint = new(0, 0);
    private Vector2 localMin = Vector2.Zero;
    private Vector2 size = Vector2.Zero;
    private Vector2 globalMin = Vector2.Zero;

    /// <summary>
    ///     The size of the rectangle.
    /// </summary>
    public Vector2 Size
    {
        get => size;
        set
        {
            size = value;
            ShouldRecalculate = true;
        }
    }

    /// <summary>
    ///     The anchor point for positioning the control node relative to its parent. (0,0) is top-left, (1,1) is bottom-right.
    /// </summary>
    public Vector2 AnchorPoint
    {
        get => anchorPoint;
        set
        {
            anchorPoint = Vector2.Clamp(value, Vector2.Zero, Vector2.One);
            ShouldRecalculate = true;
        }
    }

    /// <summary>
    ///     The size of the nearest parent RectTransform, or Vector2.Zero if none exists.
    /// </summary>
    public Vector2 ParentSize
    {
        get
        {
            var current = Parent;
            while (current != null)
            {
                if (current is RectTransform rectTransform)
                    return rectTransform.Size;
                current = current.Parent;
            }

            return Vector2.Zero;
        }
    }

    /// <summary>
    ///     The anchor of the nearest parent RectTransform, or Vector2.Zero if none exists.
    /// </summary>
    public Vector2 ParentAnchorPoint
    {
        get
        {
            var current = Parent;
            while (current != null)
            {
                if (current is RectTransform rectTransform)
                    return rectTransform.AnchorPoint;
                current = current.Parent;
            }

            return Vector2.Zero;
        }
    }

    /// <summary>
    ///     The top-left position of the nearest parent RectTransform in world coordinates, or Vector2.Zero if none exists.
    /// </summary>
    public Vector2 ParentGlobalMin
    {
        get
        {
            var current = Parent;
            while (current != null)
            {
                if (current is RectTransform rectTransform)
                    return rectTransform.GlobalMin;
                current = current.Parent;
            }

            return Vector2.Zero;
        }
    }

    /// <summary>
    ///     The origin of the parent transform, or Vector2.Zero if no parent exists.
    /// </summary>
    public Vector2 ParentPivot => Parent?.Pivot ?? Vector2.Zero;

    /// <summary>
    ///     The world position of the parent transform, or Vector2.Zero if no parent exists.
    /// </summary>
    public Vector2 ParentGlobalPosition => Parent?.GlobalPosition ?? Vector2.Zero;

    /// <summary>
    ///     The world rotation of the parent transform, or 0 if no parent exists.
    /// </summary>
    public float ParentGlobalRotation => Parent?.GlobalRotation ?? 0f;

    /// <summary>
    ///     The world scale of the parent transform, or Vector2.One if no parent exists.
    /// </summary>
    public Vector2 ParentGlobalScale => Parent?.GlobalScale ?? Vector2.One;

    /// <summary>
    ///     The top-left position of this RectTransform in local coordinates.
    /// </summary>
    public Vector2 LocalMin
    {
        get
        {
            if (ShouldRecalculate || Parent?.ShouldRecalculate == true)
                RecalculateWorldTransform();
            return localMin;
        }
    }

    /// <summary>
    ///     The top-left position of this RectTransform in world coordinates.
    /// </summary>
    public Vector2 GlobalMin
    {
        get
        {
            if (ShouldRecalculate || Parent?.ShouldRecalculate == true)
                RecalculateWorldTransform();
            return globalMin;
        }
    }

    /// <summary>
    ///     The bottom-right position of this RectTransform in local coordinates.
    /// </summary>
    public Vector2 LocalMax => LocalMin + Size;

    /// <summary>
    ///     The bottom-right position of this RectTransform in world coordinates.
    /// </summary>
    public Vector2 GlobalMax => GlobalMin + Size;

    /// <summary>
    ///     The rectangle representing this RectTransform in local coordinates.
    /// </summary>
    public Rectangle LocalRect => new(LocalMin.ToPoint() - (Size * Pivot).ToPoint(), Size.ToPoint());

    /// <summary>
    ///     The rectangle representing this RectTransform in world coordinates.
    /// </summary>
    public Rectangle GlobalRect => new(GlobalMin.ToPoint() - (Size * Pivot).ToPoint(), Size.ToPoint());

    /// <summary>
    ///     The rectangle representing this RectTransform in world coordinates, scaled and rotated by the world scale and
    ///     rotation.
    /// </summary>
    public Rectangle GlobalBounds => GlobalRect.ScaleAndRotateRect(GlobalScale, GlobalRotation);

    /// <summary>
    ///     The center position of this RectTransform in local coordinates.
    /// </summary>
    public Vector2 LocalCenter => LocalMin + Size * 0.5f;

    /// <summary>
    ///     The center position of this RectTransform in world coordinates.
    /// </summary>
    public Vector2 GlobalCenter => GlobalMin + Size * 0.5f;

    protected override void RecalculateWorldTransform()
    {
        if (Parent is null || IsRoot)
        {
            GlobalPosition = LocalPosition;
            GlobalRotation = LocalRotation;
            GlobalScale = LocalScale;
        }
        else
        {
            GlobalPosition = Parent.GlobalPosition + Vector2.Transform(LocalPosition * Parent.GlobalScale,
                Matrix.CreateRotationZ(Parent.GlobalRotation));
            GlobalRotation = LocalRotation + Parent.GlobalRotation;
            GlobalScale = LocalScale * Parent.GlobalScale;
        }

        ShouldRecalculate = false;
        localMin = LocalPosition + ParentSize * AnchorPoint;
        globalMin = ParentGlobalMin +
                       Vector2.Transform((LocalMin - ParentPivot * ParentSize) * ParentGlobalScale,
                           Matrix.CreateRotationZ(ParentGlobalRotation));
        foreach (var child in Children) child.ShouldRecalculate = true;
    }
}