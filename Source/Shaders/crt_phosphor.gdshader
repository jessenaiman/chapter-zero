shader_type canvas_item;

// ============================================================================
// CRT Phosphor Shader - Base Visual Layer for Stage 1 Terminal
// ============================================================================
// This shader creates an authentic CRT monitor effect with:
// - Screen curvature (barrel distortion)
// - RGB phosphor glow (sub-pixel separation)
// - Vignette darkening at edges
// - Thread-specific color tinting (Light/Shadow/Ambition)
// - Chromatic aberration on edges
// ============================================================================

// === Screen Curvature ===
uniform float curvature_strength : hint_range(0.0, 1.0) = 0.15;

// === Phosphor Glow ===
uniform float phosphor_glow : hint_range(0.0, 3.0) = 1.2;
uniform vec3 phosphor_tint : source_color = vec3(0.2, 1.0, 0.4); // Default green CRT
uniform float phosphor_spread : hint_range(0.5, 2.0) = 1.0;

// === Vignette ===
uniform float vignette_strength : hint_range(0.0, 1.0) = 0.3;
uniform float vignette_softness : hint_range(0.1, 1.0) = 0.5;

// === Chromatic Aberration ===
uniform float chromatic_aberration : hint_range(0.0, 10.0) = 1.5;

// === Brightness & Contrast ===
uniform float brightness : hint_range(0.0, 2.0) = 1.0;
uniform float contrast : hint_range(0.0, 2.0) = 1.1;

// ============================================================================
// Screen Curvature Function
// Creates barrel distortion to simulate curved CRT glass
// ============================================================================
vec2 curve_screen(vec2 uv, float strength) {
    // Center the UV coordinates
    vec2 centered_uv = uv * 2.0 - 1.0;

    // Apply barrel distortion
    float r2 = dot(centered_uv, centered_uv);
    float distortion = 1.0 + r2 * strength;
    vec2 curved = centered_uv / distortion;

    // Return to 0-1 range
    return curved * 0.5 + 0.5;
}

// ============================================================================
// Vignette Function
// Darkens edges to simulate CRT screen brightness falloff
// ============================================================================
float calculate_vignette(vec2 uv, float strength, float softness) {
    vec2 centered = uv * 2.0 - 1.0;
    float dist = length(centered);
    float vignette = smoothstep(1.0 - softness, 1.0, dist);
    return 1.0 - (vignette * strength);
}

// ============================================================================
// RGB Phosphor Separation
// Simulates CRT RGB sub-pixels for authentic glow effect
// ============================================================================
vec3 sample_phosphor(sampler2D tex, vec2 uv, vec3 tint, float glow, float spread) {
    // Sample RGB channels with slight offset (sub-pixel simulation)
    float offset = 0.0005 * spread;

    float r = texture(tex, uv + vec2(offset, 0.0)).r;
    float g = texture(tex, uv).g;
    float b = texture(tex, uv - vec2(offset, 0.0)).b;

    vec3 color = vec3(r, g, b);

    // Apply phosphor tint and glow
    color *= tint;
    color = pow(color, vec3(1.0 / glow));

    return color;
}

// ============================================================================
// Chromatic Aberration
// Separates color channels at screen edges
// ============================================================================
vec3 apply_chromatic_aberration(sampler2D tex, vec2 uv, float amount) {
    vec2 centered = uv - 0.5;
    float dist = length(centered);

    // Increase aberration near edges
    float aberration = amount * 0.001 * dist;

    float r = texture(tex, uv + centered * aberration).r;
    float g = texture(tex, uv).g;
    float b = texture(tex, uv - centered * aberration).b;

    return vec3(r, g, b);
}

// ============================================================================
// Contrast & Brightness Adjustment
// ============================================================================
vec3 adjust_levels(vec3 color, float bright, float contra) {
    // Apply brightness
    color *= bright;

    // Apply contrast
    color = (color - 0.5) * contra + 0.5;

    return color;
}

// ============================================================================
// Main Fragment Shader
// ============================================================================
void fragment() {
    // Get curved UV coordinates
    vec2 curved_uv = curve_screen(UV, curvature_strength);

    // Check if we're outside the curved screen bounds
    if (curved_uv.x < 0.0 || curved_uv.x > 1.0 || curved_uv.y < 0.0 || curved_uv.y > 1.0) {
        COLOR = vec4(0.0, 0.0, 0.0, 1.0); // Black border outside screen
        return;
    }

    // Sample with phosphor effect
    vec3 color = sample_phosphor(TEXTURE, curved_uv, phosphor_tint, phosphor_glow, phosphor_spread);

    // Apply chromatic aberration
    if (chromatic_aberration > 0.0) {
        vec3 aberration_color = apply_chromatic_aberration(TEXTURE, curved_uv, chromatic_aberration);
        color = mix(color, aberration_color, 0.3);
    }

    // Calculate vignette
    float vignette = calculate_vignette(curved_uv, vignette_strength, vignette_softness);
    color *= vignette;

    // Adjust brightness and contrast
    color = adjust_levels(color, brightness, contrast);

    // Ensure we don't exceed color bounds
    color = clamp(color, 0.0, 1.0);

    // Output final color
    COLOR = vec4(color, 1.0);
}
