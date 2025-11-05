extends Node
class_name GhostDialogicBridge

## Bridge between C# GhostCinematicDirector and Dialogic timeline system.
## Handles starting the Ghost Terminal timeline and capturing player choices.

signal timeline_completed(results: Dictionary)
signal choice_made(thread: String, choice_data: Dictionary)

var _scene_results: Array = []
var _current_thread: String = ""

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
	# Can be used for custom events from timeline

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
