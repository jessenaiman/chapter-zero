# Theme Manager Autoload
extends Node

# Set this as an autoload in Project Settings
# Name: ThemeManager
# Path: res://source/ui/theme_manager.gd

func _ready():
    # Load and apply the omega theme
    var theme = load("res://resources/themes/omega_theme.tres")
    if theme:
        # Apply to all UI in the current scene
        get_tree().current_scene.get_tree().call_group("ui_root", "apply_theme", theme)
        print("Omega theme applied successfully!")
    else:
        print("Failed to load omega theme!")

# Call this function to apply theme to any scene
func apply_theme_to_scene(scene_path: String) -> void:
    var theme = load("res://resources/themes/omega_theme.tres")
    if theme:
        # Load the scene and apply theme
        var scene = load(scene_path).instantiate()
        if scene:
            scene.call_deferred("apply_theme", theme)
            print("Theme applied to: ", scene_path)

# Utility function to find all Control nodes in a scene
func _find_controls(node: Node, controls: Array = []):
    if node is Control:
        controls.append(node)
    for child in node.get_children():
        controls = _find_controls(child, controls)
    return controls