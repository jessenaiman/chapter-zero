shader_type canvas_item;

// Pulsing effect parameters
uniform vec3 glow_color : source_color = vec3(0.2, 0.15, 0.3);
uniform float pulse_speed : hint_range(0.0, 2.0) = 0.35;
uniform float pulse_strength : hint_range(0.0, 1.0) = 0.6;
uniform float vignette_radius : hint_range(0.0, 1.5) = 1.1;
uniform float vignette_softness : hint_range(0.0, 1.0) = 0.45;

void fragment() {
	// Use ColorRect's color if no texture, otherwise sample texture
    vec4 base_tex = texture(TEXTURE, UV);
    vec4 color_rect = COLOR;
    
    // If texture is transparent/empty, use ColorRect color
    vec3 base_color = mix(color_rect.rgb, base_tex.rgb, base_tex.a);
    vec3 glow_tint = base_color + glow_color;

    // Create pulsing effect
    float pulse = (sin(TIME * pulse_speed) * 0.5 + 0.5) * pulse_strength;
    vec3 final_color = mix(base_color, glow_tint, pulse);

    // Apply vignette (edge darkening)
    vec2 centered = (UV - vec2(0.5));
    float vignette = smoothstep(vignette_radius, vignette_radius - vignette_softness, length(centered));
    final_color = mix(final_color, base_color, vignette);

    COLOR = vec4(final_color, color_rect.a);
}
