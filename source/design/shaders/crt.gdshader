// source/ui/omega/crt_simple.gdshader
shader_type canvas_item;

uniform float scanline_opacity : hint_range(0.0, 0.3) = 0.05;
uniform float scanline_count : hint_range(100.0, 1000.0) = 400.0;
uniform vec3 tint_color : source_color = vec3(0.7725, 0.6196, 0.3725);
uniform float tint_strength : hint_range(0.0, 1.0) = 0.1;

void fragment() {
    vec4 color = texture(TEXTURE, UV);
    
    // Scanlines
    float scanline = sin(UV.y * scanline_count * 3.14159) * 0.5 + 0.5;
    color.rgb -= scanline * scanline_opacity;
    
    // Warm tint
    color.rgb = mix(color.rgb, tint_color, tint_strength);
    
    COLOR = color;
}