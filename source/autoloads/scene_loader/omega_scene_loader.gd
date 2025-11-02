extends "res://addons/maaacks_game_template/base/nodes/autoloads/scene_loader/scene_loader.gd"

# Omega-specific scene loading extensions
func load_omega_scene(scene_path: String, show_loading: bool = true) -> void:
	# Custom logic for Omega scenes
	load_scene(scene_path, show_loading)