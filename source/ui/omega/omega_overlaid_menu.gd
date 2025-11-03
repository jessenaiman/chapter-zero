extends "res://addons/maaacks_game_template/base/nodes/overlaid_menu/overlaid_menu.gd"

# Omega Overlaid Menu with CRT frame and Omega styling
# Extends Maaacks OverlaidMenu with Omega visual design

# Omega-specific properties
@export var enable_crt_frame: bool = true
@export var crt_frame_color: Color = Color(0.7725, 0.6196, 0.3725, 1)  # Warm amber phosphor

func _ready() -> void:
	super._ready()
	
	if enable_crt_frame:
		_setup_omega_frame()

func _setup_omega_frame() -> void:
	# Create Omega CRT frame background
	var frame_bg = ColorRect.new()
	frame_bg.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	frame_bg.color = Color(0.054902, 0.0666667, 0.0862745, 1)  # Deep space background
	frame_bg.mouse_filter = Control.MOUSE_FILTER_IGNORE
	add_child(frame_bg)
	move_child(frame_bg, 0)  # Put behind everything
	
	# Add CRT shader material if available
	# Note: You can add your CRT shader here as needed
	# frame_bg.material = load("res://source/ui/omega/crt_frame_material.tres")

# Override to add Omega-specific styling to menu panel
func _apply_omega_styling() -> void:
	var menu_panel = $MenuPanelContainer
	if menu_panel:
		# Apply Omega theme styling
		menu_panel.add_theme_stylebox_override("panel", _create_omega_panel_style())

func _create_omega_panel_style() -> StyleBoxFlat:
	var style = StyleBoxFlat.new()
	style.bg_color = Color(0.1, 0.1, 0.15, 0.95)  # Semi-transparent dark
	style.border_width_left = 2
	style.border_width_right = 2
	style.border_width_top = 2
	style.border_width_bottom = 2
	style.border_color = crt_frame_color
	style.corner_radius_top_left = 4
	style.corner_radius_top_right = 4
	style.corner_radius_bottom_left = 4
	style.corner_radius_bottom_right = 4
	return style