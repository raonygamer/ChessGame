using Core.Nodes;
using Core.Nodes.Interfaces;
using Core.Utils;
using Microsoft.Xna.Framework;
using System;

namespace Core.Components;

/// <summary>
/// Represents a rectangle transform for UI elements.
/// </summary>
public class RectTransform : Transform2D
{
    /// <summary>
    /// The control node associated with this rect transform.
    /// </summary>
    public new readonly IControlNode Node;

    protected bool dirtyLocalMin = true;
    /// <summary>
    /// Indicates whether the local min (top-left local position) needs to be recalculated.
    /// </summary>
    public bool DirtyLocalMin
    {
        get => dirtyLocalMin;
        set => dirtyLocalMin = value;
    }

    protected bool dirtyGlobalMin = true;
    /// <summary>
    /// Indicates whether the global min (top-left global position) needs to be recalculated.
    /// </summary>
    public bool DirtyGlobalMin
    {
        get => dirtyGlobalMin;
        set => dirtyGlobalMin = value;
    }

    private Vector2 size = Vector2.Zero;
    /// <summary>
    /// The size of the rectangle.
    /// </summary>
    public Vector2 Size
    {
        get => size;
        set
        {
            size = value;
            OnSizeChanged?.Invoke(this);
        }
    }

    private Vector2 anchorPoint = new(0, 0);
    /// <summary>
    /// The anchor point for positioning the control node relative to its parent. (0,0) is top-left, (1,1) is bottom-right.
    /// </summary>
    public Vector2 AnchorPoint
    {
        get => anchorPoint;
        set
        {
            anchorPoint = Vector2.Clamp(value, Vector2.Zero, Vector2.One);
            OnAnchorPointChanged?.Invoke(this);
        }
    }

    private Vector2 pivot = Vector2.Zero;
    /// <summary>
    /// The pivot point for position, rotation and scaling. Default is (0,0) (top-left corner).
    /// </summary>
    public Vector2 Pivot
    {
        get => pivot;
        set
        {
            pivot = Vector2.Clamp(value, Vector2.Zero, Vector2.One);
            OnPivotChanged?.Invoke(this);
        }
    }

    public RectTransform? ParentRectTransform
    {
        get
        {
            var current = Parent;
            while (current != null)
            {
                if (current is RectTransform rectTransform)
                    return rectTransform;
                current = current.Parent;
            }

            return null;
        }
    }

    private Vector2 min = Vector2.Zero;
    /// <summary>
    /// The top-left position of this RectTransform in local coordinates.
    /// </summary>
    public Vector2 Min
    {
        get
        {
            if (DirtyLocalMin)
            {
                RecalculateLocalMin();
                DirtyLocalMin = false;
            }
            return min;
        }
    }

    private Vector2 globalMin = Vector2.Zero;
    /// <summary>
    /// The top-left position of this RectTransform in global coordinates.
    /// </summary>
    public Vector2 GlobalMin
    {
        get
        {
            if (DirtyGlobalMin)
            {
                RecalculateGlobalMin();
                DirtyGlobalMin = false;
            }
            return globalMin;
        }
    }

    /// <summary>
    /// The bottom-right position of this RectTransform in local coordinates.
    /// </summary>
    public Vector2 Max => Min + Size;

    /// <summary>
    /// The bottom-right position of this RectTransform in world coordinates.
    /// </summary>
    public Vector2 GlobalMax => GlobalMin + Size;

    /// <summary>
    /// The rectangle representing this RectTransform in local coordinates.
    /// </summary>
    public Rectangle Rect => new(Min.ToPoint() - (Size * Pivot).ToPoint(), Size.ToPoint());

    /// <summary>
    /// The rectangle representing this RectTransform in world coordinates.
    /// </summary>
    public Rectangle GlobalRect => new(GlobalMin.ToPoint() - (Size * Pivot).ToPoint(), Size.ToPoint());

