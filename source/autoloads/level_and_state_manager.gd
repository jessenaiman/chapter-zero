class_name OmegaGameManager
extends LevelManager

# Dreamweaver scoring system integrated directly
var _dreamweaver_scores : Array[int] = [0, 0, 0]  # [light, shadow/mischief, ambition/wrath]

func _ready() -> void:
	# Connect to story system signals for Dreamweaver score updates
	var storybook_engine = get_node_or_null("/root/StorybookEngine")  # Adjust path as needed
	if storybook_engine:
		storybook_engine.connect("DreamweaverScoresUpdated", Callable(self, "_on_dreamweaver_scores_updated"))
	else:
		push_warning("StorybookEngine not found at /root/StorybookEngine")

	# Load existing scores if available
	_load_dreamweaver_scores()

func _on_dreamweaver_scores_updated(scores : Array) -> void:
	_dreamweaver_scores = scores.duplicate()
	_save_dreamweaver_scores()

# Dreamweaver scoring API
func get_dreamweaver_scores() -> Array[int]:
	return _dreamweaver_scores.duplicate()

func update_dreamweaver_score(dreamweaver_index : int, points : int) -> void:
	if dreamweaver_index >= 0 and dreamweaver_index < _dreamweaver_scores.size():
		_dreamweaver_scores[dreamweaver_index] += points
		_save_dreamweaver_scores()

# Persistence helpers (using a simple approach for now)
func _load_dreamweaver_scores() -> void:
	# TODO: Implement actual persistence loading
	pass

func _save_dreamweaver_scores() -> void:
	# TODO: Implement actual persistence saving
	print("Dreamweaver scores updated: ", _dreamweaver_scores)
