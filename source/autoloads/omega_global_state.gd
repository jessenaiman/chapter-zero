extends "res://addons/maaacks_game_template/base/nodes/state/global_state.gd"

const OmegaGlobalStateData := preload("res://source/data/omega_global_state_data.gd")

## Ensures the base GlobalState always provides an Omega-specific data resource.
static func _load_current_state() -> void:
	if current is OmegaGlobalStateData:
		return
	if FileAccess.file_exists(SAVE_STATE_PATH):
		current = ResourceLoader.load(SAVE_STATE_PATH)
	if current is not OmegaGlobalStateData:
		current = OmegaGlobalStateData.new()

## Strongly typed accessor for Omega code paths.
static func get_omega_state() -> OmegaGlobalStateData:
	_load_current_state()
	return current
