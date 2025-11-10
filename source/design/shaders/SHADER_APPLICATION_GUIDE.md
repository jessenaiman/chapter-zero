# Shader Application Guide for Omega Spiral

## How to Apply Shaders Properly in Godot 4.x

### 1. Understanding Shader Types

Godot has three main shader types:
- **CanvasItem Shader** - For 2D UI elements (labels, buttons, panels, etc.)
- **Spatial Shader** - For 3D objects (meshes, particles, etc.)
- **Sky Shader** - For sky backgrounds

Your `pulsing_background.gdshader` is a **CanvasItem Shader** - perfect for 2D UI work.

### 2. Node Type Requirements

CanvasItem shaders require specific node types to work properly:

**✅ Works with CanvasItem Shaders:**
- TextureRect
- ColorRect
- Control nodes (Panels, Buttons, Labels with proper setup)
- Viewport nodes

**❌ Does NOT work with CanvasItem Shaders:**
- Node2D
- Sprite2D
- AnimatedSprite2D
- Regular Node (non-CanvasItem)

### 3. How to Apply Shaders in Scene Files

#### Method 1: Direct Material Assignment
```tscn
[node name="Background" type="TextureRect" parent="."]
material = ExtResource("5_shader")  # Your ShaderMaterial resource
color = Color(0.15, 0.1, 0.2, 1)   # Base color (shades the shader)
```

#### Method 2: Create ShaderMaterial in Editor
1. In Godot Editor: Create a new ShaderMaterial
2. Drag your shader file to the "Shader" property
3. Adjust shader parameters in inspector
4. Assign the ShaderMaterial to your node

### 4. Your Pulsing Background Shader - Perfect Setup

Your shader is **perfectly designed** for this use case:

```glsl
// This line makes it work with ColorRect
vec3 base_color = mix(color_rect.rgb, base_tex.rgb, base_tex.a);
```

**Correct Usage:**
- Apply to `TextureRect` or `ColorRect`
- Set a base color in the node
- Shader will pulse and glow that base color

### 5. Common Shader Application Scenarios

#### Background Effects
```tscn
[node name="GameBackground" type="TextureRect" parent="."]
material = ExtResource("pulsing_shader")
color = Color(0.1, 0.1, 0.2, 1)  # Base color for pulsing
```

#### Button Glow Effects
```tscn
[node name="GlowButton" type="Button" parent="."]
material = ExtResource("button_glow_shader")
theme_override_font_sizes/font_size = 24
text = "Play Game"
```

#### Panel Borders
```tscn
[node name="MenuPanel" type="Panel" parent="."]
material = ExtResource("border_shader")
theme_override_styles/panel = null
```

#### Text Glow (Advanced)
```tscn
[node name="GlowingText" type="Label" parent="."]
material = ExtResource("text_glow_shader")
text = "OMEGA SPIRAL"
# Note: Text shaders require special setup
```

### 6. ShaderMaterial Creation Guide

#### Create in Editor:
1. Right-click in FileSystem → "New Resource" → "ShaderMaterial"
2. Assign your shader file (.gdshader)
3. Configure parameters in Inspector
4. Save as .tres file

#### Create Programmatically:
```csharp
// In C#
var shaderMaterial = new ShaderMaterial();
shaderMaterial.Shader = GD.Load<Shader>("res://your_shader.gdshader");
yourNode.Material = shaderMaterial;

// Set parameters
shaderMaterial.SetShaderParameter("glow_color", new Color(0.2, 0.15, 0.3, 1));
```

### 7. Debugging Shader Issues

**If your shader doesn't work:**

1. **Check Node Type**: Must be CanvasItem-compatible
2. **Check Material Assignment**: Verify ShaderMaterial is assigned
3. **Check Shader Parameters**: Ensure required uniforms have values
4. **Check Console**: Look for shader compilation errors
5. **Test Simple Shader**: Use a basic shader first

### 8. Your Specific Shaders in Action

#### For Game Backgrounds:
- `pulsing_background.gdshader` → ColorRect/TextureRect
- Perfect for atmospheric backgrounds
- Works with any base color

#### For UI Elements:
- Add glow effects to buttons
- Create animated panels
- Make text shimmer

#### For Menus:
- Background pulsing
- Button hover effects
- Panel edge glow

### 9. Best Practices for Your Game

**Level Backgrounds:**
```tscn
[node name="LevelBackground" type="TextureRect" parent="Viewport"]
material = ExtResource("pulsing_background_tres")
color = Color(0.15, 0.1, 0.2, 1)  # Deep purple base
```

**Main Menu:**
```tscn
[node name="MenuBackground" type="ColorRect" parent="."]
material = ExtResource("pulsing_background_tres")
color = Color(0.1, 0.1, 0.15, 1)  # Dark blue base
```

**UI Panels:**
```tscn
[node name="Panel" type="Panel" parent="."]
material = ExtResource("panel_glow_tres")
theme_override_styles/panel = null
```

### 10. Shader Performance Tips

- Use CanvasItem shaders for UI (lighter than Spatial)
- Limit expensive calculations in fragment shader
- Use uniforms for colors/parameters (recompile-friendly)
- Test on target hardware
- Profile with Godot's built-in profiler

## Your Shaders Are Ready to Use!

Your design system is well-structured. The shaders are properly designed and will work beautifully when applied to the correct node types. The key is understanding that CanvasItem shaders need CanvasItem-compatible nodes.

**Always Remember:**
- CanvasItem Shader → CanvasItem Node
- TextureRect/ColorRect → Perfect for backgrounds
- Material assignment → Essential step
- Base color → Shader output color