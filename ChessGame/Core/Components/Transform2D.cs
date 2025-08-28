using System;
using System.Collections.Generic;
using Core.Nodes.Interfaces;
using Microsoft.Xna.Framework;

namespace Core.Components;

/// <summary>
/// Represents a basic 2D transformation.
/// </summary>
public class Transform2D(INode node)
{
    /// <summary>
    /// The node associated with this transform.
    /// </summary>
    public readonly INode Node = node;

    protected bool dirtyGlobalPosition = true;
    /// <summary>
    /// Indicates whether the global position needs to be recalculated.
    /// </summary>
    public bool DirtyGlobalPosition
    {
        get => dirtyGlobalPosition;
        set => dirtyGlobalPosition = value;
    }

    protected bool dirtyGlobalRotation = true;
    /// <summary>
    /// Indicates whether the global rotation needs to be recalculated.
    /// </summary>
    public bool DirtyGlobalRotation
    {
        get => dirtyGlobalRotation;
        set => dirtyGlobalRotation = value;
    }

    protected bool dirtyGlobalScale = true;
    /// <summary>
    /// Indicates whether the global scale needs to be recalculated.
    /// </summary>
    public bool DirtyGlobalScale
    {
        get => dirtyGlobalScale;
        set => dirtyGlobalScale = value;
    }

    /// <summary>
    /// Indicates whether this transform is a root (has no parent).
    /// </summary>
    public bool IsRoot => Parent == null;

    private Transform2D? parent;
    /// <summary>
    /// The parent transform of this transform. Can be null.
    /// </summary>
    public Transform2D? Parent
    {
        get => parent;
        set
        {
            if (value == Parent)
                return;
            if (value == this)
                throw new InvalidOperationException("Node cannot be its own parent.");

            var current = value;
            while (current != null)
            {
                if (current == this)
                    throw new InvalidOperationException("Cyclic parent-child relationship detected.");
                current = current.Parent;
            }

            parent?.NotifyChildRemoved(this);
            parent?.DetachChildFromEvents(this);
            parent?.children.Remove(this);
            parent = value;
            value?.children.Add(this);
            value?.AttachChildToEvents(this);
            value?.NotifyChildAdded(this);
            MarkAllDirty();
        }
    }

    private readonly List<Transform2D> children = [];
    /// <summary>
    /// The children of this transform.
    /// </summary>
    public IReadOnlyList<Transform2D> Children => children;

    private Vector2 position = Vector2.Zero;
    /// <summary>
    /// The local position of this transform in 2D space.
    /// </summary>
    public Vector2 Position
    {
        get => position;
        set
        {
            position = value;
            OnPositionChanged?.Invoke(this);
        }
    }

    private float rotation;
    /// <summary>
    /// The local rotation of this transform in radians.
    /// </summary>
    public float Rotation
    {
        get => rotation;
        set
        {
            rotation = value;
            OnRotationChanged?.Invoke(this);
        }
    }

    private Vector2 scale = Vector2.One;
    /// <summary>
    /// The local scale of this transform in 2D space.
    /// </summary>
    public Vector2 Scale
    {
        get => scale;
        set
        {
            scale = value;
            OnScaleChanged?.Invoke(this);
        }
    }

    private Vector2 globalPosition = Vector2.Zero;
    /// <summary>
    /// The global position of this transform, taking into account the transformations of all its parents.
    /// </summary>
    public Vector2 GlobalPosition
    {
        get
        {
            if (DirtyGlobalPosition)
            {
                RecalculateGlobalPosition();
                DirtyGlobalPosition = false;
            }
            return globalPosition;
        }
        protected set => globalPosition = value;
    }

    private float globalRotation;
    /// <summary>
    /// The global rotation of this transform in radians, taking into account the transformations of all its parents.
    /// </summary>
    public float GlobalRotation
    {
        get
        {
            if (DirtyGlobalRotation)
            {
                RecalculateGlobalRotation();
                DirtyGlobalRotation = false;
            }
            return globalRotation;
        }
        protected set => globalRotation = value;
    }

    private Vector2 globalScale = Vector2.One;
    /// <summary>
    /// The global scale of this transform, taking into account the transformations of all its parents.
    /// </summary>
    public Vector2 GlobalScale
    {
        get
        {
            if (DirtyGlobalScale)
            {
                RecalculateGlobalScale();
                DirtyGlobalScale = false;
            }
            return globalScale;
        }
        protected set => globalScale = value;
    }

    /// <summary>
    /// Called when the position changes.
    /// Used internally for dirty flag.
    /// External systems may also subscribe.
    /// </summary>
    public event Action<Transform2D>? OnPositionChanged = (t) => 
    {
        t.DirtyGlobalPosition = true;
    };

    /// <summary>
    /// Called when the rotation changes.
    /// Used internally for dirty flag.
    /// External systems may also subscribe.
    /// </summary>
    public event Action<Transform2D>? OnRotationChanged = (t) => 
    {
        t.DirtyGlobalRotation = true;
        t.DirtyGlobalPosition = true;
    };

    /// <summary>
    /// Called when the scale changes.
    /// Used internally for dirty flag.
    /// External systems may also subscribe.
    /// </summary>
    public event Action<Transform2D>? OnScaleChanged = (t) => 
    {
        t.DirtyGlobalScale = true;
        t.DirtyGlobalPosition = true;
    };

    /// <summary>
    /// Called when the global position is recalculated.
    /// </summary>
    public event Action<Transform2D>? OnRecalculateGlobalPosition;

