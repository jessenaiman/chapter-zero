# GDScript Templates Plugin Evaluation for Omega Spiral

## Plugin Overview

**Repository**: https://github.com/rychtar/gdscript-templates  
**License**: MIT  
**Godot Version**: 4.0+  
**Author**: Michal Rychtar (2025)

## Key Features

1. **Smart Template Expansion** - Type keywords and expand them into full code blocks
2. **Descriptive Parameters** - Use meaningful parameter names like `{name}`, `{type}`
3. **Partial Parameter Support** - Fill only some parameters, rest become placeholders
4. **Interactive Preview Panel** - See template code before inserting
5. **Auto-completion Popup** - Browse available templates with Ctrl+Space
6. **Automatic Indentation** - Templates respect current code indentation
7. **Cursor Positioning** - Places cursor automatically using `|CURSOR|` marker
8. **User Templates** - Override or extend default templates with your own

## How It Works

### Keyboard Shortcuts
- `Ctrl+Space` - Open template suggestions popup
- `Ctrl+E` - Expand template on current line
- `Tab` - Quick expand after selecting from popup
- `ESC` - Close popup window

### Template Format
```json
{
  "keyword": "template code with {param1} and {param2}|CURSOR|"
}
```

### Example Templates
- `vec 10 20` → `Vector2(10, 20)`
- `func update delta float` → `func update(delta) -> float:|CURSOR|`
- `printd health` → `print("health: ", health)`

### Built-in Templates (80+)
- Functions: `func`, `ready`, `process`, `input`
- Variables: `export`, `onready`, `const`
- Control flow: `if`, `for`, `while`, `match`
- Signals: `signal`, `sigcon`, `sigem`
- Nodes: `addch`, `getnode`, `inst`
- Math: `vec2`, `vec3`, `lerp`, `clamp`

## Configuration
- Settings via **Project → Tools → Code Templates Settings**
- Templates saved to `user://code_templates.json`
- Can toggle default templates on/off
- Full JSON customization for user templates

## Relevance to Omega Spiral Project

### ✅ Benefits for Our Project

1. **Scene Structure Consistency**
   - Can create templates for common scene patterns (NarrativeTerminal, AsciiRoom, etc.)
   - Ensures consistent Node hierarchy across scenes
   - Reduces copy-paste errors

2. **C# Script Boilerplate**
   - Templates for common C# patterns (signals, exports, ready functions)
   - XML documentation templates
   - Consistent error handling patterns

3. **JSON Data Structure Templates**
   - Scene data JSON structure templates
   - Schema validation patterns
   - DreamweaverChoice structure templates

4. **Godot-Specific Patterns**
   - Node path references
   - Signal connections
   - Resource loading patterns
   - Scene transitions

5. **Team Productivity**
   - Faster scene creation
   - Consistent code style (aligns with StyleCop/FxCop requirements)
   - Reduces onboarding time for new developers

### ⚠️ Considerations

1. **GDScript Focus**
   - Plugin is designed for GDScript, not C#
   - Our project uses C# for all scripts
   - Would need to evaluate if it works with C# script editor

2. **Limited Scene File Support**
   - Primarily for code templates, not `.tscn` files
   - Scene structure templates would be in GDScript format
   - May not directly help with scene composition

3. **Learning Curve**
   - Team needs to learn keyword shortcuts
   - Need to maintain custom template library
   - Requires discipline to keep templates updated

4. **Maintenance Overhead**
   - Need to create custom templates for our patterns
   - Templates must be kept in sync with project standards
   - Version control for template configuration

## Recommendation: **CONDITIONAL YES**

### Use Cases Where It Would Help

1. **GDScript Scene Scripts** (if we have any)
   - For simple scene configuration scripts
   - For test harness scripts in GDScript

2. **Documentation Templates**
   - README structure templates
   - ADR templates (though we have these already)
   - Comment block templates

3. **JSON Data Templates**
   - If the plugin can be extended to work with JSON files
   - Scene data structure templates
   - Schema templates

### Why NOT to Adopt (Current Assessment)

1. **C# Incompatibility**
   - Our entire codebase is C#, not GDScript
   - Plugin designed for GDScript editor integration
   - No clear benefit for C# development

2. **Existing Tooling**
   - We already have:
     - StyleCop/FxCop for code standards
     - XML documentation requirements
     - .specify templates for specs/plans/tasks
     - Pre-commit hooks for quality gates

3. **Scene Templates Better Handled by**
   - PackedScene resources
   - Scene inheritance
   - C# scene builder patterns
   - Our existing SceneLoader/SceneManager architecture

## Alternative Approaches for Scene Format Consistency

### 1. Scene Inheritance in Godot
```
BaseNarrativeScene.tscn (template)
├── Scene1Narrative.tscn (inherits)
├── Scene2Modified.tscn (inherits)
└── Scene3Custom.tscn (inherits)
```

### 2. C# Scene Builder Pattern
```csharp
public class SceneBuilder
{
    public static Node2D CreateNarrativeScene(string sceneName)
    {
        var scene = new Node2D { Name = sceneName };
        scene.AddChild(CreateTerminalUI());
        scene.AddChild(CreateCRTEffect());
        return scene;
    }
}
```

### 3. JSON Scene Templates with Validation
```json
{
  "scene_template": "narrative",
  "required_nodes": ["TerminalUI", "CRTEffect", "InputHandler"],
  "validation_schema": "res://schemas/narrative_scene.json"
}
```

### 4. Custom Editor Plugin (C#-based)
- Build our own Godot editor plugin in C#
- Tailored to our scene patterns
- Integrates with our JSON data architecture
- Supports our DreamweaverSystem integration

## Final Verdict

**DO NOT ADOPT** gdscript-templates plugin at this time because:

1. Our project is 100% C# - plugin is GDScript-focused
2. We have robust templating via .specify system
3. Scene consistency better achieved through inheritance and C# builders
4. No clear ROI for learning curve + maintenance overhead

**ALTERNATIVE**: Consider building a custom C# editor plugin if scene template needs become critical.

**MONITOR**: If plugin adds C# support in future, re-evaluate.

## Action Items

- [ ] Document scene inheritance patterns in ADR
- [ ] Create C# SceneBuilder utility class for common patterns
- [ ] Enhance .specify templates for scene-specific workflows
- [ ] Add scene validation to pre-commit hooks
