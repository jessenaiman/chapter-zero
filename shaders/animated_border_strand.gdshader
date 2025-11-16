shader_type canvas_item;

// This line is essential. It lets us read the pixels *behind* this node.
render_mode blend_add, unshaded;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_nearest;

// --- COLOR CONTROLS ---
// Use the palette you identified:
// Light Blue: #00FFFF, Red: #FF0000, Yellow: #FFFF00, White: #FFFFFF
uniform vec4 strand_color : source_color = vec4(1.0, 0.0, 0.0, 1.0); // e.g., Red
uniform vec4 highlight_color : source_color = vec4(1.0, 1.0, 1.0, 1.0); // White

// --- ANIMATION ---
uniform float speed = 1.0;
uniform float time_offset = 0.0;

// --- "MATH ART" CONTROLS ---
uniform float wave_amplitude : hint_range(0.0, 0.5) = 0.1; // "The Peaks"
uniform float wave_frequency : hint_range(1.0, 100.0) = 50.0;
uniform float noise_scale : hint_range(1.0, 20.0) = 5.0;
uniform float streak_stretch : hint_range(1.0, 30.0) = 10.0;
uniform float strand_density : hint_range(0.1, 1.0) = 0.5;


// --- SHADER LOGIC ---
// (Noise functions - unchanged)
float random(vec2 uv) { return fract(sin(dot(uv.xy, vec2(12.9898, 78.233))) * 43758.5453123); }
float noise(vec2 uv) {
    vec2 i = floor(uv); vec2 f = fract(uv);
    float a = random(i); float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0)); float d = random(i + vec2(1.0, 1.0));
    vec2 u = f * f * (3.0 - 2.0 * f);
    return mix(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.y * u.x;
}
float fbm(vec2 uv) {
    float value = 0.0; float amplitude = 0.5;
    for (int i = 0; i < 4; i++) {
        value += amplitude * noise(uv); uv *= 2.0; amplitude *= 0.5;
    }
    return value;
}


void fragment() {
    // 1. Read the "Real Border" from behind this pixel
    vec4 pixel_behind = texture(SCREEN_TEXTURE, SCREEN_UV);

    // If the pixel behind is transparent, this shader does nothing.
    // This confines the effect *only* to the 'SolidBorder' we drew.
    if (pixel_behind.a == 0.0) {
        discard;
    }

    // 2. Animate and distort UVs for the "lines"
    float time = (TIME * speed) + time_offset;
    vec2 noise_uv;

    // We can't use 'mirrored_uv' anymore, so we'll just base the
    // distortion direction on the screen UV.
    if (abs(UV.x - 0.5) > abs(UV.y - 0.5)) {
        // Left or Right side
        noise_uv.x = UV.x * noise_scale;
        noise_uv.y = UV.y * noise_scale * streak_stretch + time;
        noise_uv.x += sin(noise_uv.y * wave_frequency * 0.1) * wave_amplitude;
    } else {
        // Top or Bottom side
        noise_uv.x = UV.x * noise_scale * streak_stretch + time;
        noise_uv.y = UV.y * noise_scale;
        noise_uv.y += sin(noise_uv.x * wave_frequency * 0.1) * wave_amplitude;
    }

    // 3. Generate the strands from noise
    float noise_val = fbm(noise_uv);
    float density_low = (1.0 - strand_density) * 0.5 + 0.1;
    float density_high = density_low + 0.15;
    float strands = smoothstep(density_low, density_high, noise_val);

    // 4. Combine colors
    vec3 animated_color_base = mix(strand_color.rgb, highlight_color.rgb, noise(noise_uv * 0.5));
    vec3 animated_color = animated_color_base * strands;

    // 5. Apply final color.
    // We use 'blend_add', so this color is added *on top of* the 'SolidBorder'.
    // We multiply by the 'pixel_behind.a' just to be safe.
    COLOR = vec4(animated_color, strands * pixel_behind.a);
}