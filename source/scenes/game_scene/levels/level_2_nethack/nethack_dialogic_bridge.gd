extends Node
class_name NethackDialogicBridge

## Bridge between C# NethackCinematicDirector and Dialogic timeline system.
## Handles the Echo Chamber timeline and captures Dreamweaver selection.

signal timeline_completed(results: Dictionary)

func _ready() -> void:
	# Connect to Dialogic signals
	Dialogic.timeline_ended.connect(_on_timeline_ended)

## Start the Nethack Echo Chamber timeline
func start_nethack_timeline() -> void:
	print("[NethackDialogicBridge] Starting nethack_echo_chamber timeline")
	Dialogic.start("nethack_echo_chamber")

## Handle timeline completion
func _on_timeline_ended() -> void:
	print("[NethackDialogicBridge] Timeline ended")
	
	# Collect all choices and final Dreamweaver selection
	var results = {
		"chosen_dreamweaver": Dialogic.VAR.get("chosen_dreamweaver") or "",
		"light_score": Dialogic.VAR.get("light_score") or 0,
		"shadow_score": Dialogic.VAR.get("shadow_score") or 0,
		"ambition_score": Dialogic.VAR.get("ambition_score") or 0,
		"chamber_light_choice": Dialogic.VAR.get("chamber_light_choice") or "",
		"chamber_shadow_choice": Dialogic.VAR.get("chamber_shadow_choice") or "",
		"chamber_ambition_choice": Dialogic.VAR.get("chamber_ambition_choice") or "",
		"interlude_1_alignment": Dialogic.VAR.get("interlude_1_alignment") or "",
		"interlude_2_alignment": Dialogic.VAR.get("interlude_2_alignment") or ""
	}
	
	print("[NethackDialogicBridge] Results: ", results)
	emit_signal("timeline_completed", results)

## Get the selected Dreamweaver for scoring
func get_chosen_dreamweaver() -> String:
	return Dialogic.VAR.get("chosen_dreamweaver") or ""

## Get full results as dictionary
func get_nethack_results() -> Dictionary:
	return {
		"chosen_dreamweaver": Dialogic.VAR.get("chosen_dreamweaver") or "",
		"light_score": Dialogic.VAR.get("light_score") or 0,
		"shadow_score": Dialogic.VAR.get("shadow_score") or 0,
		"ambition_score": Dialogic.VAR.get("ambition_score") or 0
	}
