shader_type canvas_item;
render_mode unshaded, blend_add;

// Godot 4.6 requires explicit SCREEN_TEXTURE uniform
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

// ============================================================================
// CRT Glitch Shader - Interference Overlay Layer for Stage 1 Terminal
// ============================================================================
// Purpose:
//   Creates digital/analog glitch effects for boot sequences and dramatic reveals.
//   Simulates CRT interference, signal corruption, and electromagnetic distortion.
//
// Layer Stack Position: Top (final overlay on Phosphor + Scanlines)
//
// Usage:
//   Apply to ColorRect node's Material property as ShaderMaterial.
//   Set blend mode to "Mix" for signal interference effects.
//   Control glitch_intensity parameter to trigger effects (0.0 = off, 1.0 = maximum)
//
// Visual States:
//   BOOT SEQUENCE:   glitch_intensity = 0.8, interference_speed = 15.0 (heavy corruption)
//   STABLE BASELINE: glitch_intensity = 0.0 (no glitches)
//   SECRET REVEAL:   glitch_intensity = 1.0, chromatic_offset = 8.0 (reality break)
//   THREAD LOCK-IN:  glitch_intensity = 0.3 for 0.5s burst (transition effect)
//
// Parameters:
//   - time: Automatically updated by engine (animation driver)
//   - glitch_intensity: Master control (0.0 = off, 1.0 = maximum chaos)
//   - interference_speed: Speed of horizontal distortion waves
//   - chromatic_offset: RGB channel separation amount (pixels)
//   - block_size: Size of digital block artifacts
//   - noise_amount: Static/snow overlay intensity
//
// Performance: Medium (~0.3ms per frame at 1080p when active)
// ============================================================================

// === Timing ===
uniform float time;

// === Glitch Control ===
uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.0;
uniform float interference_speed : hint_range(0.0, 30.0) = 5.0;

// === Chromatic Aberration ===
uniform float chromatic_offset : hint_range(0.0, 20.0) = 2.0;

// === Block Corruption ===
uniform float block_size : hint_range(4.0, 64.0) = 16.0;
uniform float block_intensity : hint_range(0.0, 1.0) = 0.5;

// === Noise/Static ===
uniform float noise_amount : hint_range(0.0, 1.0) = 0.1;

// === Horizontal Displacement ===
uniform float displacement_strength : hint_range(0.0, 0.2) = 0.02;

// === Palette Styling ===
// Default tint values match the warm copper highlights from the Ωmega Spiral logo
uniform vec3 glitch_tint : source_color = vec3(0.6078, 0.4157, 0.2510);
uniform vec3 glitch_flash_tint : source_color = vec3(0.7725, 0.6196, 0.3725);

// ============================================================================
// random
// ============================================================================
// Generates pseudo-random value from 2D coordinates.
//
// Parameters:
//   uv: Input coordinates (any 2D vector)
//
// Returns:
//   float: Pseudo-random value between 0.0 and 1.0
//
// Implementation Notes:
//   Uses classic hash function with sine/dot product.
//   Deterministic - same input always produces same output.
// ============================================================================
float random(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
}

// ============================================================================
// noise
// ============================================================================
// Generates smooth noise from coordinates and time.
//
// Parameters:
//   uv: Texture coordinates
//   time_offset: Animation time for dynamic noise
//
// Returns:
//   float: Smooth noise value between 0.0 and 1.0
//
// Implementation Notes:
//   Combines random values with time for animated static.
//   Floor() creates blocky noise pattern (CRT static aesthetic).
// ============================================================================
float noise(vec2 uv, float time_offset) {
    vec2 noise_uv = floor(uv * 100.0 + time_offset * 10.0);
    return random(noise_uv);
}

// ============================================================================
// calculate_horizontal_displacement
// ============================================================================
// Calculates horizontal offset for scanline displacement glitches.
//
// Parameters:
//   uv_y: Vertical UV coordinate (0.0 = top, 1.0 = bottom)
//   time_offset: Animation time
//   speed: Speed of displacement waves
//   strength: Maximum displacement amount
//
// Returns:
//   float: Horizontal offset to apply (-strength to +strength)
//
// Implementation Notes:
//   Uses multiple sine waves at different frequencies for chaotic effect.
//   Creates characteristic "signal interference" horizontal bands.
// ============================================================================
float calculate_horizontal_displacement(float uv_y, float time_offset, float speed, float strength) {
    // Multiple frequency bands create complex interference pattern
    float wave1 = sin(uv_y * 10.0 + time_offset * speed);
    float wave2 = sin(uv_y * 23.0 + time_offset * speed * 1.5);
    float wave3 = sin(uv_y * 47.0 + time_offset * speed * 0.7);

    // Combine waves with different amplitudes
    float combined = (wave1 * 0.5 + wave2 * 0.3 + wave3 * 0.2);

    // Apply random jumps for digital corruption
    float random_jump = step(0.97, random(vec2(uv_y * 100.0, floor(time_offset * 10.0)))) * 2.0 - 1.0;

    return (combined + random_jump * 0.3) * strength;
}

