shader_type canvas_item;
render_mode unshaded, blend_add;

// Godot 4.6 requires explicit SCREEN_TEXTURE uniform
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

// ============================================================================
// CRT Effects Shader - Base CRT Monitor Effects (Phosphor + Scanlines)
// ============================================================================
// Combines phosphor glow, scanlines into a single shader for UI backgrounds.
// Glitch effects removed for cleaner, more focused CRT aesthetic.
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
uniform vec3 scanline_tint : source_color = vec3(0.6078, 0.4157, 0.251);

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

        // Adjust brightness and contrast
        color = adjust_levels(color, brightness, contrast);

        // Ensure we don't exceed color bounds
        color = clamp(color, 0.0, 1.0);

        // Output final color with full opacity for additive blending
        COLOR = vec4(color, 1.0);
    }
}
