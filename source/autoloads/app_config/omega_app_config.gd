extends "res://addons/maaacks_game_template/base/nodes/autoloads/app_config/omega_app_config.gd"

@export_file("*.tscn") var opening_scene_path: String = "res://source/scenes/opening/opening.tscn"
@export var default_ui_theme: Theme = "res://source/themes/omega_theme_complete.tres"

func _ready() -> void:
	super()
	# Set the game scene path to our game UI
	game_scene_path = "res://source/scenes/game_scene/omega_spiral_game_ui.tscn"
	if default_ui_theme:
		ThemeDB.set_default_theme(default_ui_theme)
		var root_viewport := get_tree().root
		if root_viewport:
			root_viewport.theme = default_ui_theme
