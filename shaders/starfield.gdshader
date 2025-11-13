shader_type canvas_item;
render_mode unshaded;

// ============================================================================
// Starfield Background Shader - Animated Space Background
// ============================================================================
// Creates a slowly animated starfield with twinkling stars.
// Perfect for Omega Spiral UI backgrounds.
// ============================================================================

uniform float time;
uniform float star_density : hint_range(0.1, 2.0) = 0.8;
uniform float twinkle_speed : hint_range(0.1, 5.0) = 1.0;
uniform float movement_speed : hint_range(0.0, 0.1) = 0.02;
uniform vec3 background_color : source_color = vec3(0.02, 0.02, 0.05); // Dark space blue-black

// ============================================================================
// Random function for star placement and twinkling
// ============================================================================
float random(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
}

// ============================================================================
// Noise for star twinkling
// ============================================================================
float noise(vec2 uv, float t) {
    vec2 i = floor(uv);
    vec2 f = fract(uv);
    float a = random(i);
    float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0));
    float d = random(i + vec2(1.0, 1.0));
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

void fragment() {
    // Base background color
    vec3 color = background_color;

    // Create star grid
    vec2 star_uv = UV * star_density * 100.0;

    // Add slow movement
    star_uv += time * movement_speed * vec2(0.1, 0.05);

    // Get star position
    vec2 star_pos = floor(star_uv);
    vec2 star_frac = fract(star_uv);

    // Random star brightness
    float star_brightness = random(star_pos);

    // Only bright stars
    if (star_brightness > 0.85) {
        // Distance from star center
        float dist = length(star_frac - vec2(0.5));

        // Star size and falloff
        float star_size = 0.02;
        float star = 1.0 - smoothstep(0.0, star_size, dist);

        // Twinkle effect
        float twinkle = noise(star_pos + time * twinkle_speed, time) * 0.5 + 0.5;
        star *= twinkle;

        // Add star color (slight blue-white tint)
        vec3 star_color = vec3(0.9, 0.95, 1.0) * star * (star_brightness - 0.85) * 5.0;

        color += star_color;
    }

    // Output final color
    COLOR = vec4(color, 1.0);
}