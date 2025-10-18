# Quick Start: Test Categorization for Omega Spiral

## 30-Second Summary

Every test needs **5 attributes**:

```csharp
[Test]
[Category("Unit")]              // ONE of: Unit, Integration, EndToEnd, Visual
[Trait("Layer", "Domain")]      // ONE of: Domain, Infrastructure, Presentation
[Trait("Speed", "Fast")]        // ONE of: Fast (<100ms), Slow (>1s)
[Trait("Owner", "Core")]        // ONE of: Core, Content
[Trait("Runtime", "NoGodot")]   // ONE of: NoGodot, RequireGodot
public void TestName_Scenario_Expected() { }
```

**Rule:** If `Trait("Runtime", "RequireGodot")`, add `[RequireGodotRuntime]` attribute.

---

## Local Development Commands

### Before committing code:
```bash
dotnet test --filter "Trait=Speed&Trait=Fast&Trait=Runtime&Trait=NoGodot"
```

### Before pushing to remote:
```bash
dotnet test --filter "Category=Unit"
```

### Working on a specific Act (e.g., Act 1):
```bash
dotnet test --filter "Trait=Scene&Trait=GhostTerminal"
```

---

## Test Categories at a Glance

| Category | Godot? | Speed | When to Use |
|----------|--------|-------|-------------|
| **Unit** | ❌ | ⚡ Fast | Pure C# logic |
| **Integration** | ❌/✅ | 🐢 Slow | Multiple systems working together |
| **EndToEnd** | ✅ Required | 🐢 Slow | Full game flow (Act 1→2, save/load) |
| **Visual** | ✅ Required | 🐢 Slow | UI rendering, scene composition |

---

## Architectural Layers

- **Domain:** Game logic, calculations, rules (e.g., damage calculation)
- **Infrastructure:** I/O, persistence, data loading (e.g., save/load, JSON parsing)
- **Presentation:** UI, scenes, rendering (e.g., dialogue UI, button positioning)

---

## Acts/Scenes

- **GhostTerminal** = Act 1 (Narrative)
- **Nethack** = Act 2 (Terminal dungeon)
- **NeverGoAlone** = Act 3 (Tactical combat)
- **TileDungeon** = Act 4 (Tile exploration)
- **FieldCombat** = Act 5 (Real-time combat)

---

## Full Documentation

See `.github/testing/TEST_CATEGORIZATION.md` for:
- Detailed category descriptions with examples
- All trait systems explained
- Common filter patterns
- Test attribute template
- Troubleshooting guide
