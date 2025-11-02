extends LevelManager

func _ready() -> void:
	# Connect to story system signals for Dreamweaver score updates
	var storybook_engine = get_node_or_null("/root/StorybookEngine")  # Adjust path as needed
	if storybook_engine:
		storybook_engine.connect("DreamweaverScoresUpdated", Callable(self, "_on_dreamweaver_scores_updated"))

func set_current_level_path(value : String) -> void:
	super.set_current_level_path(value)
	OmegaSpiralGameState.set_current_level(value)
	OmegaSpiralGameState.get_level_state(value)

func get_current_level_path() -> String:
	var state_level_path := OmegaSpiralGameState.get_current_level_path()
	if not state_level_path.is_empty():
		current_level_path = state_level_path
	return super.get_current_level_path()

func _advance_level() -> bool:
	var _advanced := super._advance_level()
	if _advanced:
		OmegaSpiralGameState.level_reached(current_level_path)
	return _advanced

func _on_dreamweaver_scores_updated(scores : Array) -> void:
	OmegaSpiralGameState.update_dreamweaver_scores(scores)
