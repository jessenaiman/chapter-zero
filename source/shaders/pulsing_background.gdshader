shader_type canvas_item;

// Use the ColorRect's color property as the base color
// base_color uniform is kept for backward compatibility but not used
uniform vec3 base_color : source_color;
uniform vec3 glow_color : source_color = vec3(0.2, 0.15, 0.3); // Relative to base color
uniform float pulse_speed : hint_range(0.0, 2.0) = 0.35;
uniform float pulse_strength : hint_range(0.0, 1.0) = 0.6;
uniform float vignette_radius : hint_range(0.0, 1.5) = 1.1;
uniform float vignette_softness : hint_range(0.0, 1.0) = 0.45;

void fragment() {
    // Use COLOR.rgb (from ColorRect's color property) as the base color instead of uniform
    vec3 effective_base_color = COLOR.rgb;
    vec3 effective_glow_color = effective_base_color + glow_color;
    
    float pulse = (sin(TIME * pulse_speed) * 0.5 + 0.5) * pulse_strength;
    vec3 color = mix(effective_base_color, effective_glow_color, pulse);

    vec2 centered = (UV - vec2(0.5));
    float vignette = smoothstep(vignette_radius, vignette_radius - vignette_softness, length(centered));
    color = mix(color, effective_base_color, vignette);

    COLOR = vec4(color, COLOR.a); // Preserve alpha from ColorRect
}
