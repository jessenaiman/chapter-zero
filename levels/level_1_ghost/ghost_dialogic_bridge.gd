extends Node
class_name GhostDialogicBridge

## Bridge between C# GhostCinematicDirector and Dialogic timeline system.
## Handles starting the Ghost Terminal timeline and capturing player choices.

signal timeline_completed(results: Dictionary)
signal choice_made(thread: String, choice_data: Dictionary)
signal pixel_dissolve_requested()
signal ascii_static_requested()

var _scene_results: Array = []
var _current_thread: String = ""
var _pixel_dissolve_active: bool = false
var _ascii_static_active: bool = false

func _ready() -> void:
	# Connect to Dialogic signals
	Dialogic.signal_event.connect(_on_dialogic_signal)
	Dialogic.timeline_ended.connect(_on_timeline_ended)

## Start the Ghost Terminal timeline
func start_ghost_timeline() -> void:
	print("[GhostDialogicBridge] Starting ghost_terminal timeline")
	Dialogic.start("ghost_terminal")

## Handle Dialogic custom signals
func _on_dialogic_signal(argument: String) -> void:
	print("[GhostDialogicBridge] Received signal: ", argument)
	
	# Handle visual effect signals from timeline
	match argument:
		"PIXEL_DISSOLVE_START":
			_start_pixel_dissolve()
		"ASCII_STATIC_START":
			_start_ascii_static()
		"PIXEL_DISSOLVE_END":
			_end_pixel_dissolve()
		"ASCII_STATIC_END":
			_end_ascii_static()
		_:
			# Handle other custom signals
			pass

## Handle timeline completion
func _on_timeline_ended() -> void:
	print("[GhostDialogicBridge] Timeline ended")
	
	# Collect all the choices player made
	var results = {
		"thread": Dialogic.VAR.get("player_thread") or "",
		"story_choice": Dialogic.VAR.get("player_story_choice") or "",
		"role": Dialogic.VAR.get("player_role") or "",
		"name_view": Dialogic.VAR.get("player_name_view") or "",
		"name_story": Dialogic.VAR.get("player_name_story") or ""
	}
	
	print("[GhostDialogicBridge] Results: ", results)
	emit_signal("timeline_completed", results)

## Get the selected thread for scoring
func get_selected_thread() -> String:
	return Dialogic.VAR.get("player_thread") or ""

## Get all player choices as dictionary
func get_player_choices() -> Dictionary:
	return {
		"thread": Dialogic.VAR.get("player_thread") or "",
		"story_choice": Dialogic.VAR.get("player_story_choice") or "",
		"role": Dialogic.VAR.get("player_role") or "",
		"name_view": Dialogic.VAR.get("player_name_view") or "",
		"name_story": Dialogic.VAR.get("player_name_story") or ""
	}

## Start pixel dissolve effect
func _start_pixel_dissolve() -> void:
	if not _pixel_dissolve_active:
		_pixel_dissolve_active = true
		pixel_dissolve_requested.emit()
		print("[GhostDialogicBridge] Starting pixel dissolve effect")

## Start ASCII static transition
func _start_ascii_static() -> void:
	if not _ascii_static_active:
		_ascii_static_active = true
		ascii_static_requested.emit()
		print("[GhostDialogicBridge] Starting ASCII static transition")

## End pixel dissolve effect
func _end_pixel_dissolve() -> void:
	_pixel_dissolve_active = false
	print("[GhostDialogicBridge] Pixel dissolve effect ended")

## End ASCII static transition
func _end_ascii_static() -> void:
	_ascii_static_active = false
	print("[GhostDialogicBridge] ASCII static transition ended")

## Check if visual effects are active
func is_visual_effect_active() -> bool:
	return _pixel_dissolve_active or _ascii_static_active
