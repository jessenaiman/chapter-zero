# Shader Configuration Architecture - Final Design

## Decision: MINIMAL, PRODUCTION-READY

**Core Principle:** One file, three sections. No wrappers. No presets orchestration. Direct access from controllers.

---

## FILE STRUCTURE

### Keep ✅
```
source/frontend/design/
└── colors_design_config.json          ← SINGLE SOURCE OF TRUTH
    ├── colors: { name: {r,g,b,a} }
    ├── shader_values: { shader_name: { param: value } }
    └── dreamweaver_presets: { preset: { color, shader_overrides } }

source/frontend/shaders/
├── *.gdshader (7 files)               ← The actual shaders
├── *.tres (7 files)                   ← Godot shader materials
└── (no individual shader JSON files)
```

### Delete ❌
- `omega_spiral_colors_config.json` (396 lines, redundant)
- `stage_presets_config.json` (unused)
- `ui_tokens_config.json` (unused)
- `shader_*.json` files (consolidate into colors_design_config.json)
- `StagePreset.cs`, `UiTokensConfig.cs`, `ShaderPreset.cs`, `ShaderParameter.cs`
- `OmegaSpiralConfiguration.cs` (wrapper, no longer needed)

---

## colors_design_config.json STRUCTURE

```json
{
  "colors": {
    "deep_space": { "r": 0.054902, "g": 0.0666667, "b": 0.0862745, "a": 1.0 },
    "light_thread": { "r": 0.95, "g": 0.95, "b": 1.0, "a": 1.0 },
    "shadow_thread": { "r": 1.0, "g": 0.75, "b": 0.2, "a": 1.0 },
    "ambition_thread": { "r": 0.9, "g": 0.15, "b": 0.1, "a": 1.0 }
    // 9 more colors...
  },
  
  "shader_values": {
    "spiral_border": {
      "rotation_speed": 0.05,
      "wave_speed": 0.8,
      "wave_frequency": 8.0,
      "wave_amplitude": 0.25,
      "border_width": 0.015,
      "glow_intensity": 1.2
    },
    "crt_phosphor": {
      "phosphor_glow": 1.2,
      "phosphor_spread": 1.0,
      "vignette_strength": 0.25,
      "scanline_intensity": 0.45
    },
    "crt_scanlines": { "scanline_intensity": 0.4, "scanline_speed": 0.5 },
    "crt_glitch": { "glitch_intensity": 0.3, "color_offset_intensity": 0.15 }
    // 3 more shaders...
  },
  
  "dreamweaver_presets": {
    "light": {
      "primary_color": "light_thread",
      "secondary_color": "shadow_thread",
      "shader_overrides": {
        "crt_phosphor": { "glow_intensity": 1.2, "vignette_strength": 0.15 },
        "crt_scanlines": { "scanline_intensity": 0.25 }
      }
    },
    "shadow": { ... },
    "ambition": { ... }
  }
}
```

**Format rules:**
- Colors: RGB normalized 0-1 (not wrapped)
- Shader values: Direct `"param": value` (no `_doc`, no nested wrappers)
- Dreamweaver presets: Color names + shader parameter overrides
- NO `_doc` fields, NO nested `value` wrappers, NO `color_ref` indirection

---

## C# SERVICE LAYER (Simple)

```csharp
// DesignConfigService.cs - SINGLE class, ~80 lines
public static class DesignConfigService
{
    private static DesignConfig? _config;
    
    public static Color GetColor(string name) => _config.Colors[name];
    
    public static Dictionary<string, object> GetShaderDefaults(string shaderName) 
        => _config.ShaderValues[shaderName];
    
    public static DreamweaverPreset GetPreset(string name) 
        => _config.DreamweaverPresets[name];
}

// DesignConfig.cs - POCO classes only
public class DesignConfig
{
    [JsonProperty("colors")]
    public Dictionary<string, ColorValue> Colors { get; set; }
    
    [JsonProperty("shader_values")]
    public Dictionary<string, Dictionary<string, object>> ShaderValues { get; set; }
    
    [JsonProperty("dreamweaver_presets")]
    public Dictionary<string, DreamweaverPreset> DreamweaverPresets { get; set; }
}

public class ColorValue
{
    [JsonProperty("r")] public float R { get; set; }
    [JsonProperty("g")] public float G { get; set; }
    [JsonProperty("b")] public float B { get; set; }
    [JsonProperty("a")] public float A { get; set; }
}

public class DreamweaverPreset
{
    [JsonProperty("primary_color")] public string PrimaryColor { get; set; }
    [JsonProperty("secondary_color")] public string SecondaryColor { get; set; }
    [JsonProperty("shader_overrides")] 
    public Dictionary<string, Dictionary<string, object>> ShaderOverrides { get; set; }
}
```

---

## USAGE IN CONTROLLERS

```csharp
// OmegaBorderFrame.cs
private void InitializeShader()
{
    var shader = GD.Load<Shader>("res://source/shaders/spiral_border.gdshader");
    _ShaderMaterial = new ShaderMaterial { Shader = shader };
    Material = _ShaderMaterial;
    
    // Apply thread colors
    _ShaderMaterial.SetShaderParameter("light_thread", DesignConfigService.GetColor("light_thread"));
    _ShaderMaterial.SetShaderParameter("shadow_thread", DesignConfigService.GetColor("shadow_thread"));
    
    // Apply shader defaults
    var defaults = DesignConfigService.GetShaderDefaults("spiral_border");
    foreach (var (param, value) in defaults)
        _ShaderMaterial.SetShaderParameter(param, value);
}

// Apply dreamweaver preset
public void ApplyPreset(string presetName)
{
    var preset = DesignConfigService.GetPreset(presetName);
    
    if (preset.ShaderOverrides.TryGetValue("crt_phosphor", out var overrides))
    {
        foreach (var (param, value) in overrides)
            material.SetShaderParameter(param, value);
    }
}
```

---

## FILES TO CREATE/MODIFY

**Create:**
- None needed (reuse existing `DesignConfigService.cs`)

**Modify:**
- `colors_design_config.json` - restructure to three sections
- `DesignConfigService.cs` - simplify to three getters
- `OmegaBorderFrame.cs` - remove `TryGetShaderPreset()`, use `GetShaderDefaults()` + loop

**Delete:**
- `omega_spiral_colors_config.json`
- `stage_presets_config.json`
- `ui_tokens_config.json`
- All `shader_*.json` files
- `StagePreset.cs`, `UiTokensConfig.cs`, `ShaderPreset.cs`, `ShaderParameter.cs`, `OmegaSpiralConfiguration.cs`

---

## VALIDATION

✅ One file. One source of truth.
✅ Flat structure. No wrappers.
✅ Direct access. No orchestration layer.
✅ Production-ready. Used in actual game dev studios.
✅ Designer-friendly. Easy to tweak values.
✅ Ships with demo. Zero scope creep.
