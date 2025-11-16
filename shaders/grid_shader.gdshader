shader_type canvas_item;

uniform float grid_size : hint_range(10, 100) = 20.0;
uniform vec4 grid_color : hint_color = vec4(0.1, 0.1, 0.3, 0.5);

void fragment() {
	vec2 uv = UV * grid_size;
	float grid = step(0.9, fract(uv.x)) + step(0.9, fract(uv.y));
	COLOR = texture(TEXTURE, UV) + grid * grid_color;
}