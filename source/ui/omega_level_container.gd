extends SubViewport

# Omega Spiral level container
# Extends SubViewport with Omega-specific level management features

# Omega-specific level properties
@export var omega_level_theme: String = "default"
@export var enable_omega_effects: bool = true

func _ready() -> void:
	super._ready()
	# Add Omega-specific initialization here
	# For example: setup Omega rendering effects, level-specific settings

# Override for Omega-specific level loading behavior
func _on_level_added(level: Node) -> void:
	super._on_level_added(level) if has_method("_on_level_added") else pass
	# Add Omega-specific level setup here
	# For example: configure level for Omega theme, setup dreamweaver elements

# Override for Omega-specific level cleanup
func _on_level_removed() -> void:
	super._on_level_removed() if has_method("_on_level_removed") else pass
	# Add Omega-specific cleanup here
	# For example: cleanup dreamweaver state, save level progress