using Godot;

public partial class OmegaBorderFrame : Control
{
    [Export] private string bus = "Master";
    [Export] private float audioSensitivity = 10.0f; // Multiplier for audio peak
    [Export] private float minAudioThreshold = 0.01f; // Ignore very quiet sounds
    [Export] private float reactionHoldTime = 0.1f;
    [Export] private float reactionFadeTime = 0.5f;

    private ShaderMaterial _shaderMaterial;
    private Tween _reactionTween;
    private int _busIndex;

    public override void _Ready()
    {
        // Find the Panel node that holds the shader
        var borderPanel = GetNode<Panel>("BorderPanel");
        if (borderPanel != null)
        {
            _shaderMaterial = borderPanel.Material as ShaderMaterial;
        }

        if (_shaderMaterial == null)
        {
            GD.PrintErr("OmegaBorderFrame: Could not find ShaderMaterial on 'BorderPanel' child.");
        }

        _busIndex = AudioServer.GetBusIndex(bus);
    }

    public override void _Input(InputEvent @event)
    {
        // Trigger reaction on any key press or mouse click
        if (@event.IsPressed() && !@event.IsEcho())
        {
            if (@event is InputEventKey || @event is InputEventMouseButton)
            {
                TriggerReaction();
            }
        }
    }

    public override void _Process(double delta)
    {
        if (_shaderMaterial == null) return;

        // Check if a tween is *not* active (i.e., no input reaction)
        if (_reactionTween == null || !_reactionTween.IsRunning())
        {
            // Apply audio reactivity
            float peakDb = AudioServer.GetBusPeakVolumeLeftDb(_busIndex, 0); // Get peak for left channel
            float linearPeak = Mathf.DbToLinear(peakDb) * audioSensitivity;

            float targetStrength = Mathf.Clamp(linearPeak, 0.0f, 1.0f);

            if (targetStrength < minAudioThreshold)
            {
                targetStrength = 0.0f;
            }

            // Smoothly move to the audio level
            float currentStrength = (float) _shaderMaterial.GetShaderParameter("reaction_strength");
            float smoothedStrength = Mathf.Lerp(currentStrength, targetStrength, (float) delta * 5.0f);

            _shaderMaterial.SetShaderParameter("reaction_strength", smoothedStrength);
        }
    }

    public void TriggerReaction()
    {
        if (_shaderMaterial == null) return;

        // Kill existing tween to restart the reaction
        if (_reactionTween != null && _reactionTween.IsRunning())
        {
            _reactionTween.Kill();
        }

        _reactionTween = CreateTween();
        _reactionTween.SetTrans(Tween.TransitionType.Sine);
        _reactionTween.SetEase(Tween.EaseType.Out);

        // Tween to 1.0, hold, then fade back to 0.0
        _reactionTween.TweenProperty(_shaderMaterial, "shader_parameter/reaction_strength", 1.0f, 0.05f); // Fast ramp up
        _reactionTween.TweenProperty(_shaderMaterial, "shader_parameter/reaction_strength", 0.0f, reactionFadeTime)
                      .SetDelay(reactionHoldTime); // Hold then fade
    }
}
