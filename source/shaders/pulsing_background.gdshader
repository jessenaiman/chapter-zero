shader_type canvas_item;

uniform vec3 base_color : source_color = vec3(0.02, 0.03, 0.05);
uniform vec3 glow_color : source_color = vec3(0.07, 0.06, 0.1);
uniform float pulse_speed : hint_range(0.0, 2.0) = 0.35;
uniform float pulse_strength : hint_range(0.0, 1.0) = 0.6;
uniform float vignette_radius : hint_range(0.0, 1.5) = 1.1;
uniform float vignette_softness : hint_range(0.0, 1.0) = 0.45;

void fragment() {
    float pulse = (sin(TIME * pulse_speed) * 0.5 + 0.5) * pulse_strength;
    vec3 color = mix(base_color, glow_color, pulse);

    vec2 centered = (UV - vec2(0.5));
    float vignette = smoothstep(vignette_radius, vignette_radius - vignette_softness, length(centered));
    color = mix(color, base_color, vignette);

    COLOR = vec4(color, 1.0);
}
