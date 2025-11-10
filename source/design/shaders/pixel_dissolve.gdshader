shader_type canvas_item
render_mode unshaded

uniform float blur_amount : hint_range(0.0, 10.0) = 0.0
uniform float dissolve_progress : hint_range(0.0, 1.0) = 0.0
uniform vec2 texture_size = vec2(1920.0, 1080.0)

void fragment() {
    vec2 uv = FRAGCOORD.xy / texture_size;
    
    // Apply blur effect
    if (blur_amount > 0.0) {
        vec4 color = vec4(0.0);
        float total = 0.0;
        
        // Simple box blur
        for (float x = -2.0; x <= 2.0; x += 1.0) {
            for (float y = -2.0; y <= 2.0; y += 1.0) {
                vec2 offset = vec2(x, y) / texture_size * blur_amount;
                color += texture(TEXTURE, uv + offset);
                total += 1.0;
            }
        }
        
        COLOR = color / total;
    } else {
        COLOR = texture(TEXTURE, uv);
    }
    
    // Apply dissolve opacity
    COLOR.a *= (1.0 - dissolve_progress * 0.8);
}