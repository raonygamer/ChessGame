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

    protected bool dirtyStretchSize = true;
    /// <summary>
    /// Indicates whether the stretch size needs to be recalculated.
    /// </summary>
    public bool DirtyStretchSize
    {
        get => dirtyStretchSize;
        set => dirtyStretchSize = value;
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

    private Vector2 stretchSize = Vector2.Zero;
    /// <summary>
    /// The size of the rectangle when in stretch mode.
    /// </summary>
    public Vector2 StretchSize
    {
        get
        {
            if (DirtyStretchSize)
            {
                RecalculateStretchSize();
                DirtyStretchSize = false;
            }
            return stretchSize;
        }
        protected set => stretchSize = value;
    }

    /// <summary>
    /// The final size of the rectangle, depending on the sizing mode.
    /// </summary>
    public Vector2 FinalSize
    {
        get => SizeMode == SizeMode.Fixed ? Size : StretchSize;
    }

    /// <summary>
    /// The sizing mode of this RectTransform.
    /// </summary>
    public SizeMode SizeMode { get; set; } = SizeMode.Fixed;

    private Vector2 anchorMin = new(0, 0);
    /// <summary>
    /// The anchor min for positioning the top-left relative to its parent. (0,0) is top-left, (1,1) is bottom-right.
    /// </summary>
    public Vector2 AnchorMin
    {
        get => anchorMin;
        set
        {
            anchorMin = value;
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
            anchorMax = value;
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
    public Vector2 Max => Min + FinalSize;

    /// <summary>
    /// The bottom-right position of this RectTransform in world coordinates.
    /// </summary>
    public Vector2 GlobalMax => GlobalMin + FinalSize;

    /// <summary>
    /// The rectangle representing this RectTransform in local coordinates.
    /// </summary>
    public Rectangle Rect => new(Min.ToPoint() - (FinalSize * Pivot).ToPoint(), FinalSize.ToPoint());

    /// <summary>
    /// The rectangle representing this RectTransform in world coordinates.
    /// </summary>
    public Rectangle GlobalRect => new(GlobalMin.ToPoint() - (FinalSize * Pivot).ToPoint(), FinalSize.ToPoint());

    /// <summary>
    /// The rectangle representing this RectTransform in world coordinates, scaled and rotated by the world scale and
    /// rotation.
    /// </summary>
    public Rectangle GlobalBounds => GlobalRect.ScaleAndRotateRect(GlobalScale, GlobalRotation);

    /// <summary>
    /// The center position of this RectTransform in local coordinates.
    /// </summary>
    public Vector2 Center => Min + FinalSize * 0.5f;

    /// <summary>
    /// The center position of this RectTransform in world coordinates.
    /// </summary>
    public Vector2 GlobalCenter => GlobalMin + FinalSize * 0.5f;

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
        t.DirtyStretchSize = true;
    };

    /// <summary>
    /// Called when the anchor max changes.
    /// Used internally for dirty flag.
    /// External systems may also subscribe.
    /// </summary>
    public event Action<RectTransform>? OnAnchorMaxChanged = (t) =>
    {
        t.DirtyStretchSize = true;
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

    /// <summary>
    /// Called when the stretch size is recalculated.
    /// </summary>
    public event Action<RectTransform>? OnRecalculateStretchSize;

    public RectTransform(IControlNode node) : base(node)
    {
        Node = node;
        OnPositionChanged += (t) =>
        {
            DirtyLocalMin = true;
        };
    }

    /// <summary>
    /// Recalculates the local min based on the position and anchors.
    /// </summary>
    public virtual void RecalculateLocalMin()
    {
        if (AncestorRectTransform is null)
        {
            min = Position;
        }
        else
        {
            min = (
                Position + 
                AncestorRectTransform.FinalSize * 
                AnchorMin
            );
        }
        OnRecalculateMin?.Invoke(this);
    }

    /// <summary>
    /// Recalculates the global min based on the ancestor's global min, scale, and rotation.
    /// </summary>
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
                AncestorRectTransform.FinalSize
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

    /// <summary>
    /// Recalculates the stretch size based on the anchors and the ancestor's size.
    /// </summary>
    protected virtual void RecalculateStretchSize()
    {
        if (AncestorRectTransform is null)
        {
            StretchSize = Size;
        }
        else
        {
            StretchSize = AncestorRectTransform.FinalSize * (AnchorMax - AnchorMin);
        }
        OnRecalculateStretchSize?.Invoke(this);
    }

    /// <summary>
    /// Called when the parent position changes.
    /// </summary>
    /// <param name="parent"></param>
    protected override void OnParentPositionChanged(Transform2D parent)
    {
        base.OnParentPositionChanged(parent);
        DirtyGlobalMin = true;
    }

    /// <summary>
    /// Called when the parent rotation changes.
    /// </summary>
    /// <param name="parent"></param>
    protected override void OnParentRotationChanged(Transform2D parent)
    {
        base.OnParentRotationChanged(parent);
        DirtyGlobalMin = true;
    }

    /// <summary>
    /// Called when the parent scale changes.
    /// </summary>
    /// <param name="parent"></param>
    protected override void OnParentScaleChanged(Transform2D parent)
    {
        base.OnParentScaleChanged(parent);
        DirtyGlobalMin = true;
    }

    /// <summary>
    /// Called when the parent size changes.
    /// </summary>
    /// <param name="parent"></param>
    protected virtual void OnParentSizeChanged(RectTransform parent)
    {
        DirtyLocalMin = true;
        DirtyGlobalMin = true;
        DirtyStretchSize = true;
    }

    /// <summary>
    /// Called when the parent anchor min changes.
    /// </summary>
    /// <param name="parent"></param>
    protected virtual void OnParentAnchorMinChanged(RectTransform parent)
    {
        DirtyLocalMin = true;
        DirtyGlobalMin = true;
    }

    /// <summary>
    /// Called when the parent anchor max changes.
    /// </summary>
    /// <param name="parent"></param>
    protected virtual void OnParentAnchorMaxChanged(RectTransform parent)
    {
        DirtyLocalMin = true;
        DirtyGlobalMin = true;
    }

    /// <summary>
    /// Called when the parent pivot changes.
    /// </summary>
    /// <param name="parent"></param>
    protected virtual void OnParentPivotChanged(RectTransform parent)
    {
        DirtyGlobalMin = true;
    }

    /// <summary>
    /// Called when a parent recalculates its local min.
    /// </summary>
    /// <param name="parent"></param>
    protected virtual void OnParentRecalculateMin(RectTransform parent)
    {
        
    }

    /// <summary>
    /// Called when a parent recalculates its global min.
    /// </summary>
    /// <param name="parent"></param>
    protected virtual void OnParentRecalculateGlobalMin(RectTransform parent)
    {
        DirtyGlobalMin = true;
    }

    /// <summary>
    /// Called when a parent recalculates its stretch size.
    /// </summary>
    /// <param name="parent"></param>
    protected virtual void OnParentRecalculateStretchSize(RectTransform parent)
    {
        DirtyLocalMin = true;
        DirtyGlobalMin = true;
        DirtyStretchSize = true;
    }

    public override void MarkAllDirty()
    {
        base.MarkAllDirty();
        DirtyLocalMin = true;
        DirtyGlobalMin = true;
        DirtyStretchSize = true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="child"></param>
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
            OnRecalculateStretchSize += rectTransform.OnParentRecalculateStretchSize;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="child"></param>
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
            OnRecalculateStretchSize -= rectTransform.OnParentRecalculateStretchSize;
        }
    }
}