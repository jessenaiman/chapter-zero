extends "res://addons/maaacks_game_template/base/nodes/menus/main_menu/main_menu.gd"

# Omega Main Menu with Omega branding and design
# Extends Maaacks MainMenu with Omega visual elements

func _ready() -> void:
	super._ready()
	_setup_omega_background()

func _setup_omega_background() -> void:
	# Replace or enhance the background with Omega branding
	var background = $BackgroundTextureRect
	if background:
		# Set Omega background color or logo
		background.color = Color(0.054902, 0.0666667, 0.0862745, 1)  # Deep space
		
		# You can load your Omega logo here:
		# background.texture = load("res://source/assets/logo/omega-spiral-logo.png")
		
		# Add Omega styling
		background.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_COVERED

# Override menu creation to use Omega buttons
func _create_menu_buttons() -> void:
	# This would be called from the parent class or you can override
	# specific button creation methods to use OmegaUiButton
	pass