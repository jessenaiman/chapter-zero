shader_type canvas_item;

// --- CONTROLS ---
uniform vec4 strand_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float strand_width : hint_range(0.0, 0.05) = 0.008;
uniform float speed = 0.5;
uniform float time_offset = 0.0;
uniform float complexity_x = 1.0;
uniform float complexity_y = 2.0;
uniform float complexity_z = 3.0;
uniform float scale = 0.4; // How big the symbol is (0.5 = full panel)


// --- SHADER LOGIC ---

float draw_line(vec2 p1, vec2 p2, vec2 uv, float width) {
    vec2 dir = p2 - p1;
    float len = length(dir);
    if (len == 0.0) return 0.0;
    dir /= len;

    vec2 normal = vec2(-dir.y, dir.x);
    vec2 p_to_uv = uv - p1;

    float dist_along = dot(p_to_uv, dir);
    float dist_perp = abs(dot(p_to_uv, normal));

    if (dist_along >= 0.0 && dist_along <= len) {
        return smoothstep(width + 0.002, width, dist_perp);
    }
    return 0.0;
}

// Function to get the animated path position (Lemniscate/Fig-8)
vec2 get_path_pos(float t) {
    vec2 pos;
    // Simple figure-eight
    pos.x = 0.5 + scale * sin(t * complexity_x);
    pos.y = 0.5 + (scale * 0.7) * sin(t * complexity_y + sin(t * complexity_z));
    return pos;
}

void fragment() {
    float time = (TIME * speed) + time_offset;

    vec2 p1 = get_path_pos(time);
    vec2 p2 = get_path_pos(time + 0.1);
    float strand = draw_line(p1, p2, UV, strand_width);

    if (strand == 0.0) {
        discard;
    }

    COLOR = vec4(strand_color.rgb, strand * strand_color.a);
}