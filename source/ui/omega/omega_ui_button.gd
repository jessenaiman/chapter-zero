extends Button

# Omega UI Button with Omega-specific behavior
# Extends base Button with Omega visual feedback and navigation support

# Omega-specific hover/focus modulate values
@export var hover_brightness: float = 1.2
@export var focus_brightness: float = 1.1

func _ready() -> void:
	super._ready()
	
	# Omega-specific button behavior (not styling!)
	focus_mode = FocusModeEnum.All  # Support keyboard/gamepad navigation
	size_flags_horizontal = SizeFlags.ExpandFill  # Fill horizontal space by default
	
	# Add subtle visual feedback on interaction (beyond theme colors)
	mouse_entered.connect(_on_button_hover)
	focus_entered.connect(_on_button_focus)
	mouse_exited.connect(_on_button_unhover)
	focus_exited.connect(_on_button_unfocus)

func _on_button_hover() -> void:
	modulate = Color(hover_brightness, hover_brightness, hover_brightness, 1.0)

func _on_button_focus() -> void:
	modulate = Color(focus_brightness, focus_brightness, focus_brightness, 1.0)

func _on_button_unhover() -> void:
	modulate = Color.WHITE

func _on_button_unfocus() -> void:
	modulate = Color.WHITE