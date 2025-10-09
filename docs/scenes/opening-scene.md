# 🎮 **Complete Scene 1 & Scene 2 Godot Drafts**

Based on all details extracted from the chat export, here are comprehensive drafts for both scenes. I've included **every detail, specification, and variation** mentioned so you can edit down rather than assuming what's needed.



## 🎯 **Scene 1: Opening Hook – "The One Story"**

### **Complete Specifications**

#### **Technical Requirements**
- **Godot Version**: 4.5
- **Resolution**: 640×480 (CRT aspect ratio)
- **Font**: Perfect DOS VGA 437 or IBM Plex Mono (size 16)
- **Colors**: Phosphor green (`#33ff33`) on black (`#000000`)

#### **Visual Effects**
- **CRT Scanlines**: Animated horizontal lines across screen
- **Screen Curvature**: Subtle barrel distortion at edges
- **Typewriter Effect**: Character-by-character text reveal (50ms per char)
- **Optional Audio**: CRT hum + mechanical click per keystroke

#### **Content Structure** (`res://content/scene1_content.json`)
```json
{
  "scene": "scene1",
  "title": "The One Story",
  "question": "If you could hear only one story... what would it be?",
  "options": [
    {
      "id": "fantasy",
      "key": "1",
      "text": "A fantasy — where light and shadow wage war",
      "description": "Epic tales of magic, heroes, and ancient powers"
    },
    {
      "id": "mystery", 
      "key": "2",
      "text": "A mystery — clues hidden in the silence",
      "description": "Puzzles, secrets, and hidden truths"
    },
    {
      "id": "horror",
      "key": "3", 
      "text": "A horror — that whispers your name in the dark",
      "description": "Dark tales that chill the soul"
    },
    {
      "id": "romance",
      "key": "4",
      "text": "A romance — written in stardust and sacrifice", 
      "description": "Love stories that transcend time"
    }
  ],
  "default_seed": "fantasy",
  "timeout_seconds": 15,
  "output_file": "user://scene1_result.json",
  "visual_effects": {
    "scanlines": true,
    "curvature": true,
    "crt_hum": false,
    "typewriter_sound": false
  }
}
```

#### **Complete Scene Flow**

1. **Input API json feed**: Load into actual boot sequence
2. **Boot Sequence**: `[Shard #472: 03/09/2025 - Echo archive active...Echo...]`
3. **Question Display**: Types the question character-by-character
4. **Option Display**: Shows numbered options after question completes
5. **Player Input**: Press 1-3 to select, or timeout to default
6. **Output**: Writes `{"scene": "scene1", "narrative_seed": "fantasy", "timestamp": "..."}`
