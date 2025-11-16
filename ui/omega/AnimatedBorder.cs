using Godot;

public partial class AnimatedBorder : PanelContainer
{
    // By [Export]ing these, you can set them in the Inspector per-instance.
    // They will override the defaults set in the shader file.
    [Export] public float BorderThickness { get; set; } = 5.0f;
    [Export] public float GlowSize { get; set; } = 5.0f;
    [Export] public float Speed { get; set; } = 0.5f;
    [Export] public Color ColorA { get; set; } = new Color(0, 1, 1); // Cyan
    [Export] public Color ColorB { get; set; } = new Color(1, 0, 1); // Magenta

    private ShaderMaterial _material;

    public override void _Ready()
    {
        // Get the material we will create in the Editor setup
        // We cast it to ShaderMaterial to access its parameters.
        _material = (ShaderMaterial)Material;

        // Set initial values on load
        _material.SetShaderParameter("border_thickness", BorderThickness);
        _material.SetShaderParameter("glow_size", GlowSize);
        _material.SetShaderParameter("speed", Speed);
        _material.SetShaderParameter("color_a", ColorA);
        _material.SetShaderParameter("color_b", ColorB);
    }

    // _Process runs every frame, so it's good for animations,
    // but the shader's built-in TIME uniform is more efficient.
    // We use _Process here ONLY to update values if they change at runtime.
    public override void _Process(double delta)
    {
        // Example: How to update a value dynamically
        // If you wanted to tie speed to a game state, you could do this:
        // _material.SetShaderParameter("speed", currentGlobalSpeed);

        // You could also update the C# properties themselves,
        // but you'd need a way to tell the script to re-apply them.
        // For simplicity, we just set them once in _Ready().
    }

    // OPTIONAL: A helper function if you change the properties in code
    public void UpdateShaderParameters()
    {
        if (_material == null) return;
        _material.SetShaderParameter("border_thickness", BorderThickness);
        _material.SetShaderParameter("glow_size", GlowSize);
        _material.SetShaderParameter("speed", Speed);
        _material.SetShaderParameter("color_a", ColorA);
        _material.SetShaderParameter("color_b", ColorB);
    }
}
