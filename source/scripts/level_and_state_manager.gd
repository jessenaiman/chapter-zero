extends LevelManager

func _ready() -> void:
	if Engine.is_editor_hint():
		return
	if not is_instance_valid(level_loader):
		push_warning("Omega LevelAndStateManager is autoloaded without a LevelLoader. Skipping scene hookups.")
		return
	super._ready()

func set_current_level_path(value: String) -> void:
	super.set_current_level_path(value)
	# Sync with game state
	OmegaSpiralGameState.set_current_level(value)
	OmegaSpiralGameState.get_level_state(value)

func _advance_level() -> bool:
	var _advanced := super._advance_level()
	if _advanced:
		OmegaSpiralGameState.level_reached(current_level_path)
		# Emit our own signal since GameState can't have signals
		emit_signal("dreamweaver_scores_updated", OmegaSpiralGameState.get_dreamweaver_scores())
	return _advanced

# Add this signal to this class since GameState can't have signals
signal dreamweaver_scores_updated(scores: Array)
