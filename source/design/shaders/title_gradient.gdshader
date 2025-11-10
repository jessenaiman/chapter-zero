shader_type canvas_item;
render_mode blend_mix;

// Two-color shading for title depth (no animation)
uniform vec4 color_primary : source_color = vec4(0.9, 0.15, 0.1, 1.0);    // Main color - Crimson/Red
uniform vec4 color_shadow : source_color = vec4(0.0, 0.0, 0.0, 0.8);      // Shadow color - Black

uniform float shadow_offset : hint_range(0.0, 0.1, 0.01) = 0.03;

void fragment() {
    // Get original texture color (text)
    vec4 text_color = texture(TEXTURE, UV);

    // If this pixel is transparent, discard it (preserve text shape)
    if (text_color.a < 0.5) {
        discard;
    }

    // Sample shadow offset (slightly down and right)
    vec2 shadow_uv = UV + vec2(shadow_offset, shadow_offset);
    vec4 shadow_sample = texture(TEXTURE, shadow_uv);

    // Blend primary color with shadow for depth
    vec3 final_color = color_primary.rgb;

    // If we're in a shadow area (where text is offset), blend in shadow color
    if (shadow_sample.a > 0.5) {
        final_color = mix(color_shadow.rgb, color_primary.rgb, 0.7);
    }

    // Apply the color to the text, preserving original alpha
    COLOR = vec4(final_color, text_color.a);
}
