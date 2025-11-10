extends "res://levels/level.gd"

# Level 1 Ghost Terminal - Main level controller
# Inherits from base level.gd and integrates with the game's level management system

func _ready() -> void:
	super._ready()
	# Load the ghost terminal scene into the GameViewport
	_load_ghost_terminal()

func _load_ghost_terminal() -> void:
	var ghost_terminal_scene = preload("res://levels/level_1_ghost/ghost_terminal.tscn")
	var ghost_terminal_instance = ghost_terminal_scene.instantiate()
	
	# Add it to the GameViewport
	%GameViewport.add_child(ghost_terminal_instance)
	
	# Connect to the level_won signal from ghost terminal
	ghost_terminal_instance.level_won.connect(_on_ghost_terminal_won)

func _on_ghost_terminal_won() -> void:
	# Forward the signal to the level management system
	level_won.emit()
