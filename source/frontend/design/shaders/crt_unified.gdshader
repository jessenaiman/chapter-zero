shader_type canvas_item;
render_mode unshaded, blend_add;

// Godot 4.6 requires explicit SCREEN_TEXTURE uniform
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

// ============================================================================
// CRT Unified Shader - All-in-One 1980s Monochrome CRT Monitor Effect
// ============================================================================
// Combines phosphor glow, scanlines, and glitch effects into a single shader.
// All effects are configurable via uniforms for maximum flexibility.
// ============================================================================

// === Screen Curvature (Phosphor Layer) ===
uniform float curvature_strength : hint_range(0.0, 1.0) = 0.15;

// === Phosphor Glow ===
uniform float phosphor_glow : hint_range(0.0, 3.0) = 1.2;
uniform vec3 phosphor_tint : source_color = vec3(0.7725, 0.6196, 0.3725);
uniform float phosphor_spread : hint_range(0.5, 2.0) = 1.0;

// === Vignette ===
uniform float vignette_strength : hint_range(0.0, 1.0) = 0.3;
uniform float vignette_softness : hint_range(0.1, 1.0) = 0.5;

// === Chromatic Aberration ===
uniform float chromatic_aberration : hint_range(0.0, 10.0) = 1.5;

// === Brightness & Contrast ===
uniform float brightness : hint_range(0.0, 2.0) = 1.0;
uniform float contrast : hint_range(0.0, 2.0) = 1.1;

// === Scanlines (Scanline Layer) ===
uniform float time;
uniform float scanline_opacity : hint_range(0.0, 0.3) = 0.08;
uniform float scanline_speed : hint_range(0.0, 20.0) = 5.0;
uniform float scanline_count : hint_range(100.0, 1000.0) = 400.0;
uniform float scanline_thickness : hint_range(0.5, 3.0) = 1.2;
uniform vec3 scanline_tint : source_color = vec3(0.6078, 0.4157, 0.2510);

// === Glitch (Glitch Layer) ===
uniform float glitch_intensity : hint_range(0.0, 1.0) = 0.0;
uniform float interference_speed : hint_range(0.0, 30.0) = 5.0;
uniform float chromatic_offset : hint_range(0.0, 20.0) = 2.0;
uniform float block_size : hint_range(4.0, 64.0) = 16.0;
uniform float block_intensity : hint_range(0.0, 1.0) = 0.5;
uniform float noise_amount : hint_range(0.0, 1.0) = 0.1;
uniform float displacement_strength : hint_range(0.0, 0.2) = 0.02;
uniform vec3 glitch_tint : source_color = vec3(0.6078, 0.4157, 0.2510);
uniform vec3 glitch_flash_tint : source_color = vec3(0.7725, 0.6196, 0.3725);

// ============================================================================
// Screen Curvature Function (Phosphor Effect)
// ============================================================================
vec2 curve_screen(vec2 uv, float strength) {
    vec2 centered_uv = uv * 2.0 - 1.0;
    float r2 = dot(centered_uv, centered_uv);
    float distortion = 1.0 + r2 * strength;
    vec2 curved = centered_uv / distortion;
    return curved * 0.5 + 0.5;
}

// ============================================================================
// Vignette Function (Phosphor Effect)
// ============================================================================
float calculate_vignette(vec2 uv, float strength, float softness) {
    vec2 centered = uv * 2.0 - 1.0;
    float dist = length(centered);
    float vignette = smoothstep(1.0 - softness, 1.0, dist);
    return 1.0 - (vignette * strength);
}

// ============================================================================
// Brightness & Contrast Adjustment
// ============================================================================
vec3 adjust_levels(vec3 color, float bright, float contra) {
    color *= bright;
    color = (color - 0.5) * contra + 0.5;
    return color;
}

// ============================================================================
// Scanline Calculation Function
// ============================================================================
float calculate_scanline_intensity(float uv_y, float time_offset, float count, float thickness) {
    float scanline_position = uv_y * count + time_offset * 10.0;
    float scanline_wave = sin(scanline_position);
    float intensity = smoothstep(1.0 - thickness * 0.1, 1.0, abs(scanline_wave));
    return intensity;
}

// ============================================================================
// Random Number Generator (Glitch Effect)
// ============================================================================
float random(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
}

// ============================================================================
// Noise Function (Glitch Effect)
// ============================================================================
float noise(vec2 uv, float time_offset) {
    vec2 noise_uv = floor(uv * 100.0 + time_offset * 10.0);
    return random(noise_uv);
}

// ============================================================================
// Horizontal Displacement for Glitch (Glitch Effect)
// ============================================================================
float calculate_horizontal_displacement(float uv_y, float time_offset, float speed, float strength) {
    float wave1 = sin(uv_y * 10.0 + time_offset * speed);
    float wave2 = sin(uv_y * 23.0 + time_offset * speed * 1.5);
    float wave3 = sin(uv_y * 47.0 + time_offset * speed * 0.7);
    float combined = (wave1 * 0.5 + wave2 * 0.3 + wave3 * 0.2);
    float random_jump = step(0.97, random(vec2(uv_y * 100.0, floor(time_offset * 10.0)))) * 2.0 - 1.0;
    return (combined + random_jump * 0.3) * strength;
}

