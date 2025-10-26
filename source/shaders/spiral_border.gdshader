shader_type canvas_item;

// Three Omega Spiral energy stream colors
uniform vec4 light_thread : source_color = vec4(0.95, 0.95, 1.0, 1.0);  // Silver/White
uniform vec4 shadow_thread : source_color = vec4(1.0, 0.75, 0.2, 1.0);  // Golden/Amber
uniform vec4 ambition_thread : source_color = vec4(0.9, 0.15, 0.1, 1.0); // Crimson/Red

// Animation and appearance parameters
uniform float rotation_speed : hint_range(0.0, 2.0, 0.01) = 0.1; // Slow spiral rotation
uniform float wave_speed : hint_range(0.0, 5.0, 0.1) = 1.0;       // Wave flow speed
uniform float wave_frequency : hint_range(1.0, 20.0, 0.5) = 8.0;  // Wavelength density
uniform float wave_amplitude : hint_range(0.0, 1.0, 0.01) = 0.3;  // Wave height
uniform float border_width : hint_range(0.0, 0.1, 0.001) = 0.02;  // Border thickness
uniform float glow_intensity : hint_range(0.0, 2.0, 0.1) = 1.0;   // Energy glow
uniform float thread_separation : hint_range(0.0, 6.28, 0.1) = 2.094; // 120 degrees in radians

void fragment() {
	vec2 center = vec2(0.5, 0.5);
	vec2 uv_centered = UV - center;
	
	// Calculate distance from center and angle
	float dist = length(uv_centered);
	float angle = atan(uv_centered.y, uv_centered.x);
	
	// Normalize distance to border region (0.0 at outer edge, 1.0 at inner edge of border)
	float outer_radius = 0.5;
	float inner_radius = outer_radius - border_width;
	float border_progress = 1.0 - smoothstep(inner_radius, outer_radius, dist);
	
	// Only render in the border region
	if (dist < inner_radius || dist > outer_radius) {
		COLOR = vec4(0.0);
		return;
	}
	
	// Animated spiral rotation
	float spiral_angle = angle + TIME * rotation_speed;
	
	// Three energy streams offset by 120 degrees each
	float stream1_angle = spiral_angle;
	float stream2_angle = spiral_angle + thread_separation;
	float stream3_angle = spiral_angle + thread_separation * 2.0;
	
	// Wavelike particle trails for each stream
	float wave_offset = TIME * wave_speed;
	float wave_pattern = sin((dist * wave_frequency * 100.0) - wave_offset);
	
	// Calculate intensity for each energy stream based on angle proximity
	float stream1_intensity = smoothstep(0.3, 0.0, abs(sin(stream1_angle * 1.5))) * (0.5 + wave_pattern * wave_amplitude);
	float stream2_intensity = smoothstep(0.3, 0.0, abs(sin(stream2_angle * 1.5))) * (0.5 + wave_pattern * wave_amplitude);
	float stream3_intensity = smoothstep(0.3, 0.0, abs(sin(stream3_angle * 1.5))) * (0.5 + wave_pattern * wave_amplitude);
	
	// Combine the three colored energy streams
	vec3 final_color = vec3(0.0);
	final_color += light_thread.rgb * stream1_intensity;
	final_color += shadow_thread.rgb * stream2_intensity;
	final_color += ambition_thread.rgb * stream3_intensity;
	
	// Apply glow and border fade
	float alpha = border_progress * glow_intensity;
	float energy_alpha = max(stream1_intensity, max(stream2_intensity, stream3_intensity));
	
	COLOR = vec4(final_color, alpha * energy_alpha);
}
