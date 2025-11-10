extends Control
## Level 1: Ghost Terminal - Narrative stage using Dialogic
## Integrates GhostCinematicDirector with Maaack's LevelManager system

signal level_won

# Reference to the C# director
var ghost_director: Node

# Reference to visual effects
var pixel_dissolve_effect: Node
var ascii_static_transition: Node

func _ready() -> void:
	# Create and start the Ghost Cinematic Director
	var director_script = load("res://levels/level_1_ghost/GhostCinematicDirector.cs")
	ghost_director = director_script.new()
	add_child(ghost_director)
	
	# Get references to visual effects
	pixel_dissolve_effect = get_node("PixelDissolveEffect")
	ascii_static_transition = get_node("AsciiStaticTransition")
	
	# Run the stage asynchronously
	_run_ghost_stage()

func _run_ghost_stage() -> void:
	# Call the async RunStageAsync method
	var results = await ghost_director.RunStageAsync()
	
	print("[Level1Ghost] Stage complete! Results: ", results)
	
	# Extract player's thread choice for GameState
	var player_thread = results.get("thread", "")
	print("[Level1Ghost] Player chose thread: ", player_thread)
	
	# TODO: Save thread choice to GameState for scoring
	# GameState.set_player_thread(player_thread)
	
	# Emit level_won to progress to next level
	level_won.emit()

## Handle pixel dissolve effect completion
func _on_pixel_dissolve_complete() -> void:
	print("[Level1Ghost] Pixel dissolve effect completed")
	# Can be used for additional post-effect processing

## Handle ASCII static transition completion
func _on_ascii_static_complete() -> void:
	print("[Level1Ghost] ASCII static transition completed")
	# Can be used for additional post-effect processing

## Get visual effects status
func get_visual_effects_status() -> Dictionary:
	return {
		"pixel_dissolve_active": pixel_dissolve_effect != null,
		"ascii_static_active": ascii_static_transition != null
	}