// ============================================================================
// Block Corruption for Glitch (Glitch Effect)
// ============================================================================
float calculate_block_corruption(vec2 uv, float time_offset, float block_sz) {
    vec2 block_uv = floor(uv * vec2(1920.0 / block_sz, 1080.0 / block_sz));
    float block_random = random(block_uv + floor(time_offset * 5.0));
    float is_corrupted = step(0.85, block_random);
    float corruption_strength = random(block_uv * 1.5 + time_offset);
    return is_corrupted * corruption_strength;
}

// ============================================================================
// Main Fragment Shader - Combined All Effects
// ============================================================================
void fragment() {
    // Get curved UV coordinates (phosphor effect)
    vec2 curved_uv = curve_screen(SCREEN_UV, curvature_strength);

    // Check if we're outside the curved screen bounds
    if (curved_uv.x < 0.0 || curved_uv.x > 1.0 || curved_uv.y < 0.0 || curved_uv.y > 1.0) {
        COLOR = vec4(0.0, 0.0, 0.0, 1.0);
    } else {
        // ==================== PHOSPHOR LAYER ====================
        float phosphor_offset = 0.0005 * phosphor_spread;
        float phosphor_safe_glow = max(phosphor_glow, 0.0001);

        float phosphor_r = texture(SCREEN_TEXTURE, curved_uv + vec2(phosphor_offset, 0.0)).r;
        float phosphor_g = texture(SCREEN_TEXTURE, curved_uv).g;
        float phosphor_b = texture(SCREEN_TEXTURE, curved_uv - vec2(phosphor_offset, 0.0)).b;

        vec3 color = vec3(phosphor_r, phosphor_g, phosphor_b);
        color *= phosphor_tint;
        color = pow(color, vec3(1.0 / phosphor_safe_glow));

        // Apply chromatic aberration
        if (chromatic_aberration > 0.0) {
            vec2 aberration_centered = curved_uv - 0.5;
            float aberration_distance = length(aberration_centered);
            float aberration_amount = chromatic_aberration * 0.001 * aberration_distance;

            vec3 aberration_color = vec3(
                texture(SCREEN_TEXTURE, curved_uv + aberration_centered * aberration_amount).r,
                texture(SCREEN_TEXTURE, curved_uv).g,
                texture(SCREEN_TEXTURE, curved_uv - aberration_centered * aberration_amount).b);

            color = mix(color, aberration_color, 0.3);
        }

        // Calculate vignette
        float vignette = calculate_vignette(curved_uv, vignette_strength, vignette_softness);
        color *= vignette;

        // ==================== SCANLINE LAYER ====================
        float scanline = calculate_scanline_intensity(
            SCREEN_UV.y,
            time * scanline_speed,
            scanline_count,
            scanline_thickness
        );

        vec3 scanline_overlay = scanline_tint * scanline * scanline_opacity;
        color += scanline_overlay;

        // ==================== GLITCH LAYER ====================
        if (glitch_intensity > 0.001) {
            // Calculate horizontal displacement for this scanline
            float displacement = calculate_horizontal_displacement(
                SCREEN_UV.y,
                time,
                interference_speed,
                displacement_strength * glitch_intensity
            );

            vec2 displaced_uv = SCREEN_UV + vec2(displacement, 0.0);
            displaced_uv = clamp(displaced_uv, 0.0, 1.0);

            vec3 glitch_color;

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

                glitch_color = vec3(r, g, b);
            } else {
                glitch_color = texture(SCREEN_TEXTURE, displaced_uv).rgb;
            }

            // Add digital block corruption
            float block_corruption = calculate_block_corruption(SCREEN_UV, time, block_size);
            float block_noise = random(SCREEN_UV + time);
            vec3 tinted_block = vec3(block_noise) * glitch_tint;
            glitch_color = mix(glitch_color, tinted_block, block_corruption * block_intensity * glitch_intensity);

            // Add noise/static overlay
            float static_noise = noise(SCREEN_UV, time);
            vec3 tinted_static = (vec3(static_noise) - vec3(0.5)) * glitch_tint;
            glitch_color += tinted_static * noise_amount * glitch_intensity;

            // Random full-screen flashes
            float flash = step(0.998, random(vec2(time * 10.0, 0.0))) * glitch_intensity;
            glitch_color = mix(glitch_color, glitch_flash_tint, flash);

            glitch_color = clamp(glitch_color, 0.0, 1.0);
            color += glitch_color * glitch_intensity;
        }

        // Adjust brightness and contrast
        color = adjust_levels(color, brightness, contrast);

        // Ensure we don't exceed color bounds
        color = clamp(color, 0.0, 1.0);

        // Output final color with full opacity for additive blending
        COLOR = vec4(color, 1.0);
    }
}
