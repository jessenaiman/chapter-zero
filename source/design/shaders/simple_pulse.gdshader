shader_type canvas_item;

uniform vec4 glow_color : source_color = vec4(1.0, 0.8, 0.2, 1.0);
uniform float pulse_speed = 2.0;
uniform float pulse_strength = 0.3;

void fragment() {
    // Use the base color from the ColorRect/TextureRect
    vec3 base_color = COLOR.rgb;
    
    // Create a simple pulsing effect
    float pulse = (sin(TIME * pulse_speed) * 0.5 + 0.5) * pulse_strength;
    
    // Mix the base color with the glow color based on pulse
    vec3 final_color = mix(base_color, glow_color.rgb, pulse);
    
    // Output the final color
    COLOR = vec4(final_color, COLOR.a);
}