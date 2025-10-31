shader_type canvas_item;
render_mode blend_add;

uniform float star_density = 0.1;
uniform float twinkle_speed = 2.0;
uniform float star_size = 2.0;
uniform vec3 star_color = vec3(1.0, 1.0, 0.8);
uniform float time_offset = 0.0;

float random(vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

float noise(vec2 st) {
    vec2 i = floor(st);
    vec2 f = fract(st);
    float a = random(i);
    float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0));
    float d = random(i + vec2(1.0, 1.0));
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

void fragment() {
    vec2 uv = UV;
    vec2 screen_size = vec2(textureSize(TEXTURE, 0));
    vec2 pixel_pos = uv * screen_size;

    // Create grid for star positions
    vec2 grid_pos = floor(pixel_pos / star_size);
    vec2 local_pos = mod(pixel_pos, star_size) / star_size;

    // Generate star at grid position
    float star_seed = random(grid_pos);
    float star_brightness = random(grid_pos + vec2(100.0, 200.0));

    // Only create stars based on density
    if (star_seed > star_density) {
        discard;
    }

    // Twinkle effect
    float twinkle = sin(TIME * twinkle_speed + star_seed * 6.28) * 0.3 + 0.7;

    // Star shape (simple cross/star pattern)
    float star_shape = 1.0 - length(local_pos - 0.5);
    star_shape = max(star_shape, 1.0 - length(local_pos - vec2(0.5, 0.2)));
    star_shape = max(star_shape, 1.0 - length(local_pos - vec2(0.5, 0.8)));
    star_shape = max(star_shape, 1.0 - length(local_pos - vec2(0.2, 0.5)));
    star_shape = max(star_shape, 1.0 - length(local_pos - vec2(0.8, 0.5)));

    // Apply brightness variation and twinkle
    float final_brightness = star_brightness * twinkle * star_shape;

    // Color with slight blue-white variation
    vec3 color = star_color * (0.8 + star_seed * 0.4);

    COLOR = vec4(color * final_brightness, final_brightness);
}