shader_type particles;

// Particle color (will be overridden per trail)
uniform vec4 color : hint_color = vec4(1.0, 0.4, 0.0, 1.0);
uniform float size : hint_range(0.1, 5.0) = 1.0;

// Optional: Fade over lifetime
uniform bool fade_over_lifetime = true;

void vertex() {
	// Optional: grow slightly over time
	float life_ratio = CUSTOM.y; // 0 = born, 1 = dead
	if (fade_over_lifetime) {
		ALPHA = 1.0 - life_ratio;
	}
	POINT_SIZE = size * (1.0 + life_ratio * 0.5);
}

void fragment() {
	COLOR = color;
	COLOR.a = ALPHA;
}