using System;
using System.Collections.Generic;
using Core.Nodes.Interfaces;
using Microsoft.Xna.Framework;

namespace Core.Components;

/// <summary>
/// Represents a basic 2D transformation.
/// </summary>
public class Transform2D
{
    /// <summary>
    /// The node associated with this transform.
    /// </summary>
    public readonly INode Node;

    #region Dirty Flags
    /// <summary>
    /// Indicates whether the global position needs to be recalculated.
    /// </summary>
    public bool DirtyGlobalPosition { get; set; } = true;

    /// <summary>
    /// Indicates whether the global rotation needs to be recalculated.
    /// </summary>
    public bool DirtyGlobalRotation { get; set; } = true;

    /// <summary>
    /// Indicates whether the global scale needs to be recalculated.
    /// </summary>
    public bool DirtyGlobalScale { get; set; } = true;
    #endregion
    #region Hierarchy
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
            EnsureNoCyclicParent(value);
            RemoveThisFromParent();
            parent = value;
            AddThisToParent();
            MarkAllDirty();
        }
    }

    private readonly List<Transform2D> children = [];
    /// <summary>
    /// The children of this transform.
    /// </summary>
    public IReadOnlyList<Transform2D> Children => children;
    #endregion
    #region Transformations
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
    #endregion
    #region Events
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
    #endregion

    public Transform2D(INode node)
    {
        Node = node;
    }

    #region Recalculation Methods
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
    #endregion
    #region Parent Change Handlers
    /// <summary>
    /// Called when the position of the parent <see cref="Transform2D"/> changes.
    /// </summary>
    /// <param name="parent">The parent <see cref="Transform2D"/> whose position has changed.</param>
    protected virtual void OnParentPositionChanged(Transform2D parent)
    {
        DirtyGlobalPosition = true;
    }

    /// <summary>
    /// Called when the rotation of the parent <see cref="Transform2D"/> changes.
    /// </summary>
    /// <param name="parent">The parent <see cref="Transform2D"/> whose rotation has changed.</param>
    protected virtual void OnParentRotationChanged(Transform2D parent)
    {
        DirtyGlobalRotation = true;
        DirtyGlobalPosition = true;
    }

    /// <summary>
    /// Called when the scale of the parent <see cref="Transform2D"/> changes.
    /// </summary>
    /// <param name="parent">The parent <see cref="Transform2D"/> whose scale has changed.</param>
    protected virtual void OnParentScaleChanged(Transform2D parent)
    {
        DirtyGlobalScale = true;
        DirtyGlobalPosition = true;
    }
    #endregion
    #region Utility Methods
    /// <summary>
    /// Ensures that setting the specified potential parent will not create a cyclic relationship.
    /// </summary>
    /// <param name="potentialParent"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void EnsureNoCyclicParent(Transform2D? potentialParent)
    {
        if (potentialParent is null)
            return;
        if (potentialParent == this)
            throw new InvalidOperationException("Node cannot be its own parent.");

        var current = potentialParent;
        while (current != null)
        {
            if (current == this)
                throw new InvalidOperationException("Cyclic parent-child relationship detected.");
            current = current.Parent;
        }
    }

    /// <summary>
    /// Removes this transform from its current parent, if any.
    /// </summary>
    private void RemoveThisFromParent()
    {
        parent?.NotifyChildRemoved(this);
        parent?.DetachChildFromEvents(this);
        parent?.children.Remove(this);
    }

    /// <summary>
    /// Adds this transform to its current parent, if any.
    /// </summary>
    private void AddThisToParent()
    {
        parent?.children.Add(this);
        parent?.AttachChildToEvents(this);
        parent?.NotifyChildAdded(this);
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
    /// Subscribes the specified child to the parent's transformation events.
    /// </summary>
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
    /// <param name="child">The child <see cref="Transform2D"/> to be unsubscribed from parent transformation events.</param>
    protected virtual void DetachChildFromEvents(Transform2D child)
    {
        OnPositionChanged -= child.OnParentPositionChanged;
        OnRotationChanged -= child.OnParentRotationChanged;
        OnScaleChanged -= child.OnParentScaleChanged;
    }

    /// <summary>
    /// Calls the OnChildAdded event.
    /// </summary>
    /// <param name="child"></param>
    protected virtual void NotifyChildAdded(Transform2D child)
    {
        OnChildAdded?.Invoke(child);
    }

    /// <summary>
    /// Calls the OnChildRemoved event.
    /// </summary>
    /// <param name="child"></param>
    protected virtual void NotifyChildRemoved(Transform2D child)
    {
        OnChildRemoved?.Invoke(child);
    }
    #endregion
}