// ============================================================================
// calculate_block_corruption
// ============================================================================
// Generates digital block artifacts (rectangular corruption zones).
//
// Parameters:
//   uv: Texture coordinates
//   time_offset: Animation time
//   block_sz: Size of corruption blocks
//
// Returns:
//   float: Block corruption value (0.0 = no corruption, 1.0 = full corruption)
//
// Implementation Notes:
//   Creates rectangular regions of random intensity.
//   Mimics digital signal compression artifacts and buffer corruption.
// ============================================================================
float calculate_block_corruption(vec2 uv, float time_offset, float block_sz) {
    // Quantize UV to block grid
    vec2 block_uv = floor(uv * vec2(1920.0 / block_sz, 1080.0 / block_sz));

    // Random value per block, changes over time
    float block_random = random(block_uv + floor(time_offset * 5.0));

    // Only some blocks are corrupted (threshold creates sparse pattern)
    float is_corrupted = step(0.85, block_random);

    // Corruption intensity varies per block
    float corruption_strength = random(block_uv * 1.5 + time_offset);

    return is_corrupted * corruption_strength;
}

void fragment() {
    // Performance optimization: skip all calculations if no glitch
    if (glitch_intensity < 0.001) {
        COLOR = texture(SCREEN_TEXTURE, SCREEN_UV);
    } else {

    // Calculate horizontal displacement for this scanline
    float displacement = calculate_horizontal_displacement(
        SCREEN_UV.y,
        time,
        interference_speed,
        displacement_strength * glitch_intensity
    );

    // Apply displacement to UV coordinates
    vec2 displaced_uv = SCREEN_UV + vec2(displacement, 0.0);

    // Clamp UV to valid range (prevent sampling outside texture)
    displaced_uv = clamp(displaced_uv, 0.0, 1.0);

    // Sample base color with displacement
    vec3 color;

    // High-intensity glitches use chromatic aberration
    if (glitch_intensity > 0.5) {
        float angle_r = random(vec2(time, 0.0)) * 6.28318;
        float angle_g = random(vec2(time, 1.0)) * 6.28318;
        float angle_b = random(vec2(time, 2.0)) * 6.28318;

        vec2 pixel_size = vec2(1.0 / 1920.0, 1.0 / 1080.0);
        vec2 offset_uv = pixel_size * (chromatic_offset * glitch_intensity);

        float r = texture(SCREEN_TEXTURE, displaced_uv + vec2(cos(angle_r), sin(angle_r)) * offset_uv).r;
        float g = texture(SCREEN_TEXTURE, displaced_uv + vec2(cos(angle_g), sin(angle_g)) * offset_uv).g;
        float b = texture(SCREEN_TEXTURE, displaced_uv + vec2(cos(angle_b), sin(angle_b)) * offset_uv).b;

        color = vec3(r, g, b);
    } else {
        color = texture(SCREEN_TEXTURE, displaced_uv).rgb;
    }

    // Add digital block corruption
    float block_corruption = calculate_block_corruption(SCREEN_UV, time, block_size);
    float block_noise = random(SCREEN_UV + time);
    vec3 tinted_block = vec3(block_noise) * glitch_tint;
    color = mix(color, tinted_block, block_corruption * block_intensity * glitch_intensity);

    // Add noise/static overlay
    float static_noise = noise(SCREEN_UV, time);
    vec3 tinted_static = (vec3(static_noise) - vec3(0.5)) * glitch_tint;
    color += tinted_static * noise_amount * glitch_intensity;

    // Random full-screen flashes (rare, high impact)
    float flash = step(0.998, random(vec2(time * 10.0, 0.0))) * glitch_intensity;
    color = mix(color, glitch_flash_tint, flash);

    // Ensure color stays in valid range
    color = clamp(color, 0.0, 1.0);

    // Scale glitch effect intensity and output for additive blending
    // Only output glitch colors when active (glitch_intensity > 0)
    vec3 glitch_output = color * glitch_intensity;

    // Output final color with additive blending
    // When glitch_intensity = 0, output is black (no effect)
    // When glitch_intensity = 1, output is full glitch effect
    COLOR = vec4(glitch_output, 1.0);
    }
}

// ============================================================================
// Usage Example (GDScript):
// ============================================================================
// var glitch_material = preload("res://source/shaders/crt_glitch.gdshader")
// var shader_instance = ShaderMaterial.new()
// shader_instance.shader = glitch_material
//
// # Boot sequence - heavy glitching
// shader_instance.set_shader_parameter("glitch_intensity", 0.8)
// shader_instance.set_shader_parameter("interference_speed", 15.0)
// shader_instance.set_shader_parameter("chromatic_offset", 5.0)
//
// # Stable operation - no glitches
// shader_instance.set_shader_parameter("glitch_intensity", 0.0)
//
// # Secret reveal - reality-breaking effect
// shader_instance.set_shader_parameter("glitch_intensity", 1.0)
// shader_instance.set_shader_parameter("chromatic_offset", 8.0)
// shader_instance.set_shader_parameter("block_size", 32.0)
//
// # Brief transition burst
// shader_instance.set_shader_parameter("glitch_intensity", 0.3)
// await get_tree().create_timer(0.5).timeout
// shader_instance.set_shader_parameter("glitch_intensity", 0.0)
//
// $GlitchLayer.material = shader_instance
// ============================================================================
