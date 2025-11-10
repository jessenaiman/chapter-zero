extends Control
## Level 2: Nethack Echo Chamber - Three Dreamweaver narrators compete
## Integrates NethackCinematicDirector with Maaack's LevelManager system

signal level_won
signal level_lost

# Reference to the C# director
var nethack_director: Node

func _ready() -> void:
	# Create and start the Nethack Cinematic Director
	var director_script = load("res://source/scenes/game_scene/levels/level_2_nethack/NethackCinematicDirector.cs")
	nethack_director = director_script.new()
	add_child(nethack_director)
	
	# Run the stage asynchronously
	_run_nethack_stage()

func _run_nethack_stage() -> void:
	# Call the async RunStageAsync method
	var results = await nethack_director.RunStageAsync()
	
	print("[Level2Nethack] Stage complete! Results: ", results)
	
	# Extract chosen Dreamweaver
	var chosen_dreamweaver = results.get("chosen_dreamweaver", "")
	print("[Level2Nethack] Player chose Dreamweaver: ", chosen_dreamweaver)
	
	# TODO: Save chosen Dreamweaver to GameState for future stages
	# GameState.set_chosen_dreamweaver(chosen_dreamweaver)
	
	# Emit level_won to progress to next level
	level_won.emit()
