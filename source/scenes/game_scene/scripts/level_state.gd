## Omega Spiral per-level state persistence.
##
## Stores level-specific data that needs to survive between playthroughs.
## One OmegaSpiralLevelState instance is created per unique level visited.
## Accessed via OmegaSpiralGameState.get_level_state(level_path).
##
## Dreamweaver System:
## - Three AI narrative personas (Light, Dark, Balance threads)
## - Each level assigns scores based on player choices
## - Scores determine which Dreamweaver's story dominates
extends Resource

class_name OmegaSpiralLevelState

## User-selected color preference for this level (example feature from template)
@export var color : Color

## Whether tutorial message was dismissed in this level
## Prevents tutorial from showing every playthrough
@export var tutorial_read : bool = false

## Dreamweaver influence scores for this level: [Light, Dark, Balance]
## Updated when level completes with player's choices
## Each score is 0-100, tracking which narrative thread won
@export var dreamweaver_scores : Array[int] = [0, 0, 0]
