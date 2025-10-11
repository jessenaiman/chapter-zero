shader_type canvas_item;
render_mode unshaded, blend_add;

uniform float intensity : hint_range(0.0, 1.0, 0.01) = 0.35;
uniform vec3 tint : source_color = vec3(0.2, 1.0, 0.6);
uniform float block_scale : hint_range(40.0, 220.0, 1.0) = 140.0;
uniform float speed : hint_range(0.0, 80.0, 0.1) = 28.0;

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

void fragment() {
    vec2 uv = UV;
    vec2 grid = floor(uv * block_scale);
    float t = TIME * speed;

    float noiseA = hash(grid + t);
    float noiseB = hash(grid * 1.73 - t * 0.5);
    float value = clamp(noiseA + noiseB * 0.75, 0.0, 1.0);

    float glyph = step(0.6, hash(grid + vec2(13.0, 91.0)));
    float shimmer = mix(value, glyph, 0.2);

    vec3 color = tint * shimmer * intensity;
    COLOR = vec4(color, shimmer * intensity);
}
