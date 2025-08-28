using System.Collections.Generic;
using System.Net.Mime;
using Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Nodes.UI;

/// <summary>
/// Represents a basic text node.
/// </summary>
public class TextNode : ControlNode
{
    /// <summary>
    /// The font used to render the text. Can be null.
    /// </summary>
    public SpriteFont? Font { get; set; }
    
    /// <summary>
    /// The text content to display. Default is an empty string.
    /// </summary>
    public string Text { get; set; } = string.Empty;
    
    /// <summary>
    /// The color to tint the sprite. Default is white (no tint).
    /// </summary>
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// The sprite effects to apply when drawing. Default is none.
    /// </summary>
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;

    /// <summary>
    /// The depth layer for drawing order. Default is 0f (front). Range is typically 0f (front) to 1f (back).
    /// </summary>
    public float LayerDepth { get; set; } = 0f;
    
    /// <summary>
    /// Whether to use a right-to-left reading order. Default is false (left-to-right).
    /// </summary>
    public bool RightToLeft { get; set; } = false;
    
    /// <summary>
    /// The size of the rendered text. If Font is null, returns Vector2.Zero.
    /// </summary>
    public Vector2 TextSize => Font?.MeasureString(Text) ?? Vector2.Zero;
    
    public bool AutoBreakLines { get; set; }
    
    public override void Draw(StateContext ctx, GameTime time)
    {
        base.Draw(ctx, time);
        
        if (Font != null && !string.IsNullOrEmpty(Text))
        {
            var bounds = Transform.Parent is RectTransform t ? t.GlobalBounds : Rectangle.Empty;
            if (AutoBreakLines && bounds != Rectangle.Empty)
            {
                var words = Text.Split(' ');
                var currentLine = string.Empty;
                var lines = new List<string>();

                foreach (var word in words)
                {
                    var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    var testSize = Font.MeasureString(testLine);
                    if (testSize.X * Transform.GlobalScale.X > bounds.Width - Transform.Position.X && !string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                        currentLine = word;
                    }
                    else
                    {
                        currentLine = testLine;
                    }
                }

                if (!string.IsNullOrEmpty(currentLine))
                    lines.Add(currentLine);

                Text = string.Join("\n", lines);
            }
            
            ctx.SpriteBatch.DrawString(Font, Text, Transform.GlobalMin, Color, Transform.GlobalRotation,
                TextSize * Transform.Pivot, Transform.GlobalScale, Effects, LayerDepth, RightToLeft);
        }
    }
}