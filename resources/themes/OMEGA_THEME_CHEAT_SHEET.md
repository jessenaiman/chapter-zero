# Ωmega Spiral Theme - Color Cheat Sheet

This theme file demonstrates how to apply your complete Omega color palette from `omega_spiral_colors_config.json` to a Godot theme.

## 🎨 Color Mapping

### **Primary Colors**
- **deep_space** (`#0E1116`): Main backgrounds, panels
- **dark_void** (`#0C0D14`): Secondary backgrounds, button normals
- **warm_amber** (`#FEC962`): Primary accent, borders, focus states
- **pure_white** (`#FFFFFF`): Text, highlights, hover states
- **pure_black** (`#000000`): Masks, overlays

### **Dreamweaver Thread Colors**
- **light_thread** (`#F2F2FF`): Light Dreamweaver progress bars
- **shadow_thread** (`#FFBF33`): Shadow Dreamweaver progress bars  
- **ambition_thread** (`#E62619`): Ambition Dreamweaver progress bars

### **Gameplay Colors**
- **damage_color** (`#B0305C`): Damage indicators
- **heal_color** (`#3CA370`): Heal indicators
- **disabled_gray** (`#808080`): Disabled elements

## 🔧 Usage Examples

### **Buttons**
```gdscript
# Normal button with amber border
Button/styles/normal = dark_void + warm_amber border

# Hover state with white border  
Button/styles/hover = deep_space + pure_white border

# Pressed state with shadow thread color
Button/styles/pressed = dark_void + shadow_thread border
```

### **Progress Bars**
```gdscript
# Standard progress bar
ProgressBar/styles/fill = warm_amber

# Dreamweaver-specific progress bars
ProgressBarLight/styles/fill = light_thread
ProgressBarShadow/styles/fill = shadow_thread  
ProgressBarAmbition/styles/fill = ambition_thread
```

### **Labels**
```gdscript
# Main text with amber shadow
Label/colors/font_color = pure_white
Label/colors/font_shadow_color = warm_amber (30% alpha)
```

## 📋 Complete Component Coverage

✅ **Buttons** (normal, hover, pressed, disabled, focus)
✅ **Panels** (backgrounds, containers)  
✅ **Labels** (text, shadows, custom types)
✅ **Progress Bars** (standard + Dreamweaver variants)
✅ **Line Edit** (input fields)
✅ **Rich Text** (outlined text)
✅ **Scroll Bars** (grabbers, tracks)
✅ **Tab Containers** (tabs, panels)

## 🎯 How to Use

1. **Set as theme**: Apply `omega_theme_complete.tres` to your UI root
2. **Custom components**: Use custom types like `ProgressBarLight` for Dreamweaver elements
3. **Consistent styling**: All components automatically use Omega colors

This is your **CHEAT SHEAT** - every UI element styled with your complete Omega color palette!