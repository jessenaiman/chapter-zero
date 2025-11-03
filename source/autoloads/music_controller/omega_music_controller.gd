extends "res://addons/maaacks_game_template/base/nodes/autoloads/music_controller/music_controller.gd"

# Omega-specific music extensions

# Exported variables for Omega theme music configuration
@export var omega_theme_main: AudioStream
@export var omega_theme_battle: AudioStream
@export var omega_theme_menu: AudioStream
@export var omega_theme_cutscene: AudioStream

# Current theme state tracking
var _current_omega_theme: AudioStream = null
var _is_omega_theme_playing: bool = false
var _omega_theme_volume: float = 0.0

# Theme categories for different game contexts
enum OmegaThemeContext {
	MAIN,
	BATTLE,
	MENU,
	CUTSCENE
}

# Plays the Omega theme based on context with optional volume and fade settings
func play_omega_theme(context: OmegaThemeContext = OmegaThemeContext.MAIN,
						volume_db: float = 0.0,
						fade_duration: float = 1.0) -> void:
	# Validate context and get appropriate audio stream
	var theme_stream: AudioStream = _get_theme_for_context(context)
	if not theme_stream:
		push_warning("No Omega theme audio stream found for context: " + OmegaThemeContext.keys()[context])
		return
	
	# Store current settings
	_current_omega_theme = theme_stream
	_omega_theme_volume = volume_db
	
	# Stop any existing Omega theme if currently playing
	if _is_omega_theme_playing:
		stop_omega_theme(fade_duration)
	
	# Play the new theme with fade-in
	var stream_player = play_stream(theme_stream)
	if stream_player:
		stream_player.volume_db = volume_db
		_is_omega_theme_playing = true
		
		# Apply fade-in if duration specified
		if fade_duration > 0:
			stream_player.volume_db = -80  # Start silent
			var tween = create_tween()
			tween.tween_property(stream_player, "volume_db", volume_db, fade_duration)

# Stops the current Omega theme with optional fade-out
func stop_omega_theme(fade_duration: float = 0.5) -> void:
	if not _is_omega_theme_playing:
		return
	
	# Find the current Omega theme player (we need to identify it from the base controller)
	if is_instance_valid(music_stream_player) and music_stream_player.stream == _current_omega_theme:
		if fade_duration > 0:
			var tween = create_tween()
			tween.tween_property(music_stream_player, "volume_db", -80, fade_duration)
			await tween.finished
	music_stream_player.stop()
	
	_is_omega_theme_playing = false
	_current_omega_theme = null

# Gets the appropriate theme audio stream for the given context
func _get_theme_for_context(context: OmegaThemeContext) -> AudioStream:
	match context:
		OmegaThemeContext.MAIN:
			return omega_theme_main
		OmegaThemeContext.BATTLE:
			return omega_theme_battle
	OmegaThemeContext.MENU:
			return omega_theme_menu
		OmegaThemeContext.CUTSCENE:
			return omega_theme_cutscene
		_:
			return omega_theme_main # Default fallback

# Pauses the current Omega theme
func pause_omega_theme() -> void:
	if _is_omega_theme_playing and is_instance_valid(music_stream_player):
		music_stream_player.pause()

# Resumes the current Omega theme
func resume_omega_theme() -> void:
	if _is_omega_theme_playing and is_instance_valid(music_stream_player):
		music_stream_player.play()

# Updates the volume of the currently playing Omega theme
func set_omega_theme_volume(volume_db: float) -> void:
	if _is_omega_theme_playing and is_instance_valid(music_stream_player):
		music_stream_player.volume_db = volume_db
	_omega_theme_volume = volume_db

# Checks if an Omega theme is currently playing
func is_omega_theme_playing() -> bool:
	return _is_omega_theme_playing and is_instance_valid(music_stream_player) and music_stream_player.playing

# Returns the current Omega theme context based on the current stream
func get_current_omega_theme_context() -> OmegaThemeContext:
	if not _current_omega_theme:
		return OmegaThemeContext.MAIN
	
	if _current_omega_theme == omega_theme_main:
		return OmegaThemeContext.MAIN
	elif _current_omega_theme == omega_theme_battle:
		return OmegaThemeContext.BATTLE
	elif _current_omega_theme == omega_theme_menu:
		return OmegaThemeContext.MENU
	elif _current_omega_theme == omega_theme_cutscene:
		return OmegaThemeContext.CUTSCENE
	else:
		return OmegaThemeContext.MAIN  # Default if unknown stream