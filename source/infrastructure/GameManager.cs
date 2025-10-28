using Godot;

namespace OmegaSpiral.Source.Infrastructure;

/// <summary>
/// Lightweight autoload shim for GameManager used in tests/runtime when the real
/// implementation may live elsewhere. Keeps autoload instantiation from failing.
/// </summary>
public partial class GameManager : Node
{
    // Intentionally minimal. Tests may extend this later or the real GameManager
    // can be used by adjusting project autoload paths.
}
