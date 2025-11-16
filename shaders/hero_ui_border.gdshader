shader_type canvas_item;

// Core Palette
uniform vec4 base_color : source_color = vec4(0.819, 0.0, 0.0, 0.5); // Red: #D10000, semi-transparent
uniform vec4 gradient_color : source_color = vec4(1.0, 1.0, 1.0, 0.3); // White: semi-transparent
uniform vec4 shadow_color : source_color = vec4(0.0, 0.0, 0.0, 0.3); // Black: semi-transparent

// Controls
uniform float border_width : hint_range(0.0, 0.1) = 0.03;
uniform float pulse_speed : hint_range(0.0, 1.0) = 0.1; // Slow motion
uniform float jaggedness : hint_range(0.0, 1.0) = 0.5; // Sharp distortions

// Function to calculate wave pattern along the border edges
float wave_pattern(vec2 uv, float time) {
    // Determine which edge we are on (bottom, right, top, left)
    float perimeter = 0.0;
    if (uv.x < 0.5 && uv.y < 0.5) {
        // Bottom edge (0.0 - 0.5)
        perimeter = uv.x;
    } else if (uv.x >= 0.5 && uv.y < 0.5) {
        // Right edge (0.5 - 1.0)
        perimeter = 0.5 + uv.y;
    } else if (uv.x >= 0.5 && uv.y >= 0.5) {
        // Top edge (1.0 - 1.5)
        perimeter = 1.0 + (1.0 - uv.x);
    } else {
        // Left edge (1.5 - 2.0)
        perimeter = 1.5 + (1.0 - uv.y);
    }
    // Normalize to 0..1 range
    perimeter = fract(perimeter * 0.5);
    // Wave parameters
    float frequency = 8.0; // number of waves around the border
    float amplitude = 0.02; // wave height
    // Sine wave moving over time (slow animation)
    float wave = sin(perimeter * frequency * 6.28318 + time * 0.5) * amplitude;
    return wave;
}

void fragment() {
    // Calculate distance from edge
    vec2 mirrored_uv = abs(UV - 0.5) * 2.0;
    float dist_from_edge = max(mirrored_uv.x, mirrored_uv.y);

    // Create border mask - strictly within the border area
    float border_mask = smoothstep(1.0 - border_width, 1.0 - border_width + 0.005, dist_from_edge);
    border_mask *= (1.0 - smoothstep(1.0 - border_width - 0.005, 1.0 - border_width, dist_from_edge));

    if (border_mask == 0.0) {
        discard; // Don't draw outside the border
    }

    // Calculate wave pattern
    float time = TIME;
    float wave = wave_pattern(UV, time);

    // Apply red color with wave intensity (gradient will be added later)
    vec3 wave_color = mix(vec3(0.0), base_color.rgb, wave);
    // Combine with border mask for alpha
    COLOR = vec4(wave_color, border_mask);
}