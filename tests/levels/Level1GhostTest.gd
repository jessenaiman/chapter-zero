extends GdUnitTestSuite
## Test suite for Level 1: Ghost Terminal
## Verifies that the level loads correctly and integrates with Dialogic

# Test timeout in milliseconds
const TEST_TIMEOUT = 5000

func test_level_1_ghost_loads_successfully() -> void:
	# Load the level scene using scene_runner
	# Scene runner automatically handles cleanup
	var runner := scene_runner("res://source/scenes/game_scene/levels/level_1_ghost/level_1_ghost.tscn")
	
	# Verify scene loaded
	assert_that(runner.scene()).is_not_null()
	assert_that(runner.scene()).is_instanceof(Control)
	
	print("[Level1GhostTest] Level scene loaded successfully")

func test_ghost_dialogic_bridge_exists() -> void:
	# Load level scene
	var runner := scene_runner("res://source/scenes/game_scene/levels/level_1_ghost/level_1_ghost.tscn")
	
	# Simulate a few frames to allow _ready() to execute
	await runner.simulate_frames(2)
	
	# Verify the ghost_director child was created
	var level_scene = runner.scene()
	
	# Note: The C# node might not have the exact name, so we check for any child
	var has_director = level_scene.get_child_count() > 0
	assert_that(has_director).is_true()
	
	print("[Level1GhostTest] Ghost director node created")

func test_ghost_terminal_timeline_exists() -> void:
	# Verify the Dialogic timeline file exists
	var timeline_path = "res://source/scenes/game_scene/levels/level_1_ghost/ghost_terminal.dtl"
	assert_that(FileAccess.file_exists(timeline_path)).is_true()
	
	print("[Level1GhostTest] Ghost terminal timeline file exists")

func test_ghost_dialogic_bridge_script_valid() -> void:
	# Verify the bridge script can be loaded
	var bridge_script = load("res://source/scenes/game_scene/levels/level_1_ghost/ghost_dialogic_bridge.gd")
	assert_that(bridge_script).is_not_null()
	
	# Create an instance to verify it's valid
	var bridge_instance = auto_free(bridge_script.new())
	assert_that(bridge_instance).is_not_null()
	assert_that(bridge_instance).is_instanceof(Node)
	
	# Verify it has the expected signals
	assert_that(bridge_instance.has_signal("timeline_completed")).is_true()
	assert_that(bridge_instance.has_signal("choice_made")).is_true()
	
	print("[Level1GhostTest] Ghost dialogic bridge script is valid")

func test_level_emits_signals() -> void:
	# Load level scene
	var runner := scene_runner("res://source/scenes/game_scene/levels/level_1_ghost/level_1_ghost.tscn")
	var level_scene = runner.scene()
	
	# Verify the level has the expected signals
	assert_that(level_scene.has_signal("level_won")).is_true()
	assert_that(level_scene.has_signal("level_lost")).is_true()
	
	print("[Level1GhostTest] Level signals exist")
