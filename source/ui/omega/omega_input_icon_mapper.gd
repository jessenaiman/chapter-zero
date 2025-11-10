extends "res://addons/maaacks_game_template/base/nodes/menus/options_menu/input/input_icon_mapper.gd"

# Omega Input Icon Mapper
# Extends base InputIconMapper with Omega-specific icon configuration

# Omega-specific icon directories (will be set up when icons are downloaded)
const OMEGA_ICON_BASE_PATH = "res://source/assets/icons/input_prompts/"

# Omega-specific string replacements for better naming consistency
const OMEGA_REPLACE_STRINGS: Dictionary = {
	"L 1": "Left Shoulder",
	"R 1": "Right Shoulder", 
	"L 2": "Left Trigger",
	"R 2": "Right Trigger",
	"Lt": "Left Trigger",
	"Rt": "Right Trigger",
	"Lb": "Left Shoulder",
	"Rb": "Right Shoulder",
	"Guide": "Home",
	"Stick L": "Left Stick",
	"Stick R": "Right Stick",
	"Generic Stick": "Generic Left Stick"
}

func _ready() -> void:
	super._ready()
	_setup_omega_icon_mapping()

# Setup Omega-specific icon configuration
func _setup_omega_icon_mapping() -> void:
	# Add Omega-specific replace strings to base configuration
	for key in OMEGA_REPLACE_STRINGS:
		if not replace_strings.has(key):
			replace_strings[key] = OMEGA_REPLACE_STRINGS[key]
	
	# Configure for Omega terminal aesthetic
	# Prioritize clean, high-contrast icons that work with CRT theme
	if prioritized_strings.is_empty():
		prioritized_strings = ["default", "white"]
	
	# Filter out colored variants for cleaner terminal look
	if not filtered_strings.has("color"):
		filtered_strings.append("color")

# Get Omega-themed icon directory structure
func get_omega_icon_directories() -> Array[String]:
	var directories = []
	
	# Standard device directories (when icons are downloaded)
	directories.append(OMEGA_ICON_BASE_PATH + "Keyboard & Mouse/Default")
	directories.append(OMEGA_ICON_BASE_PATH + "Generic/Default")
	directories.append(OMEGA_ICON_BASE_PATH + "Xbox Series/Default")
	directories.append(OMEGA_ICON_BASE_PATH + "PlayStation Series/Default")
	directories.append(OMEGA_ICON_BASE_PATH + "Nintendo Switch/Default")
	
	return directories

# Check if Omega icons are available
func has_omega_icons() -> bool:
	var directories = get_omega_icon_directories()
	for dir in directories:
		if DirAccess.dir_exists_absolute(dir):
			return true
	return false

# Auto-configure Omega icons if available
func auto_configure_omega_icons() -> void:
	if has_omega_icons():
		directories = get_omega_icon_directories()
		# Trigger icon matching
		_match_icons_to_inputs()
		print("Omega input icons configured successfully")
	else:
		print("Omega input icons not found - run setup wizard to download icons")