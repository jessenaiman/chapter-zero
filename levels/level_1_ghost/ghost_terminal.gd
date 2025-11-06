extends Control
## Level 1: Ghost Terminal - Narrative stage using Dialogic
## Integrates GhostCinematicDirector with Maaack's LevelManager system

signal level_won

# Reference to the C# director
var ghost_director: Node

func _ready() -> void:
	# Create and start the Ghost Cinematic Director
	var director_script = load("res://levels/level_1_ghost/GhostCinematicDirector.cs")
	ghost_director = director_script.new()
	add_child(ghost_director)
	
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
