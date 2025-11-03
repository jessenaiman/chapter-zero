## Per-level state persistence for Omega Spiral.
##
## Stores level-specific data including:
## - Tutorial read status (prevents tutorial from showing every playthrough)
## - Player's color preference
## - Dreamweaver narrative scores (tracks which AI persona's story dominates)
##
## One LevelState instance is created per unique level visited.
## Accessed via GameState.get_level_state(level_path).
class_name LevelState
extends Resource

## User-selected color preference for this level
@export var color : Color

## Whether tutorial message was dismissed in this level
@export var tutorial_read : bool = false

## Dreamweaver influence scores: [Light Thread, Dark Thread, Balance Thread]
## Tracks which narrative persona's choices dominated this level.
## Each score range: 0-100. Updated when level completes with player's choices.
@export var dreamweaver_scores : Array[int] = [0, 0, 0]