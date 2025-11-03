extends "res://addons/maaacks_game_template/base/nodes/config/app_settings.gd"

# Omega Spiral specific application settings
# Extends base AppSettings with Omega-specific configuration sections

# Omega-specific configuration sections
const OMEGA_SECTION = &'OmegaSettings'
const DREAMWEAVER_SECTION = &'DreamweaverSettings'

# Omega-specific settings
const OMEGA_THEME = &'OmegaTheme'
const DREAMWEAVER_PREFERENCES = &'DreamweaverPreferences'
const CRT_EFFECTS_ENABLED = &'CRTEffectsEnabled'

# Default Omega settings
static var omega_defaults : Dictionary = {
	OMEGA_THEME: "default",
	DREAMWEAVER_PREFERENCES: ["light", "shadow", "ambition"],
	CRT_EFFECTS_ENABLED: true
}

# Get Omega-specific configuration
static func get_omega_config(setting_name : String, default = null):
	return PlayerConfig.get_config(OMEGA_SECTION, setting_name, default)

# Set Omega-specific configuration
static func set_omega_config(setting_name : String, value) -> void:
	PlayerConfig.set_config(OMEGA_SECTION, setting_name, value)

# Initialize Omega defaults if they don't exist
static func initialize_omega_defaults() -> void:
	for setting_name in omega_defaults:
		if get_omega_config(setting_name) == null:
			set_omega_config(setting_name, omega_defaults[setting_name])