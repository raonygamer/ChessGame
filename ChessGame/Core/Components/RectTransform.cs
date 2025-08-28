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

    private Vector2 anchorMin = new(0, 0);
    /// <summary>
    /// The anchor min for positioning the top-left relative to its parent. (0,0) is top-left, (1,1) is bottom-right.
    /// </summary>
    public Vector2 AnchorMin
    {
        get => anchorMin;
        set
        {
            anchorMin = Vector2.Clamp(value, Vector2.Zero, Vector2.One);
            OnAnchorMinChanged?.Invoke(this);
        }
    }

    private Vector2 anchorMax = new(0, 0);
    /// <summary>
    /// The anchor max for positioning the bottom-right relative to its parent. (0,0) is top-left, (1,1) is bottom-right.
    /// </summary>
    public Vector2 AnchorMax
    {
        get => anchorMax;
        set
        {
            anchorMax = Vector2.Clamp(value, Vector2.Zero, Vector2.One);
            OnAnchorMaxChanged?.Invoke(this);
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

    /// <summary>
    /// The nearest ancestor RectTransform in the hierarchy, or null if none exists.
    /// </summary>
    public RectTransform? AncestorRectTransform
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
    /// Called when the anchor min changes.
    /// Used internally for dirty flag.
    /// External systems may also subscribe.
    /// </summary>
    public event Action<RectTransform>? OnAnchorMinChanged = (t) =>
    {
        t.DirtyLocalMin = true;
        t.DirtyGlobalMin = true;
    };

    /// <summary>
    /// Called when the anchor max changes.
    /// Used internally for dirty flag.
    /// External systems may also subscribe.
    /// </summary>
    public event Action<RectTransform>? OnAnchorMaxChanged = (t) =>
    {
        if (t.StretchWithAnchors)
            t.RecalculateSizeWithAnchors();
    };

    /// <summary>
    /// Called when the pivot changes.
    /// </summary>
    public event Action<RectTransform>? OnPivotChanged;

    /// <summary>
    /// Called when the local min is recalculated.
    /// </summary>
    public event Action<RectTransform>? OnRecalculateMin;

    /// <summary>
    /// Called when the global min is recalculated.
    /// </summary>
    public event Action<RectTransform>? OnRecalculateGlobalMin;

    private bool stretchWithAnchors = false;
    /// <summary>
    /// Whether to stretch the size with anchors.
    /// </summary>
    public bool StretchWithAnchors
    {
        get => stretchWithAnchors;
        set
        {
            stretchWithAnchors = value;
            RecalculateSizeWithAnchors();
        }
    }

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
        if (AncestorRectTransform is null)
        {
            min = Position;
        }
        else
        {
            min = Position + AncestorRectTransform.Size * AnchorMin;
        }
        OnRecalculateMin?.Invoke(this);
    }

    public virtual void RecalculateGlobalMin()
    {
        if (AncestorRectTransform is null)
        {
            globalMin = Min;
        }
        else
        {
            var parentGlobalMin = AncestorRectTransform.GlobalMin;
            var rawGlobalMin = (
                Min -
                AncestorRectTransform.Pivot *
                AncestorRectTransform.Size
            ) * AncestorRectTransform.GlobalScale;
            var parentRotation = AncestorRectTransform.GlobalRotation;
            globalMin = (
                parentGlobalMin +
                Vector2.Transform(
                    rawGlobalMin,
                    Matrix.CreateRotationZ(parentRotation)
                )
            );
        }
        OnRecalculateGlobalMin?.Invoke(this);
    }

    protected virtual void RecalculateSizeWithAnchors()
    {
        if (StretchWithAnchors && AncestorRectTransform is not null)
        {
            Size = AncestorRectTransform.Size * (AnchorMax - AnchorMin);
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
        if (StretchWithAnchors)
            RecalculateSizeWithAnchors();
    }

    protected virtual void OnParentAnchorMinChanged(RectTransform parent)
    {
        DirtyLocalMin = true;
        DirtyGlobalMin = true;
    }

    protected virtual void OnParentAnchorMaxChanged(RectTransform parent)
    {
        DirtyLocalMin = true;
        DirtyGlobalMin = true;
    }

    protected virtual void OnParentPivotChanged(RectTransform parent)
    {
        DirtyGlobalMin = true;
    }

    protected virtual void OnParentRecalculateMin(RectTransform parent)
    {
        
    }

    protected virtual void OnParentRecalculateGlobalMin(RectTransform parent)
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
            OnAnchorMinChanged += rectTransform.OnParentAnchorMinChanged;
            OnAnchorMaxChanged += rectTransform.OnParentAnchorMaxChanged;
            OnPivotChanged += rectTransform.OnParentPivotChanged;
            OnRecalculateMin += rectTransform.OnParentRecalculateMin;
            OnRecalculateGlobalMin += rectTransform.OnParentRecalculateGlobalMin;
        }
    }

    protected override void DetachChildFromEvents(Transform2D child)
    {
        base.DetachChildFromEvents(child);
        if (child is RectTransform rectTransform)
        {
            OnSizeChanged -= rectTransform.OnParentSizeChanged;
            OnAnchorMinChanged -= rectTransform.OnParentAnchorMinChanged;
            OnAnchorMaxChanged -= rectTransform.OnParentAnchorMaxChanged;
            OnPivotChanged -= rectTransform.OnParentPivotChanged;
            OnRecalculateMin -= rectTransform.OnParentRecalculateMin;
            OnRecalculateGlobalMin -= rectTransform.OnParentRecalculateGlobalMin;
        }
    }
}