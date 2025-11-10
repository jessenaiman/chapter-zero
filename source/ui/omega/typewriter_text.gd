# File: source/ui/omega/dialogic_typewriter_text.gd
extends RichTextLabel
class_name DialogicTypewriterText

## Custom Dialogic text node with TypeWriter effect for ghostwriting
## Integrates with Dialogic's reveal system while providing terminal-style typing

signal started_revealing_text()
signal continued_revealing_text(new_character: String)
signal finished_revealing_text()

enum Alignment {LEFT, CENTER, RIGHT}

@export var enabled := true
@export var alignment := Alignment.LEFT
@export var textbox_root: Node = self
@export var hide_when_empty := false
@export var start_hidden := true
@export var typing_speed: float = 30.0

var revealing := false
var base_visible_characters := 0
var active_speed: float = 0.01
var speed_counter: float = 0

# TypeWriter effect variables
var _typing_time_gap: float = 0.0
var _typing_timer: float = 0.0
var _stop_timer: float = 0.0
var _text_to_type: String = ""
var _paused: bool = false

func _ready() -> void:
    add_to_group('dialogic_dialog_text')
    bbcode_enabled = true
    
    if textbox_root == null:
        textbox_root = self
    
    if start_hidden:
        textbox_root.hide()
    
    text = ""
    _typing_time_gap = 1.0 / typing_speed
    
    # Load custom BBCode effects
    var custom_bbcode_effects: Array = ProjectSettings.get_setting("dialogic/text/custom_bbcode_effects", "").split(",", false)
    for i in custom_bbcode_effects:
        var x: Resource = load(i.strip_edges())
        if x is RichTextEffect:
            custom_effects.append(x)

func _process(delta: float) -> void:
    if revealing and !_paused:
        if !_text_to_type.is_empty():
            if _stop_timer <= 0:
                var next_chars := ""
                while !_text_to_type.is_empty() and _typing_timer <= 0:
                    var next_char = _text_to_type[0]
                    _text_to_type = _text_to_type.erase(0)
                    next_chars += next_char
                    _typing_timer += _typing_time_gap
                    
                    # Stop after punctuation
                    if next_char in [".", "!", "?", ","]:
                        _stop_timer = 0.15
                        break
                
                visible_characters += next_chars.length()
                continued_revealing_text.emit(next_chars)
                _typing_timer -= delta
            _stop_timer -= delta
        else:
            finished_revealing_text.emit()
            revealing = false

func reveal_text(_text: String, keep_previous := false) -> void:
    if !enabled:
        return
    
    show()
    
    if !keep_previous:
        text = _text
        base_visible_characters = 0
        
        if alignment == Alignment.CENTER:
            text = '[center]' + text
        elif alignment == Alignment.RIGHT:
            text = '[right]' + text
        
        visible_characters = 0
    else:
        base_visible_characters = len(text)
        visible_characters = len(get_parsed_text())
        text = text + _text
    
    # Remove BBCode for typing
    var regex = RegEx.new()
    regex.compile("\\[img.*\\].*\\[\\/img\\]")
    var text_without_img = regex.sub(_text, " ", true)
    regex.compile("\\[[^\\]]+\\]")
    _text_to_type = regex.sub(text_without_img, "", true)
    
    revealing = true
    _typing_timer = 0.0
    _stop_timer = 0.0
    _paused = false
    started_revealing_text.emit()

func set_speed(delay_per_character: float) -> void:
    active_speed = delay_per_character
    typing_speed = 1.0 / max(delay_per_character, 0.001)
    _typing_time_gap = delay_per_character

func finish_text() -> void:
    _text_to_type = ""
    visible_characters = -1
    revealing = false
    finished_revealing_text.emit()

func pause() -> void:
    _paused = true

func resume() -> void:
    _paused = false

func get_total_character_count_custom() -> int:
    return get_parsed_text().length()

func is_typewriter_finished() -> bool:
    return !revealing