    /// <summary>
    /// The rectangle representing this RectTransform in world coordinates, scaled and rotated by the world scale and
    /// rotation.
    /// </summary>
    public Rectangle GlobalBounds => GlobalRect.ScaleAndRotateRect(GlobalScale, GlobalRotation);

    /// <summary>
    /// The center position of this RectTransform in local coordinates.
    /// </summary>
    public Vector2 Center => Min + Size * 0.5f;

    /// <summary>
    /// The center position of this RectTransform in world coordinates.
    /// </summary>
    public Vector2 GlobalCenter => GlobalMin + Size * 0.5f;

    /// <summary>
    /// Called when the size changes.
    /// </summary>
    public event Action<RectTransform>? OnSizeChanged;

    /// <summary>
    /// Called when the anchor point changes.
    /// Used internally for dirty flag.
    /// External systems may also subscribe.
    /// </summary>
    public event Action<RectTransform>? OnAnchorPointChanged = (t) =>
    {
        t.DirtyLocalMin = true;
        t.DirtyGlobalMin = true;
    };

    /// <summary>
    /// Called when the pivot changes.
    /// </summary>
    public event Action<RectTransform>? OnPivotChanged;

    public RectTransform(IControlNode node) : base(node)
    {
        Node = node;
        OnPositionChanged += (t) =>
        {
            DirtyLocalMin = true;
        };
    }

    public virtual void RecalculateLocalMin()
    {
        if (ParentRectTransform is null)
        {
            min = Position;
        }
        else
        {
            min = Position + ParentRectTransform.Size * AnchorPoint;
        }
    }

    public virtual void RecalculateGlobalMin()
    {
        if (ParentRectTransform is null)
        {
            globalMin = Min;
        }
        else
        {
            var parentGlobalMin = ParentRectTransform.GlobalMin;
            var rawGlobalMin = (
                Min -
                ParentRectTransform.Pivot *
                ParentRectTransform.Size
            ) * ParentRectTransform.GlobalScale;
            var parentRotation = ParentRectTransform.GlobalRotation;
            globalMin = (
                parentGlobalMin +
                Vector2.Transform(
                    rawGlobalMin,
                    Matrix.CreateRotationZ(parentRotation)
                )
            );
        }
    }

    protected override void OnParentPositionChanged(Transform2D parent)
    {
        base.OnParentPositionChanged(parent);
        DirtyGlobalMin = true;
    }

    protected override void OnParentRotationChanged(Transform2D parent)
    {
        base.OnParentRotationChanged(parent);
        DirtyGlobalMin = true;
    }

    protected override void OnParentScaleChanged(Transform2D parent)
    {
        base.OnParentScaleChanged(parent);
        DirtyGlobalMin = true;
    }

    protected virtual void OnParentSizeChanged(RectTransform parent)
    {
        DirtyLocalMin = true;
        DirtyGlobalMin = true;
    }

    protected virtual void OnParentAnchorPointChanged(RectTransform parent)
    {
        DirtyLocalMin = true;
        DirtyGlobalMin = true;
    }

    protected virtual void OnParentPivotChanged(RectTransform parent)
    {
        DirtyGlobalMin = true;
    }

    public override void MarkAllDirty()
    {
        base.MarkAllDirty();
        DirtyLocalMin = true;
        DirtyGlobalMin = true;
    }

    protected override void AttachChildToEvents(Transform2D child)
    {
        base.AttachChildToEvents(child);
        if (child is RectTransform rectTransform)
        {
            OnSizeChanged += rectTransform.OnParentSizeChanged;
            OnAnchorPointChanged += rectTransform.OnParentAnchorPointChanged;
            OnPivotChanged += rectTransform.OnParentPivotChanged;
        }
    }

    protected override void DetachChildFromEvents(Transform2D child)
    {
        base.DetachChildFromEvents(child);
        if (child is RectTransform rectTransform)
        {
            OnSizeChanged -= rectTransform.OnParentSizeChanged;
            OnAnchorPointChanged -= rectTransform.OnParentAnchorPointChanged;
            OnPivotChanged -= rectTransform.OnParentPivotChanged;
        }
    }
}