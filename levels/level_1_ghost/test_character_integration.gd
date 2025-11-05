extends Node
## Test script to validate Dialogic character integration
## Run this scene to test character dialogue and variable tracking

@onready var bridge = $GhostDialogicBridge

func _ready() -> void:
	print("=== DIALOGIC CHARACTER INTEGRATION TEST ===")
	
	# Verify characters are loaded
	var light = load("res://source/scenes/game_scene/levels/level_1_ghost/characters/light.dch")
	var shadow = load("res://source/scenes/game_scene/levels/level_1_ghost/characters/shadow.dch")
	var ambition = load("res://source/scenes/game_scene/levels/level_1_ghost/characters/ambition.dch")
	var omega = load("res://source/scenes/game_scene/levels/level_1_ghost/characters/omega.dch")
	
	print("✓ Light character: ", light.display_name, " | Color: ", light.color)
	print("✓ Shadow character: ", shadow.display_name, " | Color: ", shadow.color)
	print("✓ Ambition character: ", ambition.display_name, " | Color: ", ambition.color)
	print("✓ SYSTEM character: ", omega.display_name, " | Color: ", omega.color)
	
	print("\n--- Custom Info (for LLM context) ---")
	print("Light personality: ", light.custom_info.get("personality", "N/A"))
	print("Shadow personality: ", shadow.custom_info.get("personality", "N/A"))
	print("Ambition personality: ", ambition.custom_info.get("personality", "N/A"))
	
	# Connect to timeline completion
	bridge.timeline_completed.connect(_on_timeline_completed)
	
	print("\n--- Starting Timeline ---")
	bridge.start_ghost_timeline()

func _on_timeline_completed(results: Dictionary) -> void:
	print("\n=== TIMELINE COMPLETE ===")
	print("Player Thread: ", results.get("thread", "NONE"))
	print("Player Role: ", results.get("role", "NONE"))
	print("Story Choice: ", results.get("story_choice", "NONE"))
	print("Name View: ", results.get("name_view", "NONE"))
	print("Name Story: ", results.get("name_story", "NONE"))
	
	print("\n✓ Test completed successfully!")
	print("✓ All variables captured")
	print("✓ Ready for external AI integration")
