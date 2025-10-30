extends MainMenu

## Override to call GameManager.StartGameAsync() instead of loading a scene directly
func load_game_scene() -> void:
	# Get the GameManager from the scene tree
	var game_manager = get_tree().root.get_node_or_null("GameManager")

	if game_manager == null:
		push_error("[MainMenu] GameManager not found in scene tree!")
		return

	# Call StartGameAsync (C# method accessible from GDScript)
	game_manager.StartGameAsync(0)

	# Hide the menu while game runs
	visible = false
	game_started.emit()

## Override to show New Game button even when game_scene_path is empty
## Our architecture uses GameManager to load stages, not direct scene paths
func _hide_new_game_if_unset() -> void:
	# Don't hide the button - GameManager handles the actual loading
	pass
