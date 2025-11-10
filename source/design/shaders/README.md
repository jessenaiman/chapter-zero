# Stage 1 CRT Shaders


## crt_phosphor.gdshader

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

## crt_scanlines.gdshader

**Purpose:** Movement overlay layer with animated horizontal scanlines

**Features:**

- Animated horizontal scanlines (electron beam simulation)
- Configurable speed, opacity, count, and thickness
- Thread-specific tinting and movement patterns
- Authentic CRT darkening (not brightening) effect

**Thread Configuration Presets:**

```gdscript
// Light Thread (Fast, Subtle)
scanline_opacity = 0.05
scanline_speed = 5.0
scanline_tint = Vector3(1.0, 1.0, 1.0)

// Shadow Thread (Slow, Heavy)
scanline_opacity = 0.12
scanline_speed = 3.0
scanline_tint = Vector3(1.0, 1.0, 1.0)

// Ambition Thread (Very Fast, Medium)
scanline_opacity = 0.08
scanline_speed = 8.0
scanline_tint = Vector3(1.0, 1.0, 1.0)
```

**Usage in TerminalBase.cs:**

```csharp
// Intensify scanlines during glitch effect
SetShaderParameter(ShaderLayer.Scanline, "scanline_opacity", 0.25f);
SetShaderParameter(ShaderLayer.Scanline, "scanline_speed", 15.0f);
```

## Shader Layer Stack

Applied in this order on TerminalBase.tscn:

1. **PhosphorLayer** - crt_phosphor.gdshader (base color/curvature)
2. **ScanlineLayer** - crt_scanlines.gdshader (movement overlay)
3. **GlitchLayer** - crt_glitch.gdshader (boot/reveal effects)

## crt_glitch.gdshader

**Purpose:** Interference overlay layer for boot sequences and dramatic reveals

**Features:**

- Horizontal scanline displacement (signal corruption)
- Chromatic aberration with random directional offsets
- Digital block corruption artifacts
- Noise/static overlay
- Random full-screen flashes

