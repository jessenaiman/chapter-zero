# Godot 4+ GlobalClass and Namespace Strategy

## Decision
Use the `[GlobalClass]` attribute on Godot Node classes to enable proper C# namespace usage while maintaining Godot editor integration.

## Implementation Pattern

### For Godot Node classes that need editor visibility:
```csharp
namespace OmegaSpiral.Combat;

[GlobalClass]
public partial class CombatActor : Node2D
{
    // Class implementation
}
```

### For regular C# classes (not Godot Nodes):
```csharp
namespace OmegaSpiral.Domain.Models;

public class CharacterStats
{
    // Class implementation
}
```

## Benefits
1. **Proper C# organization**: Use namespaces for logical code organization
2. **No CA1050 warnings**: Follows .NET coding standards
3. **Editor integration**: [GlobalClass] makes types available in Godot editor
4. **Better IntelliSense**: IDEs can better organize and suggest code

## What Changed
- Removed CA1050 suppression from GlobalSuppressions.cs
- Added CA1805 suppression (explicit null initialization is acceptable for clarity)
- Need to add [GlobalClass] attribute to all Godot Node classes
- Need to add appropriate namespaces to all C# files

## Namespace Convention
- `OmegaSpiral.Combat` - Combat system
- `OmegaSpiral.Field` - Field/overworld system
- `OmegaSpiral.Ui` - Ui components
- `OmegaSpiral.Domain.Models` - Domain models
- `OmegaSpiral.Infrastructure` - Infrastructure code
- `OmegaSpiral.Tests` - Test classes

## Action Items
1. Add [GlobalClass] attribute to all Godot Node-derived classes
2. Add appropriate namespace declarations to all C# files
3. Update using statements as needed
4. Verify build succeeds with no CA1050 warnings
5. Test in Godot editor to ensure all nodes are still accessible
