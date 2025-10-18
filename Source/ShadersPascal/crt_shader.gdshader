shader_type canvas_item;

uniform float time;

void fragment() {
    vec2 uv = UV;

    // CRT curvature
    uv = uv * 2.0 - 1.0;
    uv *= 1.0 + dot(uv, uv) * 0.1;
    uv = uv * 0.5 + 0.5;

    // Scanlines
    float scanline = sin(uv.y * 800.0 + time * 10.0) * 0.1;

    // Phosphor glow
    vec4 color = texture(TEXTURE, uv);
    color.rgb += vec3(0.0, 0.5, 0.0) * scanline;

    COLOR = color;
}
