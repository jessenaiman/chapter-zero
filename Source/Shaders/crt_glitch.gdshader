shader_type canvas_item;

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
uniform float interference_speed : hint_range(0.0, 30.0) = 10.0;

// === Chromatic Aberration ===
uniform float chromatic_offset : hint_range(0.0, 20.0) = 3.0;

// === Block Corruption ===
uniform float block_size : hint_range(4.0, 64.0) = 16.0;
uniform float block_intensity : hint_range(0.0, 1.0) = 0.5;

// === Noise/Static ===
uniform float noise_amount : hint_range(0.0, 1.0) = 0.15;

// === Horizontal Displacement ===
uniform float displacement_strength : hint_range(0.0, 0.2) = 0.05;

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

// ============================================================================
// apply_chromatic_glitch
// ============================================================================
// Separates RGB channels with random offsets (chromatic aberration glitch).
//
// Parameters:
//   tex: Source texture
//   uv: Base texture coordinates
//   time_offset: Animation time
//   offset: Maximum channel separation (pixels)
//
// Returns:
//   vec3: Color with separated RGB channels
//
// Implementation Notes:
//   Each channel gets random directional offset.
//   Creates characteristic "analog signal bleeding" effect.
//   Offset direction changes over time for dynamic glitch.
// ============================================================================
vec3 apply_chromatic_glitch(sampler2D tex, vec2 uv, float time_offset, float offset) {
    // Random offset direction changes over time
    float angle_r = random(vec2(time_offset, 0.0)) * 6.28318; // 2*PI
    float angle_g = random(vec2(time_offset, 1.0)) * 6.28318;
    float angle_b = random(vec2(time_offset, 2.0)) * 6.28318;
    
    // Convert pixel offset to UV offset (assuming 1920x1080)
    vec2 pixel_size = vec2(1.0 / 1920.0, 1.0 / 1080.0);
    vec2 offset_uv = pixel_size * offset;
    
    // Sample each channel with directional offset
    float r = texture(tex, uv + vec2(cos(angle_r), sin(angle_r)) * offset_uv).r;
    float g = texture(tex, uv + vec2(cos(angle_g), sin(angle_g)) * offset_uv).g;
    float b = texture(tex, uv + vec2(cos(angle_b), sin(angle_b)) * offset_uv).b;
    
    return vec3(r, g, b);
}

// ============================================================================
// Main Fragment Shader
// ============================================================================
// Execution Flow:
//   1. Early exit if glitch_intensity is zero (performance optimization)
//   2. Calculate horizontal displacement for scanline corruption
//   3. Sample texture with displaced coordinates
//   4. Apply chromatic aberration if intensity high enough
//   5. Add block corruption artifacts
//   6. Overlay noise/static
//   7. Blend with original based on glitch_intensity
//
// Blend Strategy:
//   Base texture visible when glitch_intensity = 0.0
//   Full glitch effect when glitch_intensity = 1.0
//   Smooth transition between states for dramatic reveals
// ============================================================================
void fragment() {
    // Performance optimization: skip all calculations if no glitch
    if (glitch_intensity < 0.001) {
        COLOR = texture(TEXTURE, UV);
        return;
    }
    
    // Calculate horizontal displacement for this scanline
    float displacement = calculate_horizontal_displacement(
        UV.y,
        time,
        interference_speed,
        displacement_strength * glitch_intensity
    );
    
    // Apply displacement to UV coordinates
    vec2 displaced_uv = UV + vec2(displacement, 0.0);
    
    // Clamp UV to valid range (prevent sampling outside texture)
    displaced_uv = clamp(displaced_uv, 0.0, 1.0);
    
    // Sample base color with displacement
    vec3 color;
    
    // High-intensity glitches use chromatic aberration
    if (glitch_intensity > 0.5) {
        color = apply_chromatic_glitch(
            TEXTURE,
            displaced_uv,
            time,
            chromatic_offset * glitch_intensity
        );
    } else {
        color = texture(TEXTURE, displaced_uv).rgb;
    }
    
    // Add digital block corruption
    float block_corruption = calculate_block_corruption(UV, time, block_size);
    color = mix(color, vec3(random(UV + time)), block_corruption * block_intensity * glitch_intensity);
    
    // Add noise/static overlay
    float static_noise = noise(UV, time);
    color += vec3(static_noise - 0.5) * noise_amount * glitch_intensity;
    
    // Random full-screen flashes (rare, high impact)
    float flash = step(0.998, random(vec2(time * 10.0, 0.0))) * glitch_intensity;
    color = mix(color, vec3(1.0), flash);
    
    // Ensure color stays in valid range
    color = clamp(color, 0.0, 1.0);
    
    // Output final color with original alpha
    COLOR = vec4(color, texture(TEXTURE, UV).a);
}

// ============================================================================
// Usage Example (GDScript):
// ============================================================================
// var glitch_material = preload("res://Source/Shaders/crt_glitch.gdshader")
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
