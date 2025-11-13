shader_type canvas_item;

// Core Palette
uniform vec4 base_color : source_color = vec4(0.819, 0.0, 0.0, 0.5); // Red: #D10000, semi-transparent
uniform vec4 gradient_color : source_color = vec4(1.0, 1.0, 1.0, 0.3); // White: semi-transparent
uniform vec4 shadow_color : source_color = vec4(0.0, 0.0, 0.0, 0.3); // Black: semi-transparent

// Controls
uniform float border_width : hint_range(0.0, 0.1) = 0.03;
uniform float pulse_speed : hint_range(0.0, 1.0) = 0.1; // Slow motion
uniform float jaggedness : hint_range(0.0, 1.0) = 0.5; // Sharp distortions

// Function to calculate jagged resonance effect
float jagged_resonance(vec2 uv, float time) {
    float noise = fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
    float sine_wave = abs(sin(time * pulse_speed + noise * jaggedness));
    return sine_wave;
}

void fragment() {
    // Calculate distance from edge
    vec2 mirrored_uv = abs(UV - 0.5) * 2.0;
    float dist_from_edge = max(mirrored_uv.x, mirrored_uv.y);

    // Create border mask
    float border_mask = smoothstep(1.0 - border_width - 0.01, 1.0 - border_width, dist_from_edge);
    border_mask *= (1.0 - smoothstep(0.99, 1.0, dist_from_edge));

    if (border_mask == 0.0) {
        discard; // Don't draw outside the border
    }

    // Calculate resonance effect
    float time = TIME;
    float resonance = jagged_resonance(UV, time);

    // Apply gradients
    vec3 final_color = base_color.rgb * resonance;
    final_color += gradient_color.rgb * (1.0 - resonance);
    final_color += shadow_color.rgb * resonance * 0.5;

    // Output color with border mask
    COLOR = vec4(final_color, border_mask);
}