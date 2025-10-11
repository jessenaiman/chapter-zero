shader_type canvas_item;
render_mode unshaded, blend_mix;

uniform float progress : hint_range(0.0, 1.0, 0.01) = 0.0;
uniform float scale : hint_range(32.0, 256.0, 1.0) = 140.0;
uniform vec4 tint_color : source_color = vec4(0.0, 0.05, 0.04, 1.0);

float hash(vec2 p) {
    p = vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)));
    return fract(sin(p.x + p.y) * 43758.5453);
}

void fragment() {
    vec2 uv = SCREEN_UV;
    vec2 grid = floor(uv * scale);
    float noise = hash(grid);
    vec4 src = texture(SCREEN_TEXTURE, uv);

    float mask = step(progress, noise);

    float grain = hash(grid + progress * 3.17);
    vec3 dissolve_color = mix(tint_color.rgb, vec3(grain), 0.25);

    vec3 final_rgb = mix(dissolve_color, src.rgb, mask);
    float final_alpha = mix(tint_color.a, src.a, mask);

    COLOR = vec4(final_rgb, final_alpha);
}