    /// <summary>
    /// Called when the global rotation is recalculated.
    /// </summary>
    public event Action<Transform2D>? OnRecalculateGlobalRotation;

    /// <summary>
    /// Called when the global scale is recalculated.
    /// </summary>
    public event Action<Transform2D>? OnRecalculateGlobalScale;

    /// <summary>
    /// Called when a child is added to this transform.
    /// </summary>
    public event Action<Transform2D>? OnChildAdded;

    /// <summary>
    /// Called when a child is removed from this transform.
    /// </summary>
    public event Action<Transform2D>? OnChildRemoved;

    /// <summary>
    /// Recalculates the global position of this transform based on the global position, rotation and scale of the parent.
    /// </summary>
    public virtual void RecalculateGlobalPosition()
    {
        if (Parent is null)
        {
            GlobalPosition = Position;
        }
        else
        {
            var scaledLocalPos = Position * Parent.GlobalScale;
            var rotationMatrix = Matrix.CreateRotationZ(Parent.GlobalRotation);
            GlobalPosition = Parent.GlobalPosition + Vector2.Transform(scaledLocalPos, rotationMatrix);
        }
        OnRecalculateGlobalPosition?.Invoke(this);
    }

    /// <summary>
    /// Recalculates the global rotation of this transform based on the global rotation of the parent.
    /// </summary>
    public virtual void RecalculateGlobalRotation()
    {
        if (Parent is null)
        {
            GlobalRotation = Rotation;
        }
        else
        {
            GlobalRotation = Rotation + Parent.GlobalRotation;
        }
        OnRecalculateGlobalRotation?.Invoke(this);
    }

    /// <summary>
    /// Recalculates the global scale of this transform based on the global scale of the parent.
    /// </summary>
    public virtual void RecalculateGlobalScale()
    {
        if (Parent is null)
        {
            GlobalScale = Scale;
        }
        else
        {
            GlobalScale = Scale * Parent.GlobalScale;
        }
        OnRecalculateGlobalScale?.Invoke(this);
    }

    /// <summary>
    /// Marks all the dirty flags true.
    /// </summary>
    public virtual void MarkAllDirty()
    {
        DirtyGlobalScale = true;
        DirtyGlobalRotation = true;
        DirtyGlobalPosition = true;
    }

    /// <summary>
    /// Called when the position of the parent <see cref="Transform2D"/> changes.
    /// </summary>
    /// <remarks>This method marks the global position as dirty, indicating that it needs to be recalculated.
    /// Subclasses can override this method to perform additional actions when the parent's position changes.</remarks>
    /// <param name="parent">The parent <see cref="Transform2D"/> whose position has changed.</param>
    protected virtual void OnParentPositionChanged(Transform2D parent)
    {
        DirtyGlobalPosition = true;
    }

    /// <summary>
    /// Called when the rotation of the parent <see cref="Transform2D"/> changes.
    /// </summary>
    /// <remarks>This method marks the global rotation and position as dirty, indicating that they need to be
    /// recalculated. Override this method in a derived class to provide additional behavior when the parent's rotation
    /// changes.</remarks>
    /// <param name="parent">The parent <see cref="Transform2D"/> whose rotation has changed.</param>
    protected virtual void OnParentRotationChanged(Transform2D parent)
    {
        DirtyGlobalRotation = true;
        DirtyGlobalPosition = true;
    }

    /// <summary>
    /// Called when the scale of the parent <see cref="Transform2D"/> changes.
    /// </summary>
    /// <remarks>This method marks the global scale and position as dirty, indicating that they need to be
    /// recalculated. Override this method in a derived class to provide additional behavior when the parent's scale
    /// changes.</remarks>
    /// <param name="parent">The parent <see cref="Transform2D"/> whose scale has changed.</param>
    protected virtual void OnParentScaleChanged(Transform2D parent)
    {
        DirtyGlobalScale = true;
        DirtyGlobalPosition = true;
    }

    /// <summary>
    /// Subscribes the specified child to the parent's transformation events.
    /// </summary>
    /// <remarks>This method ensures that the child is notified whenever the parent's position, rotation, or
    /// scale changes.  The child must implement handlers for these events to respond appropriately.</remarks>
    /// <param name="child">The child <see cref="Transform2D"/> instance to be subscribed to the parent's position, rotation, and scale
    /// change events.</param>
    protected virtual void AttachChildToEvents(Transform2D child)
    {
        OnPositionChanged += child.OnParentPositionChanged;
        OnRotationChanged += child.OnParentRotationChanged;
        OnScaleChanged += child.OnParentScaleChanged;
    }

    /// <summary>
    /// Removes the specified child from receiving notifications of parent transformation changes.
    /// </summary>
    /// <remarks>This method detaches the child from the parent's position, rotation, and scale change events.
    /// After calling this method, the child will no longer automatically update in response to changes in the
    /// parent.</remarks>
    /// <param name="child">The child <see cref="Transform2D"/> to be unsubscribed from parent transformation events.</param>
    protected virtual void DetachChildFromEvents(Transform2D child)
    {
        OnPositionChanged -= child.OnParentPositionChanged;
        OnRotationChanged -= child.OnParentRotationChanged;
        OnScaleChanged -= child.OnParentScaleChanged;
    }

    protected virtual void NotifyChildAdded(Transform2D child)
    {
        OnChildAdded?.Invoke(child);
    }

    protected virtual void NotifyChildRemoved(Transform2D child)
    {
        OnChildRemoved?.Invoke(child);
    }
}