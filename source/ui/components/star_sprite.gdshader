shader_type canvas_item;
render_mode blend_add;

uniform float intensity = 1.0;
uniform vec3 color_tint = vec3(1.0, 0.95, 0.8);

void fragment() {
    float dist = length(UV - vec2(0.5));
    float alpha = 1.0 - smoothstep(0.0, 0.5, dist);
    alpha = pow(alpha, 2.0); // Sharper falloff

    // Add some sparkle effect
    float sparkle = sin(TIME * 8.0 + UV.x * 10.0 + UV.y * 10.0) * 0.2 + 0.8;

    COLOR = vec4(color_tint * intensity * sparkle, alpha * intensity);
}