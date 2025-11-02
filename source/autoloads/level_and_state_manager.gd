extends LevelManager
## Omega extension of LevelManager for Dreamweaver score integration

func _ready() -> void:
	# Connect to story system signals for Dreamweaver score updates
	var storybook_engine = get_node_or_null("/root/StorybookEngine")  # Adjust path as needed
	if storybook_engine:
		storybook_engine.connect("DreamweaverPointsAwarded", Callable(self, "_on_dreamweaver_points_awarded"))
	else:
		push_warning("StorybookEngine not found at /root/StorybookEngine")

func _on_dreamweaver_points_awarded(dreamweaver_type : String, points : int) -> void:
	var current_scores = GameState.get_dreamweaver_scores()
	var index = _get_dreamweaver_index(dreamweaver_type)
	if index >= 0:
		current_scores[index] += points
		GameState.update_dreamweaver_scores(current_scores)

# Dreamweaver scoring API - now delegates to GameState
func get_dreamweaver_scores() -> Array[int]:
	return GameState.get_dreamweaver_scores()

func update_dreamweaver_score(dreamweaver_index : int, points : int) -> void:
	var current_scores = GameState.get_dreamweaver_scores()
	if dreamweaver_index >= 0 and dreamweaver_index < current_scores.size():
		current_scores[dreamweaver_index] += points
		GameState.update_dreamweaver_scores(current_scores)

func _get_dreamweaver_index(dreamweaver_type : String) -> int:
	match dreamweaver_type:
		"light": return 0
		"shadow": return 1
		"ambition": return 2
		_: return -1
