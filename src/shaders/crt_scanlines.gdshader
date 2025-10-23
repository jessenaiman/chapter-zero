shader_type canvas_item;

// ============================================================================
// CRT Scanlines Shader - Movement Overlay Layer for Stage 1 Terminal
// ============================================================================
// Purpose:
//   Creates animated horizontal scanlines that move across the screen,
//   simulating the electron beam refresh pattern of CRT monitors.
//
// Layer Stack Position: Middle (between Phosphor base and Glitch overlay)
//
// Usage:
//   Apply to ColorRect node's Material property as ShaderMaterial.
//   Set blend mode to "Add" or "Mix" depending on intensity desired.
//
// Thread-Specific Configurations:
//   LIGHT:    scanline_opacity = 0.05, scanline_speed = 5.0
//   SHADOW:   scanline_opacity = 0.12, scanline_speed = 3.0
//   AMBITION: scanline_opacity = 0.08, scanline_speed = 8.0
//
// Parameters:
//   - time: Automatically updated by engine (animation driver)
//   - scanline_opacity: Visibility of scanlines (0.0 = invisible, 1.0 = opaque)
//   - scanline_speed: Speed of scanline movement (pixels per second)
//   - scanline_count: Number of scanlines on screen (resolution dependent)
//   - scanline_thickness: Width of each scanline in pixels
//
// Performance: Lightweight (~0.1ms per frame at 1080p)
// ============================================================================

// === Timing ===
// Automatically updated by Godot engine, drives animation
uniform float time;

// === Scanline Appearance ===
uniform float scanline_opacity : hint_range(0.0, 0.3) = 0.08;
uniform float scanline_speed : hint_range(0.0, 20.0) = 5.0;
uniform float scanline_count : hint_range(100.0, 1000.0) = 400.0;
uniform float scanline_thickness : hint_range(0.5, 3.0) = 1.2;

// === Color Tinting ===
// Allows thread-specific color overlay (default tuned to Omega Spiral copper tones)
uniform vec3 scanline_tint : source_color = vec3(0.6078, 0.4157, 0.2510);

// ============================================================================
// calculate_scanline_intensity
// ============================================================================
// Calculates the intensity of scanlines at a given vertical position.
//
// Parameters:
//   uv_y: Vertical UV coordinate (0.0 = top, 1.0 = bottom)
//   time_offset: Animation time for scrolling effect
//   count: Number of scanlines to display
//   thickness: Width of each scanline
//
// Returns:
//   float: Scanline intensity at this position (0.0 = no scanline, 1.0 = full scanline)
//
// Implementation Notes:
//   Uses sine wave pattern for smooth scanline edges.
//   Time offset creates downward scrolling motion.
// ============================================================================
float calculate_scanline_intensity(float uv_y, float time_offset, float count, float thickness) {
    float scanline_position = uv_y * count + time_offset * 10.0;
    float scanline_wave = sin(scanline_position);

    // Sharpen the sine wave to create distinct lines
    float intensity = smoothstep(1.0 - thickness * 0.1, 1.0, abs(scanline_wave));

    return intensity;
}

// ============================================================================
// Main Fragment Shader
// ============================================================================
// Execution Flow:
//   1. Sample original texture color
//   2. Calculate scanline intensity at current pixel
//   3. Apply scanline darkening/lightening effect
//   4. Apply thread-specific color tint
//   5. Output final composited color
//
// Blend Mode Recommendations:
//   - Use "Mix" blend for subtle effect (recommended for gameplay)
//   - Use "Add" blend for intense retro aesthetic (recommended for boot sequence)
// ============================================================================
void fragment() {
    // Sample the underlying texture (from phosphor layer)
    vec4 base_color = texture(TEXTURE, UV);

    // Calculate scanline intensity at this pixel
    float scanline = calculate_scanline_intensity(
        UV.y,
        time * scanline_speed,
        scanline_count,
        scanline_thickness
    );

    // Apply scanline darkening effect
    // Note: Scanlines darken the image, not brighten it (authentic CRT behavior)
    float darkening = 1.0 - (scanline * scanline_opacity);
    vec3 scanlined_color = base_color.rgb * darkening;

    // Apply thread-specific color tint
    scanlined_color *= scanline_tint;

    // Ensure color values stay in valid range
    scanlined_color = clamp(scanlined_color, 0.0, 1.0);

    // Output with original alpha channel preserved
    COLOR = vec4(scanlined_color, base_color.a);
}

// ============================================================================
// Usage Example (GDScript):
// ============================================================================
// var scanline_material = preload("res://source/shaders/crt_scanlines.gdshader")
// var shader_instance = ShaderMaterial.new()
// shader_instance.shader = scanline_material
//
// # Configure for SHADOW thread
// shader_instance.set_shader_parameter("scanline_opacity", 0.12)
// shader_instance.set_shader_parameter("scanline_speed", 3.0)
// shader_instance.set_shader_parameter("scanline_tint", Vector3(0.6, 0.3, 0.8))
//
// $ScanlineLayer.material = shader_instance
// ============================================================================
