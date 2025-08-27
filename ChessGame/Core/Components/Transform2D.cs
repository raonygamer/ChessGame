using System;
using System.Collections.Generic;
using Core.Nodes.Interfaces;
using Microsoft.Xna.Framework;

namespace Core.Components;

/// <summary>
///     Represents a basic 2D transformation.
/// </summary>
public class Transform2D(INode node)
{
    private readonly List<Transform2D> children = [];

    /// <summary>
    ///     The node associated with this transform.
    /// </summary>
    public readonly INode Node = node;

    private Vector2 localPosition = Vector2.Zero;

    private float localRotation;

    private Vector2 localScale = Vector2.One;

    private Transform2D? parent;

    protected bool shouldRecalculate = true;

    private Vector2 globalPosition = Vector2.Zero;

    private float globalRotation;

    private Vector2 globalScale = Vector2.One;

    /// <summary>
    ///     Indicates whether the world transform needs to be recalculated.
    /// </summary>
    public bool ShouldRecalculate
    {
        get => shouldRecalculate;
        set => shouldRecalculate = value;
    }

    /// <summary>
    ///     Indicates whether this transform is a root (has no parent).
    /// </summary>
    public bool IsRoot => Parent == null;

    /// <summary>
    ///     The parent transform of this transform. Can be null.
    /// </summary>
    public Transform2D? Parent
    {
        get => parent;
        set
        {
            if (value == this)
                throw new InvalidOperationException("Node cannot be its own parent.");

            var current = value;
            while (current != null)
            {
                if (current == this)
                    throw new InvalidOperationException("Cyclic parent-child relationship detected.");
                current = current.Parent;
            }

            parent?.children.Remove(this);
            parent = value;
            parent?.children.Add(this);
            ShouldRecalculate = true;
        }
    }

    /// <summary>
    ///     The children of this transform.
    /// </summary>
    public IReadOnlyList<Transform2D> Children => children;

    /// <summary>
    ///     The position of this transform in 2D space.
    /// </summary>
    public Vector2 LocalPosition
    {
        get => localPosition;
        set
        {
            localPosition = value;
            PositionChanged();
        }
    }

    /// <summary>
    ///     The rotation of this transform in radians.
    /// </summary>
    public float LocalRotation
    {
        get => localRotation;
        set
        {
            localRotation = value;
            RotationChanged();
        }
    }

    /// <summary>
    ///     The scale of this transform in 2D space.
    /// </summary>
    public Vector2 LocalScale
    {
        get => localScale;
        set
        {
            localScale = value;
            ScaleChanged();
        }
    }

    /// <summary>
    ///     The world position of this transform, taking into account the transformations of all its parents.
    /// </summary>
    public Vector2 GlobalPosition
    {
        get
        {
            if (shouldRecalculate)
                RecalculateWorldTransform();
            return globalPosition;
        }
        protected set => globalPosition = value;
    }

    /// <summary>
    ///     The world rotation of this transform in radians, taking into account the transformations of all its parents.
    /// </summary>
    public float GlobalRotation
    {
        get
        {
            if (shouldRecalculate)
                RecalculateWorldTransform();
            return globalRotation;
        }
        protected set => globalRotation = value;
    }

    /// <summary>
    ///     The world scale of this transform, taking into account the transformations of all its parents.
    /// </summary>
    public Vector2 GlobalScale
    {
        get
        {
            if (shouldRecalculate)
                RecalculateWorldTransform();
            return globalScale;
        }
        protected set => globalScale = value;
    }

    /// <summary>
    ///     The origin point for position, rotation and scaling. Default is (0,0) (top-left corner).
    /// </summary>
    public Vector2 Pivot { get; set; } = Vector2.Zero;

    /// <summary>
    ///     Called when the position changes.
    /// </summary>
    public event Action<Transform2D>? OnPositionChanged;

    /// <summary>
    ///     Called when the rotation changes.
    /// </summary>
    public event Action<Transform2D>? OnRotationChanged;

    /// <summary>
    ///     Called when the scale changes.
    /// </summary>
    public event Action<Transform2D>? OnScaleChanged;

    private void PositionChanged()
    {
        ShouldRecalculate = true;
        OnPositionChanged?.Invoke(this);
    }

    private void RotationChanged()
    {
        ShouldRecalculate = true;
        OnRotationChanged?.Invoke(this);
    }

    private void ScaleChanged()
    {
        ShouldRecalculate = true;
        OnScaleChanged?.Invoke(this);
    }

    protected virtual void RecalculateWorldTransform()
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
        foreach (var child in Children) child.ShouldRecalculate = true;
    }
}