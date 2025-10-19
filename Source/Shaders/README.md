# Stage 1 CRT Shaders

This directory contains the custom shaders for Stage 1's terminal aesthetic.

## CRT_Phosphor.gdshader

**Purpose:** Base visual layer creating authentic CRT monitor appearance

**Features:**
- Screen curvature (barrel distortion for curved glass simulation)
- RGB phosphor glow (sub-pixel color separation)
- Vignette darkening (brightness falloff at edges)
- Thread-specific color tinting (Dreamweaver themes)
- Chromatic aberration (color channel separation at edges)
- Brightness & contrast controls

**Thread Color Presets:**
```gdscript
// Light Thread (Golden Certainty)
phosphor_tint = Vector3(1.0, 0.9, 0.5)
phosphor_glow = 1.4
vignette_strength = 0.15

// Shadow Thread (Violet Patience)  
phosphor_tint = Vector3(0.6, 0.3, 0.8)
phosphor_glow = 1.2
vignette_strength = 0.5

// Ambition Thread (Crimson Self)
phosphor_tint = Vector3(1.0, 0.2, 0.05)
phosphor_glow = 1.3
vignette_strength = 0.45
```

**Usage in TerminalBase.cs:**
```csharp
// Change phosphor color for Dreamweaver thread
SetShaderParameter(ShaderLayer.Phosphor, "phosphor_tint", new Vector3(1.0f, 0.9f, 0.5f)); // Light

// Intensify glow during secret reveal
SetShaderParameter(ShaderLayer.Phosphor, "phosphor_glow", 2.5f);
```

## Shader Layer Stack

Applied in this order on TerminalBase.tscn:
1. **PhosphorLayer** - CRT_Phosphor.gdshader (base color/curvature)
2. **ScanlineLayer** - CRT_Scanlines.gdshader (TODO: movement overlay)
3. **GlitchLayer** - CRT_Glitch.gdshader (TODO: boot/reveal effects)

## Performance Notes

- Shaders run on GPU, minimal CPU impact
- Screen curvature is the most expensive effect
- Can disable curvature on low-end hardware (set `curvature_strength = 0.0`)
- All effects are real-time adjustable via shader parameters
