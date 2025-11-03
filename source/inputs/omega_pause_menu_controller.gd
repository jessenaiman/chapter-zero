extends "res://addons/maaacks_game_template/base/nodes/utilities/pause_menu_controller/pause_menu_controller.gd"

# Omega Spiral specific pause menu controller
# Extends base pause functionality with Omega-specific features

func _ready() -> void:
	super._ready()
	# Add Omega-specific pause menu initialization here
	# For example: custom pause sounds, Omega UI integration, etc.

# Override pause behavior for Omega Spiral requirements
func _pause_game() -> void:
	super._pause_game()
	# Add Omega-specific pause logic here
	# For example: pause Omega music, save game state, etc.

# Override resume behavior for Omega Spiral requirements  
func _resume_game() -> void:
	super._resume_game()
	# Add Omega-specific resume logic here
	# For example: resume Omega music, restore game state, etc.