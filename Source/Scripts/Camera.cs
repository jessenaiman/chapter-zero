using Godot;
using System;
using System.Threading.Tasks;

/// <summary>
/// Game camera system with smooth following and screen effects.
/// The Camera class provides a centralized way to manage the game camera, including
/// smooth following of game objects, screen shake effects, zoom controls, and
/// camera bounds management.
/// </summary>
public partial class Camera : Camera2D
{
    /// <summary>
    /// Singleton instance of Camera
    /// </summary>
    public static Camera Instance { get; private set; }

    /// <summary>
    /// The gamepiece that the camera should follow.
    /// </summary>
    public Gamepiece Gamepiece
    {
        get => gamepiece;
        set
        {
            if (gamepiece != value)
            {
                gamepiece = value;
                OnGamepieceChanged();
            }
        }
    }

    /// <summary>
    /// Whether the camera is currently active.
    /// </summary>
    [Export]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The smoothing speed for camera movement.
    /// Higher values make the camera follow more closely to the target.
    /// </summary>
    [Export]
    public float SmoothingSpeed { get; set; } = 5.0f;

    /// <summary>
    /// The amplitude of screen shake effects.
    /// </summary>
    [Export]
    public float ShakeAmplitude { get; set; } = 10.0f;

    /// <summary>
    /// The frequency of screen shake effects.
    /// </summary>
    [Export]
    public float ShakeFrequency { get; set; } = 15.0f;

    /// <summary>
    /// The duration of screen shake effects in seconds.
    /// </summary>
    [Export]
    public float ShakeDuration { get; set; } = 0.5f;

    /// <summary>
    /// The bounds within which the camera is constrained.
    /// </summary>
    [Export]
    public Rect2 CameraBounds { get; set; } = new Rect2();

    /// <summary>
    /// Whether to constrain the camera to the bounds.
    /// </summary>
    [Export]
    public bool ConstrainToBounds { get; set; } = false;

    /// <summary>
    /// The zoom level of the camera.
    /// </summary>
    [Export]
    public Vector2 ZoomLevel { get; set; } = Vector2.One;

    /// <summary>
    /// The target position for the camera to follow.
    /// </summary>
    private Vector2 targetPosition = Vector2.Zero;

    /// <summary>
    /// The current shake offset.
    /// </summary>
    private Vector2 shakeOffset = Vector2.Zero;

    /// <summary>
    /// The remaining shake duration.
    /// </summary>
    private float shakeRemaining = 0.0f;

    /// <summary>
    /// The gamepiece that the camera is currently following.
    /// </summary>
    private Gamepiece gamepiece;

    /// <summary>
    /// Timer for shake effects.
    /// </summary>
    private Timer shakeTimer;

    public override void _Ready()
    {
        Instance = this;

        // Set the initial zoom level
        Zoom = ZoomLevel;

        // Create shake timer
        shakeTimer = new Timer();
        shakeTimer.OneShot = true;
        shakeTimer.Timeout += OnShakeTimeout;
        AddChild(shakeTimer);
    }

    public override void _Process(double delta)
    {
        if (!IsActive)
        {
            return;
        }

        // Update the target position based on the followed gamepiece
        if (Gamepiece != null)
        {
            targetPosition = Gamepiece.Position;
        }

        // Apply smoothing to the camera movement
        Position = Position.Lerp(targetPosition, SmoothingSpeed * (float)delta);

        // Apply screen shake if active
        if (shakeRemaining > 0.0f)
        {
            shakeRemaining -= (float)delta;
            if (shakeRemaining <= 0.0f)
            {
                shakeRemaining = 0.0f;
                shakeOffset = Vector2.Zero;
            }
            else
            {
                // Calculate shake offset using Perlin noise for natural-looking shake
                var time = (float)GetTimeSinceStartup();
                shakeOffset.X = Mathf.PerlinNoise(time * ShakeFrequency, 0) * 2.0f - 1.0f;
                shakeOffset.Y = Mathf.PerlinNoise(0, time * ShakeFrequency) * 2.0f - 1.0f;
                shakeOffset *= ShakeAmplitude * (shakeRemaining / ShakeDuration);
            }
        }

        // Apply the shake offset
        Offset = shakeOffset;

        // Constrain the camera to bounds if enabled
        if (ConstrainToBounds)
        {
            ConstrainToBoundaries();
        }
    }

    /// <summary>
    /// Make this camera the current active camera.
    /// </summary>
    public void MakeCurrent()
    {
        Current = true;
    }

    /// <summary>
    /// Reset the camera position to zero.
    /// </summary>
    public void ResetPosition()
    {
        Position = Vector2.Zero;
        targetPosition = Vector2.Zero;
    }

    /// <summary>
    /// Apply a screen shake effect.
    /// </summary>
    public async void Shake(float amplitude = -1.0f, float frequency = -1.0f, float duration = -1.0f)
    {
        if (amplitude < 0)
        {
            amplitude = ShakeAmplitude;
        }

        if (frequency < 0)
        {
            frequency = ShakeFrequency;
        }

        if (duration < 0)
        {
            duration = ShakeDuration;
        }

        // Set the shake parameters
        ShakeAmplitude = amplitude;
        ShakeFrequency = frequency;
        shakeRemaining = duration;

        // Wait for the shake to finish
        await Task.Delay(TimeSpan.FromSeconds(duration));

        // Reset the shake offset
        shakeOffset = Vector2.Zero;
    }

    /// <summary>
    /// Zoom the camera to a specific level.
    /// </summary>
    public async void ZoomTo(Vector2 targetZoom, float duration = 1.0f)
    {
        var startZoom = Zoom;
        var elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += GetProcessDeltaTime();
            var t = elapsed / duration;
            Zoom = startZoom.Lerp(targetZoom, t);

            // Wait for the next frame
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        Zoom = targetZoom;
    }

    /// <summary>
    /// Move the camera to a specific position instantly.
    /// </summary>
    public void JumpTo(Vector2 position)
    {
        Position = position;
        targetPosition = position;
    }

    /// <summary>
    /// Constrain the camera position to the defined boundaries.
    /// </summary>
    private void ConstrainToBoundaries()
    {
        if (CameraBounds == Rect2.Empty)
        {
            return;
        }

        // Get the camera viewport size
        var viewportSize = GetViewportRect().Size;

        // Calculate the camera bounds considering the viewport size and zoom
        var minX = CameraBounds.Position.X + viewportSize.X * 0.5f / Zoom.X;
        var maxX = CameraBounds.End.X - viewportSize.X * 0.5f / Zoom.X;
        var minY = CameraBounds.Position.Y + viewportSize.Y * 0.5f / Zoom.Y;
        var maxY = CameraBounds.End.Y - viewportSize.Y * 0.5f / Zoom.Y;

        // Constrain the camera position
        Position = new Vector2(
            Mathf.Clamp(Position.X, minX, maxX),
            Mathf.Clamp(Position.Y, minY, maxY)
        );
    }

    /// <summary>
    /// Callback when the followed gamepiece changes.
    /// </summary>
    private void OnGamepieceChanged()
    {
        if (Gamepiece != null)
        {
            targetPosition = Gamepiece.Position;
        }
    }

    /// <summary>
    /// Callback when the shake timer times out.
    /// </summary>
    private void OnShakeTimeout()
    {
        shakeRemaining = 0.0f;
        shakeOffset = Vector2.Zero;
    }
}
