extends "res://addons/maaacks_game_template/base/nodes/autoloads/ui_sound_controller/ui_sound_controller.gd"

# Omega-specific UI sound extensions
@export var omega_button_sound: AudioStream

# Configure Omega Spiral UI sounds
func _ready():
	super._ready()
	# Set custom sounds for Omega Spiral UI
	button_hovered = preload("res://source/assets/sfx/confirmation_002.ogg")
	button_focused = preload("res://source/assets/sfx/confirmation_002.ogg")
	button_pressed = preload("res://source/assets/sfx/impactWood_light_002.ogg")

	# Rebuild stream players with new sounds
	_build_all_stream_players()
	_recursive_connect_ui_sounds(root_node)

func play_omega_sound() -> void:
	# Custom Omega UI sound logic
	if omega_button_sound:
		var player = AudioStreamPlayer.new()
		player.stream = omega_button_sound
		player.bus = audio_bus
		add_child(player)
		player.play()
		player.finished.connect(player.queue_free)