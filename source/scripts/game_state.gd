## Game state persistence layer for Omega Spiral.
##
## Manages:
## - Current level progression and level-specific state
## - Dreamweaver scoring (3 AI narrative personas)
## - Game session tracking (playtime, games played, etc.)
##
## This Resource is automatically persisted to disk by GlobalState (Maaack's addon).
## Use static methods to read/write state - never instantiate directly.
##
## Architecture:
## - Maaack's GlobalState (addon) manages file I/O to user://
## - This class defines what data gets saved
## - LevelState stores per-level data (dreamweaver scores, tutorials, etc.)
class_name GameState
extends Resource

const STATE_NAME : String = "GameState"
const FILE_PATH = "res://source/scripts/game_state.gd"
const _GlobalState = preload("res://source/autoloads/omega_global_state.gd")

@export var level_states : Dictionary = {}
@export var current_level_path : String
@export var continue_level_path : String
@export var total_games_played : int
@export var play_time : int
@export var total_time : int

static func get_level_state(level_state_key : String) -> LevelState:
	if not has_game_state(): 
		return
	var game_state := get_or_create_state()
	if level_state_key.is_empty() : return
	if level_state_key in game_state.level_states:
		return game_state.level_states[level_state_key] 
	else:
		var new_level_state := LevelState.new()
		game_state.level_states[level_state_key] = new_level_state
		_GlobalState.save()
		return new_level_state

static func has_game_state() -> bool:
	return _GlobalState.has_state(STATE_NAME)

static func get_or_create_state() -> GameState:
	return _GlobalState.get_or_create_state(STATE_NAME, FILE_PATH)

static func get_current_level_path() -> String:
	if not has_game_state(): 
		return ""
	var game_state := get_or_create_state()
	return game_state.current_level_path

static func get_levels_reached() -> int:
	if not has_game_state(): 
		return 0
	var game_state := get_or_create_state()
	return game_state.level_states.size()

static func level_reached(level_path : String) -> void:
	var game_state := get_or_create_state()
	game_state.current_level_path = level_path
	game_state.continue_level_path = level_path
	get_level_state(level_path)
	_GlobalState.save()

static func set_current_level(level_path : String) -> void:
	var game_state := get_or_create_state()
	game_state.current_level_path = level_path
	_GlobalState.save()

static func start_game() -> void:
	var game_state := get_or_create_state()
	game_state.total_games_played += 1
	_GlobalState.save()

static func continue_game() -> void:
	var game_state := get_or_create_state()
	game_state.current_level_path = game_state.continue_level_path
	_GlobalState.save()

static func reset() -> void:
	var game_state := get_or_create_state()
	game_state.level_states = {}
	game_state.current_level_path = ""
	game_state.continue_level_path = ""
	game_state.play_time = 0
	game_state.total_time = 0
	_GlobalState.save()

## Updates Dreamweaver scores for the current level.
## Expects 3 scores: [Light Thread, Dark Thread, Balance Thread]
## Called by stages when they complete with player choices.
static func update_dreamweaver_scores(scores : Array) -> void:
	var game_state := get_or_create_state()
	var level_state := get_level_state(game_state.current_level_path) as LevelState
	if level_state and scores.size() >= 3:
		level_state.dreamweaver_scores = scores
		GD.Print("Dreamweaver scores for %s: %s" % [game_state.current_level_path, level_state.dreamweaver_scores])
		_GlobalState.save()

## Retrieves the current Dreamweaver scores.
## Returns [0, 0, 0] if no scores set yet.
## Used for final scoring/narrative branching decisions.
static func get_dreamweaver_scores() -> Array:
	if not has_game_state():
		return [0, 0, 0]
	var game_state := get_or_create_state()
	var level_state := get_level_state(game_state.current_level_path) as LevelState
	if level_state:
		GD.Print("Retrieved Dreamweaver scores for %s: %s" % [game_state.current_level_path, level_state.dreamweaver_scores])
		return level_state.dreamweaver_scores.duplicate()
	return [0, 0, 0]
