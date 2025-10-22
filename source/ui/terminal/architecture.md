

```mermaid
graph TB
    subgraph "Game Scenes (Consumers)"
        BS[BootSequence<br/>OpeningMonologue<br/>Question1_Name<br/>etc.]
    end
    
    subgraph "Terminal Orchestrator (Composite)"
        TB[TerminalBase<br/>───────────────<br/>Single Responsibility:<br/>Component Lifecycle<br/>& Composition<br/>───────────────<br/>+ Mode: TerminalMode<br/>+ CaptionsEnabled: bool<br/>───────────────<br/>+ AppendTextAsync()<br/>+ PresentChoicesAsync()<br/>+ ClearText()<br/>+ ApplyVisualPresetAsync()<br/>+ PixelDissolveAsync()]
    end
    
    subgraph "Component Interfaces (Abstractions)"
        ISC[ITerminalShaderController<br/>───────────────<br/>+ ApplyVisualPresetAsync()<br/>+ PixelDissolveAsync()<br/>+ ResetShaderEffects()<br/>+ GetCurrentShaderMaterial()]
        
        ITR[ITerminalTextRenderer<br/>───────────────<br/>+ AppendTextAsync()<br/>+ ClearText()<br/>+ SetTextColor()<br/>+ GetCurrentText()<br/>+ ScrollToBottom()<br/>+ IsAnimating()]
        
        ICP[ITerminalChoicePresenter<br/>───────────────<br/>+ PresentChoicesAsync()<br/>+ HideChoices()<br/>+ GetSelectedChoiceIndex()<br/>+ SetChoiceNavigationEnabled()<br/>+ AreChoicesVisible()]
    end
    
    subgraph "Component Implementations (Atomic)"
        SC[TerminalShaderController<br/>───────────────<br/>Single Responsibility:<br/>CRT Shader Effects<br/>───────────────<br/>- _display: ColorRect<br/>- _currentMaterial: ShaderMaterial<br/>───────────────<br/>+ ApplyVisualPresetAsync()<br/>+ PixelDissolveAsync()]
        
        TR[TerminalTextRenderer<br/>───────────────<br/>Single Responsibility:<br/>Text Display & Animation<br/>───────────────<br/>- _textDisplay: RichTextLabel<br/>- _isAnimating: bool<br/>───────────────<br/>+ AppendTextAsync()<br/>+ ClearText()]
        
        CP[TerminalChoicePresenter<br/>───────────────<br/>Single Responsibility:<br/>Choice UI & Selection<br/>───────────────<br/>- _choiceContainer: VBoxContainer<br/>- _choiceButtons: List<br/>───────────────<br/>+ PresentChoicesAsync()<br/>+ HideChoices()]
    end
    
    subgraph "Supporting Components"
        PP[TerminalPresetProvider<br/>───────────────<br/>Static Preset Configs<br/>───────────────<br/>+ PhosphorPreset<br/>+ ScanlinesPreset<br/>+ GlitchPreset<br/>+ CrtPreset]
        
        CO[ChoiceOption<br/>───────────────<br/>Data Class<br/>───────────────<br/>+ Text: string<br/>+ TextColor: Color?<br/>+ IsSelected: bool<br/>+ Metadata: object?]
    end
    
    subgraph "Godot Scene Nodes"
        PL[PhosphorLayer<br/>ColorRect]
        TD[TextDisplay<br/>RichTextLabel]
        CC[ChoiceContainer<br/>VBoxContainer]
        CL[CaptionLabel<br/>Label]
    end
    
    BS -->|inherits| TB
    TB -->|depends on| ISC
    TB -->|depends on| ITR
    TB -->|depends on| ICP
    
    TB -->|creates via factory| SC
    TB -->|creates via factory| TR
    TB -->|creates via factory| CP
    
    SC -.implements.-> ISC
    TR -.implements.-> ITR
    CP -.implements.-> ICP
    
    SC -->|uses| PP
    SC -->|operates on| PL
    TR -->|operates on| TD
    CP -->|operates on| CC
    CP -->|uses| CO
    
    TB -->|references| PL
    TB -->|references| TD
    TB -->|references| CC
    TB -->|references| CL
    
    style TB fill:#e1f5ff,stroke:#01579b,stroke-width:3px
    style ISC fill:#fff9c4,stroke:#f57f17,stroke-width:2px
    style ITR fill:#fff9c4,stroke:#f57f17,stroke-width:2px
    style ICP fill:#fff9c4,stroke:#f57f17,stroke-width:2px
    style SC fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style TR fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style CP fill:#c8e6c9,stroke:#2e7d32,stroke-width:2px
    style PP fill:#f3e5f5,stroke:#6a1b9a,stroke-width:2px
    style CO fill:#f3e5f5,stroke:#6a1b9a,stroke-width:2px
```

---

## Architecture Legend

**Colors:**
- 🔵 **Blue** = Composite Orchestrator (TerminalBase)
- 🟡 **Yellow** = Interfaces (Abstractions)
- 🟢 **Green** = Atomic Components (Implementations)
- 🟣 **Purple** = Supporting/Data Classes

**SOLID Principles Applied:**
1. **Single Responsibility**: Each component has ONE job
   - `TerminalBase`: Component lifecycle only
   - `TerminalShaderController`: CRT effects only
   - `TerminalTextRenderer`: Text display only
   - `TerminalChoicePresenter`: Choice UI only

2. **Open/Closed**: Extensible via factory methods
   - Override `CreateShaderController()`, `CreateTextRenderer()`, `CreateChoicePresenter()` in derived classes

3. **Liskov Substitution**: Interfaces ensure interchangeability
   - Any `ITerminalShaderController` implementation works
   - Any `ITerminalTextRenderer` implementation works
   - Any `ITerminalChoicePresenter` implementation works

4. **Interface Segregation**: Focused, minimal interfaces
   - `ITerminalShaderController`: 4 methods (shader-specific)
   - `ITerminalTextRenderer`: 6 methods (text-specific)
   - `ITerminalChoicePresenter`: 5 methods (choice-specific)

5. **Dependency Inversion**: Depends on abstractions (interfaces), not concrete types
   - `TerminalBase` → `ITerminalShaderController` (not `TerminalShaderController`)
   - `TerminalBase` → `ITerminalTextRenderer` (not `TerminalTextRenderer`)
   - `TerminalBase` → `ITerminalChoicePresenter` (not `TerminalChoicePresenter`)

---

## Component Responsibilities

| Component | Responsibility | Dependencies |
|-----------|---------------|--------------|
| `TerminalBase` | Scene lifecycle, component orchestration | Interfaces only |
| `TerminalShaderController` | CRT shader effects (phosphor, scanlines, glitch) | `ColorRect`, `TerminalPresetProvider` |
| `TerminalTextRenderer` | Text display, typing animations | `RichTextLabel` |
| `TerminalChoicePresenter` | Choice UI, button creation, selection handling | `VBoxContainer`, `ChoiceOption` |
| `TerminalPresetProvider` | Static shader preset configurations | None (static data) |
| `ChoiceOption` | Choice data model | None (data class) |

---

You can copy this directly into your documentation! The diagram shows the clean separation of concerns and SOLID principles in action.