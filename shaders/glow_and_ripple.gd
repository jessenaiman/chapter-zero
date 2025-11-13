extends Button
var _exit_tween: Tween  # Reference to the exit animation tween
var _is_mouse_over = false  # Flag to track if mouse is hovering over the button
var _original_brightness = {}  # Dictionary to store original brightness values of child nodes
var _center1 = Vector2(0.5, 0.5)  # Center point coordinates for click effect
var _center2 = Vector2(0.5, 0.5)  # Center point coordinates for mouse hover effect

func _ready():
	material.set("shader_parameter/size", size); material.set("shader_parameter/time1", 1.0); material.set("shader_parameter/time2", 0.0)  # Initialize shader parameters
	pressed.connect(_on_pressed); mouse_exited.connect(_on_mouse_exited); mouse_entered.connect(_on_mouse_entered)  # Connect button signals to handler functions
	material.set("shader_parameter/center1", _center1); material.set("shader_parameter/center2", _center2)  # Set center point parameters

	# Check if style box exists and is StyleBoxFlat type, if so, calculate corner radius parameters
	var normal_style = get_theme_stylebox("normal")  # Get normal style box
	if normal_style and normal_style is StyleBoxFlat: material.set("shader_parameter/corner_radius", normal_style.corner_radius_top_left / size.y * 2)
	var text_color = modulate if modulate != Color(1,1,1,1) else Color(1,1,1,1)  # Get button color or use default white
	material.set("shader_parameter/color", text_color)  # Set shader color parameter

func _process(_delta):
	var local_mouse = (get_global_transform().affine_inverse() * get_global_mouse_position()) / size  # Convert mouse position to local coordinates
	if _is_mouse_over: _center2 = local_mouse; material.set("shader_parameter/center2", _center2)  # Update hover center point when mouse is hovering
	material.set("shader_parameter/center1", _center1)  # Update click center point

func _on_pressed():
	_center1 = (get_global_transform().affine_inverse() * get_global_mouse_position()) / size  # Set click position as center point
	create_tween().tween_property(material, "shader_parameter/time1", 1.0, 0.5).from(0.0)  # Create click animation

func _on_mouse_entered():
	_is_mouse_over = true  # Mark mouse as hovering
	if _exit_tween: _exit_tween.kill()  # Stop exit animation
	create_tween().tween_property(material, "shader_parameter/glow", 2.0, 0.2)  # Use tween animation to set glow intensity to 2
	for node in _get_all_children(self):  # Iterate through all child nodes
		if node is Label or node is RichTextLabel:  # Check if it's a text node
			var path = node.get_path()  # Get node path
			if not _original_brightness.has(path): _original_brightness[path] = node.modulate  # Store original brightness
			node.modulate = _original_brightness[path] * 2  # Increase text brightness
	set_process(true)  # Enable _process function
	create_tween().tween_property(material, "shader_parameter/time2", 0.35, 0.2)  # Create hover enter animation

func _on_mouse_exited():
	_is_mouse_over = false  # Mark mouse as left
	var center = Vector2(0.5, 0.5)  # Center point coordinates
	var exit_target = center + (_center2 - center).normalized() * 2.0  # Calculate exit target position
	_exit_tween = create_tween()  # Create exit animation
	_exit_tween.parallel().tween_property(self, "_center2", exit_target, 0.3)  # Move center point
	_exit_tween.parallel().tween_property(material, "shader_parameter/time2", 0.0, 0.3)  # Reset time parameter
	_exit_tween.parallel().tween_property(material, "shader_parameter/glow", 0.0, 0.2)  # Use tween animation to set glow intensity to 0
	_exit_tween.tween_callback(func(): _center2 = Vector2(0.5, 0.5); set_process(false))  # Reset center point after animation completes
	for node in _get_all_children(self):  # Iterate through all child nodes
		if (node is Label or node is RichTextLabel) and _original_brightness.has(node.get_path()):  # Check if it's a text node and brightness is stored
			node.modulate = _original_brightness[node.get_path()]  # Restore original brightness

func _get_all_children(node: Node) -> Array:
	var children = []  # Child nodes array
	for child in node.get_children(): children.append(child); children.append_array(_get_all_children(child))  # Recursively get all child nodes
	return children  # Return child nodes array
