using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Memory-efficient starfield background using canvas drawing.
/// No textures required, generates stars procedurally.
/// </summary>
public partial class CanvasStarfield : Node2D
{
    /// <summary>
    /// Number of stars to render
    /// </summary>
    [Export] public int StarCount { get; set; } = 150;

    /// <summary>
    /// Base star color
    /// </summary>
    [Export] public Color StarColor { get; set; } = new Color(1.0f, 0.95f, 0.8f);

    /// <summary>
    /// Twinkle speed multiplier
    /// </summary>
    [Export] public float TwinkleSpeed { get; set; } = 2.0f;

    /// <summary>
    /// Minimum star size
    /// </summary>
    [Export] public float MinStarSize { get; set; } = 1.0f;

    /// <summary>
    /// Maximum star size
    /// </summary>
    [Export] public float MaxStarSize { get; set; } = 3.0f;

    private struct Star
    {
        public Vector2 Position;
        public float Size;
        public float Brightness;
        public float TwinklePhase;
        public Color Color;
    }

    private List<Star> _Stars = new();
    private Random _Random = new();
    private float _Time;

    public override void _Ready()
    {
        GenerateStars();
    }

    public override void _Process(double delta)
    {
        _Time += (float)delta;
        QueueRedraw();
    }

    public override void _Draw()
    {
        foreach (var star in _Stars)
        {
            // Calculate twinkle effect
            float twinkle = Mathf.Sin(_Time * TwinkleSpeed + star.TwinklePhase) * 0.4f + 0.6f;
            float currentBrightness = star.Brightness * twinkle;

            // Draw star as a simple point with slight glow
            Color starColor = star.Color * currentBrightness;
            DrawCircle(star.Position, star.Size, starColor);

            // Add subtle cross pattern for star shape
            if (star.Size > 1.5f)
            {
                float crossSize = star.Size * 0.3f;
                DrawLine(star.Position + Vector2.Up * crossSize, star.Position + Vector2.Down * crossSize, starColor * 0.7f, star.Size * 0.5f);
                DrawLine(star.Position + Vector2.Left * crossSize, star.Position + Vector2.Right * crossSize, starColor * 0.7f, star.Size * 0.5f);
            }
        }
    }

    private void GenerateStars()
    {
        _Stars.Clear();
        Vector2 viewportSize = GetViewportRect().Size;

        for (int i = 0; i < StarCount; i++)
        {
            var star = new Star
            {
                Position = new Vector2(
                    (float)_Random.NextDouble() * viewportSize.X,
                    (float)_Random.NextDouble() * viewportSize.Y
                ),
                Size = MinStarSize + (float)_Random.NextDouble() * (MaxStarSize - MinStarSize),
                Brightness = 0.3f + (float)_Random.NextDouble() * 0.7f,
                TwinklePhase = (float)_Random.NextDouble() * Mathf.Pi * 2.0f,
                Color = StarColor * (0.7f + (float)_Random.NextDouble() * 0.5f)
            };
            _Stars.Add(star);
        }
    }

    /// <summary>
    /// Regenerate stars with new random positions
    /// </summary>
    public void RegenerateStars()
    {
        GenerateStars();
        QueueRedraw();
    }

    /// <summary>
    /// Set viewport size and regenerate stars
    /// </summary>
    public void SetViewportSize(Vector2 size)
    {
        GenerateStars();
    }
}
