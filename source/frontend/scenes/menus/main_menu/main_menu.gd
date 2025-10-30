extends MainMenu

## Override to call GameManager.StartGameAsync() instead of loading a scene directly
func load_game_scene() -> void:
	# Get the GameManager autoload instance
	var game_manager = get_tree().root.get_node("GameManager")

	if game_manager == null:
		push_error("[MainMenu] GameManager autoload not found!")
		return

	# Hide the menu while game runs
	visible = false
	game_started.emit()

	# Await the game loop completion via C# Task
	var start_task = game_manager.StartGameAsync(0)
	await start_task

## Override to show New Game button even when game_scene_path is empty
## Our architecture uses GameManager to load stages, not direct scene paths
func _hide_new_game_if_unset() -> void:
	# Don't hide the button - GameManager handles the actual loading
	pass
