extends "res://addons/maaacks_game_template/base/nodes/autoloads/app_config/app_config.gd"

@export_file("*.tscn") var opening_scene_path: String = "res://source/scenes/opening.tscn"
@export_file("*.tscn") var boot_scene_path: String = "res://source/scenes/boot_sequence.tscn"

func _ready() -> void:
	super()
	# Omega-specific initialization