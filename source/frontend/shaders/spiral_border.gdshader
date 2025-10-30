shader_type canvas_item;

// Three Omega Spiral energy stream colors (matching logo)
uniform vec4 light_thread : source_color = vec4(0.95, 0.95, 1.0, 1.0);  // Silver/White
uniform vec4 shadow_thread : source_color = vec4(1.0, 0.75, 0.2, 1.0);  // Golden/Amber
uniform vec4 ambition_thread : source_color = vec4(0.9, 0.15, 0.1, 1.0); // Crimson/Red

// Animation parameters
uniform float rotation_speed : hint_range(0.0, 2.0, 0.01) = 0.15;     // Spiral rotation speed
uniform float flow_speed : hint_range(0.0, 5.0, 0.1) = 2.0;           // Particle flow speed
uniform float particle_density : hint_range(1.0, 50.0, 1.0) = 20.0;   // Number of particles
uniform float trail_length : hint_range(0.1, 3.0, 0.05) = 1.5;        // Length of light trails
uniform float line_width : hint_range(0.001, 0.05, 0.001) = 0.008;    // Width of flowing lines
uniform float glow_intensity : hint_range(0.0, 3.0, 0.1) = 2.5;       // Glow strength

void fragment() {
	vec2 center = vec2(0.5, 0.5);
	vec2 uv_centered = UV - center;

	// Box distance for rectangular frame
	float box_dist = max(abs(uv_centered.x), abs(uv_centered.y));

	// Only draw a thin line at the very edge
	float edge_distance = abs(box_dist - 0.48); // Slightly inset from absolute edge

	// Discard if not on the border line
	if (edge_distance > line_width) {
		discard;
	}

	// Calculate angle around the frame
	float angle = atan(uv_centered.y, uv_centered.x);

	// Animated spiral rotation
	float spiral_rotation = TIME * rotation_speed;

	// Three spiraling particle streams (120 degrees apart)
	float stream1_base = angle + spiral_rotation;
	float stream2_base = angle + spiral_rotation + 2.094; // 120 degrees
	float stream3_base = angle + spiral_rotation + 4.188; // 240 degrees

	vec3 final_color = vec3(0.0);
	float total_alpha = 0.0;

	// Create flowing particle trails
	for (float i = 0.0; i < particle_density; i++) {
		float particle_offset = (i / particle_density) * 6.28318;
		float particle_progress = fract((i / particle_density) + TIME * flow_speed * 0.1);

		// Calculate angle differences (how close is this pixel to each particle stream)
		float angle_diff1 = abs(mod(angle - (stream1_base + particle_offset) + 3.14159, 6.28318) - 3.14159);
		float angle_diff2 = abs(mod(angle - (stream2_base + particle_offset) + 3.14159, 6.28318) - 3.14159);
		float angle_diff3 = abs(mod(angle - (stream3_base + particle_offset) + 3.14159, 6.28318) - 3.14159);

		// Create trailing effect (brighter at particle head, fading behind)
		float fade = (1.0 - particle_progress);

		// Stream intensities based on proximity to particles
		float stream1_intensity = smoothstep(trail_length, 0.0, angle_diff1) * fade;
		float stream2_intensity = smoothstep(trail_length, 0.0, angle_diff2) * fade;
		float stream3_intensity = smoothstep(trail_length, 0.0, angle_diff3) * fade;

		// Accumulate colors
		final_color += light_thread.rgb * stream1_intensity;
		final_color += shadow_thread.rgb * stream2_intensity;
		final_color += ambition_thread.rgb * stream3_intensity;

		total_alpha += stream1_intensity + stream2_intensity + stream3_intensity;
	}

	// Edge fade for smooth anti-aliasing
	float edge_fade = 1.0 - smoothstep(0.0, line_width, edge_distance);

	// Final color with glow
	final_color *= glow_intensity;
	total_alpha = clamp(total_alpha * glow_intensity * edge_fade, 0.0, 1.0);

	COLOR = vec4(final_color, total_alpha);
}
